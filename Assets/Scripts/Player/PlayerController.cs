using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("PlayerController")] 
    [HideInInspector] public bool Enabled = true;
    [SerializeField, Range(1, 10)] float walkingSpeed = 3.0f;
    [Range(0.1f, 5)] public float CrouchSpeed = 1.0f;
    public float proneSpeed;
    public float staminaDrainCrouch;
    public float speedToCrouch;
    public float speedToStandUp;
    [SerializeField, Range(2, 20)] float RunningSpeed = 4.0f;
    [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
    public float staminaDrainJump;
    [SerializeField, Range(0.5f, 10)] float lookSpeed = 2.0f;
    [SerializeField, Range(10, 120)] float lookXDownLimit = 80.0f;
    [SerializeField, Range(10, 120)] float lookXUpLimit = 80.0f;
    [Space(20)]
    [Header("Advanced")]
    [SerializeField] float RunningFOV;
    [SerializeField] float SpeedToFOV = 4.0f;
    [SerializeField] float CrouchHeight = 1.0f;
    public bool isGrounded;
    [SerializeField] float timeToRunning = 2.0f;
    private float InstallGoingProneTime;
    [SerializeField] private float goingProneTime;
    [SerializeField] private float layingDownTime;

    [HideInInspector] public Rigidbody characterController;
    [HideInInspector] public CapsuleCollider collider;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public bool isCrouching = false;
    float InstallCrouchHeight;
    float rotationX = 0;
    [HideInInspector] public bool isRunning = false;
    float InstallFOV;
    public Camera cam;
    public GameObject camHolder;
    [HideInInspector] public bool Moving;
    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float Lookvertical;
    [HideInInspector] public float Lookhorizontal;
    float RunningValue;
    bool WallDistance;
    [HideInInspector] public float WalkingValue;

    private Player player;
    private bool isDrainingStaminaRunning;
    private float prevSpeedToStandUp;
    private Controls controls;
    public bool isProne = false;
    
    private float xRotation, yRotation;
    public Transform playerHead;
    private float InstallLookSpeed;

    void Start()
    {
        player = Player.Instance;
        characterController = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        collider = GetComponent<CapsuleCollider>();
        InstallCrouchHeight = collider.height;
        InstallFOV = cam.fieldOfView;
        InstallLookSpeed = lookSpeed;
        InstallGoingProneTime = goingProneTime;
        WalkingValue = walkingSpeed;
        RunningValue = WalkingValue;
        prevSpeedToStandUp = speedToStandUp;
        Enabled = true;
        controls = Player.Instance.controls;
    }

    private void LateUpdate()
    {
        MouseMoving();
    }

    void Update()
    {
        Running();

        Vector3 forward = playerHead.TransformDirection(Vector3.forward);
        Vector3 right = playerHead.TransformDirection(Vector3.right);
        vertical = Enabled ? (isProne ? proneSpeed : (isRunning ? RunningValue : WalkingValue)) * Input.GetAxis("Vertical") : 0;
        horizontal = Enabled ? (isProne ? proneSpeed : (isRunning ? RunningValue : WalkingValue)) * Input.GetAxis("Horizontal") : 0;

        moveDirection = (forward * vertical) + (right * horizontal);

        if (player.Stamina >= player.minStaminaForAction && controls.Land.Jump.triggered && Enabled && isGrounded && !isProne)
        {
            characterController.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            player.Stamina -= staminaDrainJump;
            if (isCrouching) Player.Instance.Stamina -= staminaDrainCrouch;
            isCrouching = false;
        }

        Moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;

        Crouching();

        if (controls.Land.Prone.triggered && Enabled && isGrounded && Player.Instance.Stamina >= staminaDrainCrouch)
        {
            StopCoroutine(nameof(GoProne));
            isProne = !isProne;
            if (isProne)
            {
                lookXDownLimit = 27f;
                lookXUpLimit = 20f;
                lookSpeed = 0.5f;
                isCrouching = false;
                if (isRunning)
                {
                    Enabled = false;
                    goingProneTime *= 2f;
                    isRunning = false;
                    characterController.velocity = new Vector3(moveDirection.x / 2f, 2f, moveDirection.z / 2f);
                    layingDownTime = 6f;
                    Invoke(nameof(AbleToProne), goingProneTime);
                }
                playerHead.localRotation = Quaternion.Euler(-90, 0, 0);
                Player.Instance.Stamina -= staminaDrainCrouch;
            }
            else
            {
                if (!Physics.Raycast(transform.position, Vector3.up, 0.8f))
                {
                    lookXUpLimit = 80f;
                    lookXDownLimit = 80f;
                    playerHead.localRotation = Quaternion.Euler(0, 0, 0);
                    Player.Instance.Stamina -= staminaDrainCrouch;
                    goingProneTime = InstallGoingProneTime;
                    AbleToProne();
                }
                else isProne = true;

            }
            StartCoroutine(nameof(GoProne));
        }
    }
    
    private void FixedUpdate()
    {
        if (Enabled)
            characterController.velocity = new Vector3(moveDirection.x, characterController.velocity.y, moveDirection.z);
    }

    private IEnumerator GoProne()
    {
        while (isProne)
        {
            transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.eulerAngles.x, 90, layingDownTime * Time.deltaTime), transform.eulerAngles.y, transform.eulerAngles.z);
            yield return null;
        }

        while (!isProne)
        {
            transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.eulerAngles.x, 0, layingDownTime / 2f * Time.deltaTime), transform.eulerAngles.y, transform.eulerAngles.z);
            yield return null;
        }
    }

    private void AbleToProne()
    {
        Enabled = true;
        lookSpeed = InstallLookSpeed;
        layingDownTime = 3f;
    }

    private void Crouching()
    {
        RaycastHit CrouchCheck;
        
        if (controls.Land.Crouch.triggered && isGrounded)
        {
            if (Enabled && player.Stamina >= player.minStaminaForAction && !Physics.Raycast(transform.position, Vector3.up, CrouchHeight / 2f))
            {
                StopCoroutine(nameof(DoCrouch));
                isCrouching = !isCrouching;
                if (isCrouching)
                {
                    player.Stamina -= staminaDrainCrouch;
                    speedToStandUp = prevSpeedToStandUp;
                    if (isRunning)
                    {
                        Enabled = false;
                        Invoke(nameof(AbleToProne), 0.2f);
                    }
                    isRunning = false;
                    if (isProne)
                    {
                        isProne = false;
                        playerHead.localRotation = Quaternion.Euler(0, 0, 0);
                        Enabled = false;
                        Invoke(nameof(AbleToProne), goingProneTime / 2f);
                        lookXUpLimit = 80f;
                        lookXDownLimit = 80f;
                    }
                }
                else
                {
                    if (player.Stamina < player.minStaminaForAction)
                    {
                        prevSpeedToStandUp = speedToStandUp;
                        speedToStandUp /= 2f;
                    }
                    player.Stamina -= staminaDrainCrouch / 2;
                }
                StartCoroutine(nameof(DoCrouch));
            }
        }
    }

    private IEnumerator DoCrouch()
    {
        while (isCrouching)
        {
            float Height = Mathf.Lerp(collider.height, CrouchHeight, speedToCrouch * Time.deltaTime);
            collider.height = Height;
            WalkingValue = Mathf.Lerp(WalkingValue, CrouchSpeed, 6 * Time.deltaTime);
            yield return null;
        }
        
        while (!isCrouching)
        {
            float Height = Mathf.Lerp(collider.height, InstallCrouchHeight, speedToStandUp * Time.deltaTime);
            collider.height = Height;
            WalkingValue = Mathf.Lerp(WalkingValue, walkingSpeed, 4 * Time.deltaTime);
            yield return null;
        }

        speedToStandUp = prevSpeedToStandUp;
    }

    private void Running()
    {
        if (controls.Land.Run.triggered && !isProne)
        {
            if ((player.Stamina >= player.minStaminaForAction && !isCrouching && Enabled) ||
                (isRunning && player.Stamina < player.minStaminaForAction))
            {
                isRunning = !isRunning;
                StartCoroutine(nameof(DoRunning));
                if (!isDrainingStaminaRunning) StartCoroutine(nameof(DrainStaminaRunning));
            }
        }
    }

    private IEnumerator DoRunning()
    {
        while (isRunning)
        {
            RunningValue = Mathf.Lerp(RunningValue, RunningSpeed, timeToRunning * Time.deltaTime);
            if (controls.Land.Move.ReadValue<Vector2>().y == 0 && controls.Land.Move.ReadValue<Vector2>().x == 0)
            {
                isRunning = false;
            }
            yield return null;
        }
        RunningValue = WalkingValue;
    }
    

    private IEnumerator DrainStaminaRunning()
    {
        isDrainingStaminaRunning = true;
        while (isRunning)
        {
            if (Moving)
            {
                player.Stamina -= 0.05f;
                isDrainingStaminaRunning = true;
            }
            else isRunning = false;
            yield return new WaitForSeconds(0.2f);
        }

        isDrainingStaminaRunning = false;
    }

    private void MouseMoving()
    {
        if (Cursor.lockState == CursorLockMode.Locked && Enabled)
        {
            Lookvertical = -controls.Land.MouseY.ReadValue<float>() * lookSpeed / 10f;
            Lookhorizontal = controls.Land.MouseX.ReadValue<float>() * lookSpeed / 10f;

            yRotation += Lookhorizontal;

            xRotation -= Lookvertical;
            xRotation = Mathf.Clamp(xRotation, -lookXDownLimit, lookXUpLimit);

            // rotate cam and orientation
            camHolder.transform.rotation = Quaternion.Euler(-xRotation, yRotation, transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);

            if (isRunning && Moving) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, RunningFOV, SpeedToFOV * Time.deltaTime);
            else cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, InstallFOV, SpeedToFOV * Time.deltaTime);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        isGrounded = true;
    }
    
    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }
}