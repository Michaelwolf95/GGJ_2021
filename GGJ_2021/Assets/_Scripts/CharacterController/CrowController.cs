using System;
using System.Collections;
using System.Collections.Generic;
using MichaelWolfGames;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CrowGame
{
    public class CrowController : MonoBehaviour
    {
        [SerializeField] private CharacterController moveController;
        [SerializeField] private PlayerInput inputController;
        [SerializeField] private Animator animator;
        //private Transform characterRoot => animator.transform;
        [SerializeField] private Transform characterRoot;
        [SerializeField] private Camera camera;
        
        [Header("Control Parameters")]
        [SerializeField] private float groundedMoveSpeed = 4f;
        [SerializeField] private float glideMoveSpeed = 4f;
        [SerializeField] private float flapForce = 5f;
        [SerializeField] private int maxJumps = 3;
        [SerializeField] private float defaultGravity = -9.8f;
        [SerializeField] private float glideGravity = -4.5f;
        
        

        private int currentJumpsSinceGrounded;
        private bool lastFramePress = false; 
        public enum ControlState
        {
            Grounded,
            Flying
        }
        
        
        private Vector3 groundNormal;
        private ControllerColliderHit currentControllerHit = null;

        private EventButton jumpButton = null;
        
        private void Awake()
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
            inputController.camera = camera;

            jumpButton = new EventButton(inputController, "Jump");
        }

        private void Update()
        {
            jumpButton.HandleUpdate(Time.deltaTime);

            Vector3 currentVelocity = moveController.velocity;
            
            Vector2 moveInput = inputController.actions["Move"].ReadValue<Vector2>();
            Vector3 moveDelta = Vector3.zero;
            
            if (moveController.isGrounded)
            {
                currentJumpsSinceGrounded = 0; 
                animator.SetBool("flying", false);
                
                if (jumpButton.isPressedDown)
                {
                    // JUMP FROM GROUND ====
                    Debug.Log("Jump From Ground");
                    //currentVelocity.y = 0f;
                    moveDelta.y = flapForce * Time.deltaTime;
                    
                    currentJumpsSinceGrounded++;
                    animator.SetBool("flying", true);
                }
                else
                {
                    Debug.Log("On Ground");
                    HandleGroundedMovement(moveInput, ref moveDelta);
                }
            }
            else // Not grounded
            {
                Vector3 moveDir = GetMoveDirectionXZ(moveInput);
                Debug.Log("MOVE DIR: " + moveDir);
                
                moveDelta = currentVelocity;
                //Debug.Log("MOVE DIR: " + moveDir);
                
                if (jumpButton.isPressedDown && currentJumpsSinceGrounded < maxJumps)
                {
                    Debug.Log("Mid Air Jump");
                    // Mid Air Jump
                    currentJumpsSinceGrounded++;
                    moveDelta.y = flapForce * Time.deltaTime;
                }
                else
                {
                    Debug.Log("Falling or Gliding");
                    //moveDelta = moveController.velocity;
                    
                    if (jumpButton.isPressed) // Held or pressed
                    {
                        //Debug.Log("Glide Gravity");
                        // Glide gravity
                        moveDelta.y += glideGravity * Time.deltaTime;
                    }
                    else
                    {
                        //Debug.Log("Fall Gravity: " + defaultGravity * Time.deltaTime + ", " + currentVelocity + ", " );
                        // Fall Gravity
                        moveDelta.y += defaultGravity * Time.deltaTime;
                    }
                    
                    moveDelta += moveDir * glideMoveSpeed * Time.deltaTime;
                    
                    // ToDo: Smooth this.
                    characterRoot.LookAt(characterRoot.transform.position + moveDir.normalized);
                }
                
            }
            
            
            moveController.Move(moveDelta);
        }

        private void HandleGroundedMovement(Vector2 moveInput, ref Vector3 moveDelta)
        {
            float slopeAngle = 0f;
            if (currentControllerHit != null)
            {
                // ToDo: Reenable this later.
                //slopeAngle = Vector3.Angle(Vector3.up, currentControllerHit.normal);
            }
            
            // Slide down slope (Not used)
            if (slopeAngle > moveController.slopeLimit) // && currentControllerHit != null
            {
                Vector3 g = Vector3.Cross(Vector3.Cross(currentControllerHit.normal, Vector3.down), currentControllerHit.normal);
                g *= groundedMoveSpeed * Time.deltaTime;
                //moveDelta += g;
                //Debug.Log(slopeAngle + ", " + currentControllerHit.collider.gameObject.name + ", " + currentControllerHit.normal);
                Debug.DrawLine(currentControllerHit.point + currentControllerHit.normal, currentControllerHit.point + currentControllerHit.normal + (g * 2f), Color.magenta, 0.1f);
            }
            else
            {
                Vector3 moveDir = GetMoveDirectionXZ(moveInput);
                moveDelta += moveDir * groundedMoveSpeed * Time.deltaTime;
                
                // ToDo: Smooth this.
                characterRoot.LookAt(characterRoot.transform.position + moveDir.normalized);
            }
            
            
            //animator.SetInteger("hop", (int)moveInput.magnitude);
            
            moveDelta.y += defaultGravity * Time.deltaTime;
        }

        private Vector3 GetMoveDirectionXZ(Vector2 moveInput)
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Quaternion.AngleAxis(90f, Vector3.up) * cameraForward;
                    
            Debug.DrawLine(transform.position, transform.position + cameraForward * 2f, Color.blue);
            Debug.DrawLine(transform.position, transform.position + cameraRight * 2f, Color.red);

            return ((cameraForward * moveInput.y) + (cameraRight * moveInput.x)).normalized;
        }
        
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //Debug.Log("Normal vector we collided at: " + hit.normal);
            currentControllerHit = hit;
        }

    }
}
