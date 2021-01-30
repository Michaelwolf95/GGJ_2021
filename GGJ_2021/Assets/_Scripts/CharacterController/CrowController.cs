using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CrowGame
{
    public class CrowController : MonoBehaviour
    {
        [SerializeField] private CharacterController moveController;
        [SerializeField] private PlayerInput inputController;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private Animator animator;
        //private Transform characterRoot => animator.transform;
        [SerializeField] private Transform characterRoot;
        [SerializeField] private Camera camera;

        public enum ControlState
        {
            Grounded,
            Flying
        }
        
        
        private Vector3 groundNormal;
        private ControllerColliderHit currentControllerHit = null;

        private void Awake()
        {
            inputController.actions["Jump"].performed += ctx =>
            {
                HandleJumpInput();
            };
        }

        private void HandleJumpInput()
        {
            //Debug.Log("Jump");
        }

        private void Update()
        {
            Vector2 moveInput = inputController.actions["Move"].ReadValue<Vector2>();

            bool jump = inputController.actions["Jump"].ReadValue<float>() != 0;
            

            Vector3 moveDelta = Vector3.zero;
            if (moveController.isGrounded)
            {
                float slopeAngle = 0f;
                if (currentControllerHit != null)
                {
                    // ToDo: Reenable this later.
                    //slopeAngle = Vector3.Angle(Vector3.up, currentControllerHit.normal);
                }
                
                if (slopeAngle > moveController.slopeLimit) // && currentControllerHit != null
                {
                    Vector3 g = Vector3.Cross(Vector3.Cross(currentControllerHit.normal, Vector3.down), currentControllerHit.normal);
                    g *= moveSpeed * Time.deltaTime;
                    //moveDelta += g;
                    //Debug.Log(slopeAngle + ", " + currentControllerHit.collider.gameObject.name + ", " + currentControllerHit.normal);
                    Debug.DrawLine(currentControllerHit.point + currentControllerHit.normal, currentControllerHit.point + currentControllerHit.normal + (g * 2f), Color.magenta, 0.1f);
                }
                else
                {
                    Vector3 cameraForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
                    Vector3 cameraRight = Quaternion.AngleAxis(90f, Vector3.up) * cameraForward;
                    
                    Debug.DrawLine(transform.position, transform.position + cameraForward * 2f, Color.blue);
                    Debug.DrawLine(transform.position, transform.position + cameraRight * 2f, Color.red);

                    Vector3 moveDir = ((cameraForward * moveInput.y) + (cameraRight * moveInput.x)); //ToDo: Normalize this?

                    moveDelta += moveDir * moveSpeed * Time.deltaTime;
                    
                    characterRoot.LookAt(characterRoot.transform.position + moveDir.normalized);
                }
                
                //animator.SetFloat("Speed", moveInput.magnitude);
            }
            else // Not grounded
            {
                
            }
            moveDelta += Physics.gravity * Time.deltaTime;
            moveController.Move(moveDelta);
        }
        
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //Debug.Log("Normal vector we collided at: " + hit.normal);
            currentControllerHit = hit;
        }

    }
}
