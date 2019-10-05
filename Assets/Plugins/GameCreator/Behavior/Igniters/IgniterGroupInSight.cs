namespace GameCreator.Behavior
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Variables;

    [AddComponentMenu("")]
    public class IgniterGroupInSight : Igniter 
	{
        #if UNITY_EDITOR
        public new static string ICON_PATH = "Assets/Plugins/GameCreator/Behavior/Icons/Igniters/";
        public const string CUSTOM_ICON_PATH = "Assets/Plugins/GameCreator/Behavior/Icons/Igniters/";

        public new static string NAME = "Behavior/Group in Sight";
        #endif

        public enum Operation
        {
            OnSight,
            OnLoseSight,
        }

        public TargetGameObject observer = new TargetGameObject(TargetGameObject.Target.Invoker);
        [Space] public HelperListVariable listVariables = new HelperListVariable();
        public Operation when = Operation.OnSight;

        [Space, VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeObserved = new VariableProperty(Variable.VarType.GlobalVariable);

        private Perception cacheObserver;
        private ListVariables cacheList;

        private void Start()
        {
            this.cacheObserver = this.observer.GetComponent<Perception>(gameObject);
            this.cacheList = this.listVariables.GetListVariables(gameObject);

            if (this.cacheList == null || this.cacheObserver == null) return;
            VariablesManager.events.StartListenListAdd(this.OnListCallback, this.cacheList.gameObject);
            VariablesManager.events.StartListenListRmv(this.OnListCallback, this.cacheList.gameObject);
            VariablesManager.events.StartListenListChg(this.OnListCallback, this.cacheList.gameObject);

            for (int i = 0; i < this.cacheList.variables.Count; ++i)
            {
                GameObject value = this.cacheList.variables[i].Get<GameObject>();
                this.OnListCallback(i, null, value);
            }
        }

        private void OnDestroy()
        {
            ListVariables list = this.listVariables.GetListVariables(gameObject);
            if (list != null) return;

            VariablesManager.events.StopListenListAdd(this.OnListCallback, this.cacheList.gameObject);
            VariablesManager.events.StopListenListRmv(this.OnListCallback, this.cacheList.gameObject);
            VariablesManager.events.StopListenListChg(this.OnListCallback, this.cacheList.gameObject);
        }

        private void OnListCallback(int index, object prev, object next)
        {
            if (this.cacheList == null || this.cacheObserver == null) return;
            GameObject goPrev = prev as GameObject;
            GameObject goNext = next as GameObject;

            if (goPrev != null) this.cacheObserver.StopListenPerceptron(Perception.Type.Sight, goPrev, this.PerceptionCallback);
            if (goNext != null) this.cacheObserver.StartListenPerceptron(Perception.Type.Sight, goNext, this.PerceptionCallback);
        }

        private void PerceptionCallback(bool inSight, GameObject target)
        {
            GameObject goObserver = (this.cacheObserver == null
                ? this.observer.GetGameObject(gameObject)
                : this.cacheObserver.gameObject
            );

            this.storeObserved.Set(target, gameObject);

            switch (this.when)
            {
                case Operation.OnSight: if (inSight) this.ExecuteTrigger(goObserver); break;
                case Operation.OnLoseSight: if (!inSight) this.ExecuteTrigger(goObserver); break;
            }
        }
    }
}