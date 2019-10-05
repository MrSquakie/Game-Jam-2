﻿namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;
    using System;

    public class LocomotionSystemTank : ILocomotionSystem 
	{
		// PROPERTIES: ----------------------------------------------------------------------------

		protected Vector3 desiredDirection = Vector3.zero;
		protected float rotationY = 0f;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
		{
            base.Update();

			if (this.characterLocomotion.navmeshAgent != null)
			{
				this.characterLocomotion.navmeshAgent.updatePosition = false;
				this.characterLocomotion.navmeshAgent.updateUpAxis = false;
			}

			Vector3 targetDirection = this.desiredDirection;
			Quaternion targetRotation = Quaternion.identity;

            CharacterController controller = this.characterLocomotion.characterController;

			float targetSpeed = this.CalculateSpeed(targetDirection, controller.isGrounded);
			if (targetDirection == Vector3.zero)
			{
				targetDirection = this.movementDirection;
				targetSpeed = 0f;
			}

			float speed = this.CalculateAccelerationFromSpeed(targetSpeed);

			this.UpdateAnimationConstraints(ref targetDirection, ref targetRotation);
			targetDirection *= speed;

            this.pivotSpeed = this.rotationY * this.characterLocomotion.angularSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(Vector3.up * this.pivotSpeed);

            this.UpdateSliding();

			if (this.isSliding) targetDirection = this.slideDirection;
			targetDirection += Vector3.up * this.characterLocomotion.verticalSpeed;

			if (this.isDashing)
			{
				targetDirection = this.dashVelocity;
				targetRotation = controller.transform.rotation;
			}

			controller.Move(targetDirection * Time.deltaTime);
			controller.transform.rotation *= targetRotation;

			if (this.characterLocomotion.navmeshAgent != null && 
                this.characterLocomotion.navmeshAgent.isActiveAndEnabled)
			{
                this.characterLocomotion.navmeshAgent.enabled = false;
            }

            return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
		}

        public override void OnDestroy ()
		{
			return;
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetDirection(Vector3 direction, float rotationY)
		{
			this.desiredDirection = direction;
            this.rotationY = rotationY;
		}
    }
}
