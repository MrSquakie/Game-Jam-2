namespace GameCreator.Behavior
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Characters;

    [Serializable]
    public class PerceptronSight : PerceptronBase
    {
        public const float DEFAULT_FOV = 114f;
        public const float DEFAULT_DISTANCE = 100f;

        // PROPERTIES: ----------------------------------------------------------------------------

        private Transform eyes;

        // INITIALIZERS: --------------------------------------------------------------------------

        public override void Awake(Perception perception)
        {
            base.Awake(perception);
            this.eyes = PerceptronSight.GetEyes(perception);
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            List<int> removeCandidates = new List<int>();

            foreach (KeyValuePair<int, Tracker> item in this.trackers)
            {
                Tracker tracker = item.Value;

                if (tracker.reference == null) removeCandidates.Add(item.Key);
                else this.UpdateTracker(tracker); 
            }

            for (int i = removeCandidates.Count - 1; i >= 0; --i)
            {
                int instanceID = removeCandidates[i];
                this.trackers[instanceID].SetState(false);
                this.trackers.Remove(instanceID);
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool CanSee(GameObject target)
        {
            if (target == null) return false;

            int targetID = target.GetInstanceID();
            if (this.trackers.ContainsKey(targetID)) return this.trackers[targetID].GetState();

            Vector3 position = target.transform.position;
            CharacterController characterController = target.GetComponent<CharacterController>();
            if (characterController != null)
            {
                position += characterController.center;
            }

            return this.CheckCanSee(target, position);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void UpdateTracker(Tracker tracker)
        {
            tracker.UpdateMemory();

            bool canSee = this.CheckCanSee(tracker.reference, tracker.GetPosition());
            tracker.SetState(canSee);
        }

        private bool CheckCanSee(GameObject target)
        {
            return this.CheckCanSee(target, target.transform.position);
        }

        private bool CheckCanSee(GameObject target, Vector3 sightTarget)
        {
            Vector3 position1 = this.eyes.position;
            Vector3 position2 = sightTarget;

            float distance = Vector3.Distance(position1, position2);
            Ray ray = new Ray(position1, (position2 - position1));
            QueryTriggerInteraction query = QueryTriggerInteraction.Ignore;

            RaycastHit hitInfo;
            if (distance <= this.perception.visionDistance &&
                Physics.Raycast(ray, out hitInfo, this.perception.visionDistance, this.perception.sightLayerMask, query))
            {
                float angle = Vector3.Angle(
                    this.perception.transform.forward, 
                    ray.direction.normalized
                );

                bool state = (
                    hitInfo.collider.gameObject == target &&
                    angle <= this.perception.fieldOfView / 2f
                );

                return state;
            }

            return false;
        }

        // HELPER METHODS: ------------------------------------------------------------------------

        public static Transform GetEyes(Perception perception)
        {
            Transform sight = perception.transform;
            CharacterAnimator characterAnimator = perception.GetComponent<CharacterAnimator>();
            if (characterAnimator != null && characterAnimator.animator != null)
            {
                Transform head = characterAnimator.animator.GetBoneTransform(HumanBodyBones.Head);
                if (head != null) sight = head;
            }

            return sight;
        }
    }
}