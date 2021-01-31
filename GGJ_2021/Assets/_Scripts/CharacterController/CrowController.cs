using System;
using System.Collections;
using System.Collections.Generic;
using MichaelWolfGames;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CrowGame
{
    public class CrowController : MonoBehaviour
    {
        [SerializeField] private CharacterController moveController;
        [SerializeField] private PlayerInput inputController;
        [SerializeField] private Animator animator;
        //private Transform characterRoot => animator.transform;
        [SerializeField] private Transform characterRoot;
        [SerializeField] private Transform rotationRoot;
        [SerializeField] private Camera camera;

        [Header("Control Parameters")]
        [SerializeField] private float groundedMoveSpeed = 4f;
        [SerializeField] private float groundedTurnSpeed = 300f;
        [SerializeField] private float glideMoveSpeed = 4f;
        [SerializeField] private float glideTurnSpeed = 130f;
        [SerializeField] private float fallMoveSpeed = 4f;
        [SerializeField] private float flapForce = 5f;
        [SerializeField] private int maxJumps = 3;
        [SerializeField] private float defaultGravity = -9.8f;
        [SerializeField] private float glideGravity = -4.5f;
        [SerializeField] private float rotateSpeed = 2f;

        private CrowAnimationAudio animationAudioController = null;

        private int numJumpsSinceGrounded = 0;
        
        private Vector3 groundNormal;
        private ControllerColliderHit currentControllerHit = null;
        private EventButton jumpButton = null;
        private EventButton cawButton = null;

        private bool wasGrounded = false;
        private bool isGliding = false;
        //private bool glidePress = false;
        
        private void Awake()
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
            inputController.camera = camera;

            jumpButton = new EventButton(inputController, "Jump");

            if (animator)
            {
                animationAudioController = animator.GetComponent<CrowAnimationAudio>();
            }

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            jumpButton.HandleUpdate(Time.deltaTime);
            
            if (inputController.actions["Pause"].ReadValue<float>() > 0)
            {
                // ToDo: Replace this when we have the pause menu.
                Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked)? CursorLockMode.None : CursorLockMode.Locked;
            }

            Vector2 moveInput = inputController.actions["Move"].ReadValue<Vector2>();
            Vector3 moveDelta = Vector3.zero;
            
            Vector3 currentVelocity = moveController.velocity;
            float currentGravityY = defaultGravity;

            if (moveController.isGrounded)
            {
                numJumpsSinceGrounded = 0; 
                animator.SetBool("flying", false);
                animator.SetBool("Gliding", false);
                isGliding = false;
                if (jumpButton.isPressedDown)
                {
                    // JUMP FROM GROUND ====
                    moveDelta.y = flapForce * Time.deltaTime;
                    
                    numJumpsSinceGrounded++;
                    animator.SetBool("flying", true);
                    currentGravityY = 0f; // No gravity this frame.
                    currentVelocity.y = 0f;
                    
                    animator.SetTrigger("Flap");
                }
                else {
                    // GROUND MOVEMENT
                    HandleGroundedMovement(moveInput, ref moveDelta);
                    currentGravityY = defaultGravity;
                }

                if (wasGrounded == false)
                {
                    // LANDED
                    animationAudioController.PlayLandOnGroundSound();
                    rotationRoot.localRotation = Quaternion.identity;
                    
                }
                
            }
            else // Not grounded
            {
                Vector3 moveDir = GetMoveDirectionXZ(moveInput);
                bool specialJumpFrame = false;
                if (jumpButton.isPressedDown)
                {
                    if (numJumpsSinceGrounded < maxJumps)
                    {
                        // MID AIR JUMP =========
                        numJumpsSinceGrounded++;
                        moveDelta.y = flapForce * Time.deltaTime;
                        currentGravityY = 0f; // No gravity this frame.
                        currentVelocity.y = 0f;
                        animator.SetTrigger("Flap");
                        specialJumpFrame = true;
                    }
                    else if (isGliding == false)
                    {
                        // START GLIDE
                        isGliding = true;
                        specialJumpFrame = true; // Why Not??
                        animator.SetBool("Gliding", true);
                    }
                }

                if(specialJumpFrame == false)
                {
                    // Maintain y velocity.
                    moveDelta.y = currentVelocity.y * Time.deltaTime;
                    if (isGliding)
                    {
                        if (jumpButton.isPressed) // Held or on button down
                        {
                            // GLIDING MOVEMENT
                            currentGravityY = glideGravity;
                            
                            // Force moving forward.
                            moveDir = characterRoot.forward;
                            Vector3 targetDir = characterRoot.right * ((moveInput.x < 0)? -1 : 1);
                            moveDir = Vector3.RotateTowards(moveDir, targetDir, Mathf.Deg2Rad * glideTurnSpeed * Time.deltaTime * Mathf.Abs(moveInput.x), 0f);

                            // Rotate the crow model while gliding!
                            float rotLimit = 50f;
                            float rotSpeed = 120f;
                            rotationRoot.localRotation = Quaternion.RotateTowards(
                                rotationRoot.localRotation, Quaternion.Euler(0f, 0f, -moveInput.x * rotLimit),
                                rotSpeed * Time.deltaTime);
                            
                            characterRoot.LookAt(characterRoot.position + moveDir.normalized, Vector3.up);
                            
                            moveDelta += characterRoot.forward * (glideMoveSpeed * Time.deltaTime);
                        }
                        else
                        {
                            // STOP GLIDE
                            isGliding = false; // Fall instead.
                            animator.SetBool("Gliding", false);
                            rotationRoot.localRotation = Quaternion.identity;
                        }
                    }
                    
                    if (isGliding == false)
                    {
                        // FALLING MOVEMENT
                        currentGravityY = defaultGravity;
                        moveDelta += moveDir * (groundedMoveSpeed * Time.deltaTime); // ToDo: Different speed for falling?
                        RotateTowardsDirection(moveDir, groundedTurnSpeed);
                    }
                }
            }
            
            animator.SetInteger("hop", (moveInput.magnitude > 0.01f && moveController.isGrounded)? 1 : 0);
            
            wasGrounded = moveController.isGrounded;
            moveDelta.y += currentGravityY * Time.deltaTime * Time.deltaTime;
            moveController.Move(moveDelta);
        }


        public void OnCaw()
        {
            animationAudioController.PlayCaw(); 
        }

        private void RotateTowardsDirection(Vector3 moveDirection, float argRotationSpeed = 0f)
        {
            // ToDo: Smooth this Rotation.
            Vector3 forward = Vector3.RotateTowards(characterRoot.transform.forward, moveDirection.normalized, argRotationSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
            characterRoot.LookAt(characterRoot.transform.position + forward.normalized, Vector3.up);
            //characterRoot.LookAt(Vector3.Lerp(characterRoot.transform.position, characterRoot.transform.position + moveDirection.normalized, rotateSpeed));
            // characterRoot.rotation = Quaternion.RotateTowards(characterRoot.transform.rotation, Quaternion.LookRotation(characterRoot.transform.position + moveDirection.normalized, Vector3.up), 
            //     Time.deltaTime * argRotationSpeed);
        }

        private void HandleGroundedMovement(Vector2 moveInput, ref Vector3 moveDelta)
        {
            float slopeAngle = 0f;
            if (currentControllerHit != null)
            {
                // ToDo: Reenable this later.
                //slopeAngle = Vector3.Angle(Vector3.up, currentControllerHit.normal);
            }
            
            if (slopeAngle > moveController.slopeLimit)
            {
                // Slide down slope (Not used)
                // Vector3 g = Vector3.Cross(Vector3.Cross(currentControllerHit.normal, Vector3.down), currentControllerHit.normal);
                // g *= groundedMoveSpeed * Time.deltaTime;
                // Debug.DrawLine(currentControllerHit.point + currentControllerHit.normal, currentControllerHit.point + currentControllerHit.normal + (g * 2f), Color.magenta, 0.1f);
            }
            else 
            {
                // NORMAL GROUNDED MOVEMENT
                Vector3 moveDir = GetMoveDirectionXZ(moveInput);
                moveDelta += moveDir * (groundedMoveSpeed * Time.deltaTime);
                
                RotateTowardsDirection(moveDir, groundedTurnSpeed);
            }

            //moveDelta.y += defaultGravity * Time.deltaTime * Time.deltaTime;
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
            currentControllerHit = hit;
            
            animationAudioController = animator.GetComponent<CrowAnimationAudio>();
            if (animationAudioController)
            {
                animationAudioController.HandleControllerColliderHit(moveController, hit);
            }
        }

#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            if (EditorApplication.isPlaying && moveController != null)
            {
                Vector3 textPos = transform.position + new Vector3(0f, moveController.height + 0.2f, 0f);
                Color prevColor = Handles.color;
                Handles.color = (moveController.isGrounded) ? Color.cyan : new Color(1f, 0.5f, 0.2f);
                Handles.Label(textPos, string.Format("V:{0}\nG:{1}", moveController.velocity, moveController.isGrounded));
                Handles.color = prevColor;
            }
        }
#endif
    }
}
