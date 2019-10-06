namespace GameCreator.Core
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Characters;
	using UnityEngine.Events;
    using GameCreator.Variables;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("")]
	public class ActionWanderAround : IAction
	{
        public enum MOVE_BY
        {
            Marker,
            Range
        }

        public enum CHARACTER_SPEED
        {
            Run,
            Walk,
            Random
        }

        #region Properties

        // Wandertype -------------------------------------------------------------
        public MOVE_BY moveby = MOVE_BY.Range;

        // Character --------------------------------------------------------------

        public TargetCharacter target = new TargetCharacter();
        private Character character = null;
        public CHARACTER_SPEED characterSpeed = CHARACTER_SPEED.Walk;

        // RangeSettings ----------------------------------------------------------

        public float minRange = 3f;
        public float maxRange = 8f;

        // MarkerSettings ---------------------------------------------------------

        public bool randomMarker = true;
        public GameObject marker;
        GameObject[] childs;
        private float currentMarkerInt = 0f;
        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);
        public VariableProperty currentMarker = new VariableProperty(Variable.VarType.LocalVariable);

        // IdleTime ----------------------------------------------------------------

        public float minWaitTime = 1.0f;
        public float maxWaitTime = 2.0f;
        private bool forceStop = false;
        public bool cancelable = false;
        private bool isCharacterMoving = false;

        #endregion

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            float stopTime = Time.time + UnityEngine.Random.Range(this.minWaitTime, this.maxWaitTime);
            WaitUntil waitUntil = new WaitUntil(() => Time.time > stopTime || this.forceStop);
            this.character = this.target.GetCharacter(target);

            Vector3 cPosition = Vector3.zero;
            ILocomotionSystem.TargetRotation cRotation = null;
            float cStopThresh = 0f;

            this.character.characterLocomotion.canRun = getRunSpeed();
            this.GetTarget(this.character, target, ref cPosition, ref cRotation, ref cStopThresh);

            this.isCharacterMoving = true;

            while (isCharacterMoving && !forceStop)
            {
                this.character.characterLocomotion.SetTarget(cPosition, cRotation, cStopThresh, this.CharacterArrivedCallback);

                yield return waitUntil;
                yield return 0;
            }
            if(forceStop)
            {
                this.character.characterLocomotion.SetTarget(character.transform.position, cRotation, cStopThresh, this.CharacterArrivedCallback);
            }
            yield return 0;
        }

        public override void Stop()
        {
            this.forceStop = true;
        }

        public void CharacterArrivedCallback()
        {
            this.isCharacterMoving = false;
        }

        private void GetTarget(Character targetCharacter, GameObject invoker, ref Vector3 cPosition, ref ILocomotionSystem.TargetRotation cRotation, ref float cStopThresh)
        {
            switch (moveby)
            {
                case MOVE_BY.Marker:
                    {
                        //Collect the WaypointGroup out of the local var (Blackboard)
                        marker = this.variable.Get(invoker) as GameObject;
                        //Count the number of waypoints
                        childs = new GameObject[marker.transform.childCount];

                        if (randomMarker)
                        {
                            NavigationMarker nextMarker = pickRandomMarker();
                            cPosition = nextMarker.transform.position;
                            cRotation = new ILocomotionSystem.TargetRotation(true, nextMarker.transform.forward);
                            cStopThresh = nextMarker.stopThreshold;
                            break;
                        }
                        else
                        {
                            //Get the current int value of currentmaker of the local var
                            currentMarkerInt = Convert.ToInt32(this.currentMarker.ToStringValue(invoker));
                            //get the next marker
                            NavigationMarker nextMarker = getNextMarker(invoker);
                            cPosition = nextMarker.transform.position;
                            cRotation = new ILocomotionSystem.TargetRotation(true, nextMarker.transform.forward);
                            cStopThresh = nextMarker.stopThreshold;
                            break;
                        }
                    }
                case MOVE_BY.Range:
                    {
                        float x_range = UnityEngine.Random.Range(0 - UnityEngine.Random.Range(minRange,maxRange), UnityEngine.Random.Range(minRange, maxRange));
                        float z_range = UnityEngine.Random.Range(0 - UnityEngine.Random.Range(minRange, maxRange), UnityEngine.Random.Range(minRange, maxRange));
                        cPosition = new Vector3(x_range, x_range, z_range);
                        cRotation = new ILocomotionSystem.TargetRotation(false);
                        break;
                    }
            }
        }

        /// <summary>
        /// Set the speed of movement
        /// </summary>
        /// <returns></returns>
        private bool getRunSpeed()
        {
            switch (characterSpeed)
            {
                case CHARACTER_SPEED.Run: return true;
                case CHARACTER_SPEED.Walk: return false;
                case CHARACTER_SPEED.Random:
                    {
                        int i = UnityEngine.Random.Range(0, 20);
                        return i < 10 ? false : true;
                    }
                default: return false;
            }
        }

        #region getMarkers

        /// <summary>
        /// Picks a random Marker out of the array
        /// </summary>
        /// <returns>Gameobject(Marker)</returns>
        private NavigationMarker pickRandomMarker()
        {           
            return marker.transform.GetChild((int)UnityEngine.Random.Range(0f, childs.Length)).gameObject.GetComponent<NavigationMarker>();
        }

        /// <summary>
        /// Picks the next Marker out of the array
        /// </summary>
        /// <returns>GameObject(Marker)</returns>
        private NavigationMarker getNextMarker(GameObject invoker)
        {
            /* Is the currentMarker (float) smaller than the arraylenght minus 1  
             * Add +1 to currentMarker and return the Gameobject
             * If currentMarker is not smaller than set the currentMarker back to 0
             * In this case the cycle begins again.
             */

            if (currentMarkerInt < childs.Length - 1)
            {
                currentMarkerInt++;
                this.currentMarker.Set(currentMarkerInt,invoker);
            }
            else
            {
                currentMarkerInt = 0f;
                this.currentMarker.Set(currentMarkerInt,invoker);
            }
            return marker.transform.GetChild((int)currentMarkerInt).gameObject.GetComponent<NavigationMarker>();
        }

        #endregion

        #region UI Settings for Unity-Editor

#if UNITY_EDITOR
        public static new string NAME = "Character/ActionWanderAround";

        private static readonly GUIContent GUICONTENT_WANDERTYPE = new GUIContent("Wandertype","Choose between 'Ranged' and 'Marker'\r\nRanged: Character wanders around in a range around him (min/max)\r\nMarker: Character wander along all markers in a row or random between them if selected");
        private static readonly GUIContent GUICONTENT_RANGEHEADER = new GUIContent("Wander by min/max range","MinRange : The smalles amount which the character will move.\r\nMaxRange: The biggest amount which the character will move");
        private static readonly GUIContent GUICONTENT_MARKERHEADER = new GUIContent("Wander by Marker");
        private static readonly GUIContent GUICONTENT_WALKRANDOM = new GUIContent("Walk random", "The markers are picked randomly");
        private static readonly GUIContent GUICONTENT_WALKSPEED = new GUIContent("Walkspeed", "Walk around, run around or randomly each cycle");
        private static readonly GUIContent GUICONTENT_CURRMARKER = new GUIContent("Current Marker", "Create a group of markers and drop them as gameobject into this field.");
        private static readonly GUIContent GUICONTENT_WAITTIME = new GUIContent("Idle at the new position (s)", "Time where the character idles between each step");
        private static readonly GUIContent GC_CANCEL = new GUIContent("Cancelable", "Action can be interrupted");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spWanderType;
        private SerializedProperty spWanderMinRange;
        private SerializedProperty spWanderMaxRange;
        private SerializedProperty spRandomMarker;
        private SerializedProperty spMinWaitTime;
        private SerializedProperty spMaxWaitTime;
        private SerializedProperty spMarker;
        private SerializedProperty spCurrMarker;
        private SerializedProperty spWalkSpeed;
        private SerializedProperty spCancelable;
        private SerializedProperty spCancelDelay;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string NODE_TITLE = "";
            if (moveby == MOVE_BY.Range)
            {
                NODE_TITLE = "Wander ranged (min/max {0}/{1}) and idle between {2}&{3} seconds";
                return string.Format(NODE_TITLE, this.minRange, this.maxRange, this.minWaitTime, this.maxWaitTime);
            }
            else
            {
                if(randomMarker)
                {
                    NODE_TITLE = "Wander random markers and idle between {0}&{1} seconds";
                    return string.Format(NODE_TITLE, this.minWaitTime, this.maxWaitTime);
                }
                else
                {                  
                    NODE_TITLE = "Wander marker from first to last and idle between {1}&{2} seconds";
                    return string.Format(NODE_TITLE, childs.Length-1, this.minWaitTime, this.maxWaitTime);
                }
            }          
        }

        protected override void OnEnableEditorChild()
        {
            this.spTarget = this.serializedObject.FindProperty("target");
            this.spWanderType = this.serializedObject.FindProperty("moveby");
            this.spWanderMinRange = this.serializedObject.FindProperty("minRange");
            this.spWanderMaxRange = this.serializedObject.FindProperty("maxRange");
            this.spRandomMarker = this.serializedObject.FindProperty("randomMarker");
            this.spMinWaitTime = this.serializedObject.FindProperty("minWaitTime");
            this.spMaxWaitTime = this.serializedObject.FindProperty("maxWaitTime");
            this.spCurrMarker = this.serializedObject.FindProperty("currentMarker");
            this.spMarker = this.serializedObject.FindProperty("variable");
            this.spWalkSpeed = this.serializedObject.FindProperty("characterSpeed");
            this.spCancelable = this.serializedObject.FindProperty("cancelable");
        }

        protected override void OnDisableEditorChild()
        {
            this.spTarget = null;
            this.spWanderType = null;
            this.spWanderMinRange = null;
            this.spWanderMaxRange = null;
            this.spRandomMarker = null;
            this.spMinWaitTime = null;
            this.spMaxWaitTime = null;
            this.spMarker = null;
            this.spCurrMarker = null;
            this.spWalkSpeed = null;
            this.spCancelable = null;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.PropertyField(this.spWalkSpeed, GUICONTENT_WALKSPEED);
            EditorGUILayout.PropertyField(this.spWanderType, GUICONTENT_WANDERTYPE);
            EditorGUILayout.PropertyField(this.spCancelable, GC_CANCEL, GUILayout.MinWidth(50));
            GUILayout.Space(5);

            #region RangeWandering
            if (moveby == MOVE_BY.Range)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label(GUICONTENT_RANGEHEADER, EditorStyles.boldLabel);
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(this.spWanderMinRange, GUILayout.MinWidth(50));
                EditorGUILayout.PropertyField(this.spWanderMaxRange, GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
            #endregion

            #region MarkerWandering

            if (moveby == MOVE_BY.Marker)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label(GUICONTENT_MARKERHEADER, EditorStyles.boldLabel);
                GUILayout.Space(3);
                EditorGUILayout.PropertyField(this.spRandomMarker, GUICONTENT_WALKRANDOM, GUILayout.MinWidth(50));
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(this.spMarker, true, GUILayout.MinWidth(70));
                if(!randomMarker)
                {
                    EditorGUILayout.PropertyField(this.spCurrMarker, GUICONTENT_CURRMARKER, true, GUILayout.MinWidth(70));
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }

            #endregion

            #region Idle after wandering

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(GUICONTENT_WAITTIME, EditorStyles.boldLabel);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(this.spMinWaitTime, GUILayout.MinWidth(50));
            EditorGUILayout.PropertyField(this.spMaxWaitTime, GUILayout.MinWidth(50));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(5);
            #endregion

            

            this.serializedObject.ApplyModifiedProperties();
        }
#endif

        #endregion
    }
}
