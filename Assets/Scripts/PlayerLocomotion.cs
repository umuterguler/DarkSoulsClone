using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

namespace SG
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private Transform cameraObject;
        private InputHandler inputHandler;
        private Vector3 moveDirection;

        [HideInInspector] public Transform myTransorm;
        [HideInInspector] public AnimationHandler animationHandler;
        
        [HideInInspector] public new Rigidbody rigidBody;
        [HideInInspector] public GameObject normalCamera;

        [Header("Stats")] 
        [SerializeField] private float movementSpeed = 3;
        [SerializeField] private float rotationSpeed = 10;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animationHandler = GetComponentInChildren<AnimationHandler>();
            cameraObject = Camera.main.transform;
            myTransorm = transform;
            animationHandler.Initialize();
        }

        public void Update()                    // inputhandlerda alınan vertical ve horizontalı kullanarak hareket yönünü set etti
        {                                       // normalize edip hızla çarptı ve projectonplane kullandı bunu araştır***   
            float delta = Time.deltaTime;       // canrotatese bakış açısını da değiştirdi
            
            inputHandler.TickInput(delta);

            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidBody.velocity = projectedVelocity;

            animationHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);
            
            if (animationHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        #region Movement
        private Vector3 normalVector;
        private Vector3 targetPosition;

        private void HandleRotation(float delta) // input handlerda alınan vertical ve horizontalı kullanarak targetdir'ı set ediyor
        {                                        // targetdir ile tr'ı lookrotation kullanarak hesaplıyor
            Vector3 targetDir = Vector3.zero;    // targetrotationı slerp ile hızı da dahil ederek son haline getiriyor
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;
            
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransorm.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransorm.rotation, tr, rs * delta);

            myTransorm.rotation = targetRotation;
        }
        
        #endregion

    }
}
