using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("PlayerController")]
    [SerializeField] public Transform Camera;
    [SerializeField, Range(1, 10)] float walkingSpeed = 3.0f;
    [Range(0.1f, 5)] public float CrouchSpeed = 1.0f;
    public float staminaDrainCrouch;
    public float speedToCrouch;
    public float speedToStandUp;
    [SerializeField, Range(2, 20)] float RunningSpeed = 4.0f;
    [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
    public float staminaDrainJump;
    [SerializeField, Range(0.5f, 10)] float lookSpeed = 2.0f;
    [SerializeField, Range(10, 120)] float lookXLimit = 80.0f;
    [Space(20)]
    [Header("Advanced")]
    [SerializeField] float RunningFOV;
    [SerializeField] float SpeedToFOV = 4.0f;
    [SerializeField] float CrouchHeight = 1.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float timeToRunning = 2.0f;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool CanRun = true;

    [Space(20)]
    [Header("Input")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    bool isCrouching = false;
    float InstallCrouchHeight;
    float rotationX = 0;
    public bool isRunning = false;
    float InstallFOV;
    Camera cam;
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
    void Start()
    {
        player = Player.Instance;
        characterController = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InstallCrouchHeight = characterController.height;
        InstallFOV = cam.fieldOfView;
        WalkingValue = walkingSpeed;
        RunningValue = WalkingValue;
        prevSpeedToStandUp = speedToStandUp;
    }

    void Update()
    {
        MouseMoving();
        Running();

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        vertical = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Vertical") : 0;
        horizontal = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * vertical) + (right * horizontal);

        if (player.Stamina >= player.minStaminaForAction && Input.GetKey(jumpKey) && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
            player.Stamina -= staminaDrainJump;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        
        characterController.Move(moveDirection * Time.deltaTime);
        Moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;
        
        Crouching();
    }

    private void Crouching()
    {
        RaycastHit CrouchCheck;
        
        if (Input.GetKeyDown(crouchKey))
        {
            if (player.Stamina >= player.minStaminaForAction ||
                (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position,
                    transform.TransformDirection(Vector3.up), out CrouchCheck, 0.8f, 1) && isCrouching))
            {
                StopCoroutine(nameof(DoCrouch));
                isCrouching = !isCrouching;
                if (isCrouching)
                {
                    player.Stamina -= staminaDrainCrouch;
                    speedToStandUp = prevSpeedToStandUp;
                    isRunning = false;
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
            float Height = Mathf.Lerp(characterController.height, CrouchHeight, speedToCrouch * Time.deltaTime);
            characterController.height = Height;
            WalkingValue = Mathf.Lerp(WalkingValue, CrouchSpeed, 6 * Time.deltaTime);
            yield return null;
        }
        
        while (!isCrouching)
        {
            float Height = Mathf.Lerp(characterController.height, InstallCrouchHeight, speedToStandUp * Time.deltaTime);
            characterController.height = Height;
            WalkingValue = Mathf.Lerp(WalkingValue, walkingSpeed, 4 * Time.deltaTime);
            yield return null;
        }

        speedToStandUp = prevSpeedToStandUp;
    }

    private void Running()
    {
        if (Input.GetKeyDown(runKey))
        {
            if ((player.Stamina >= player.minStaminaForAction && !isCrouching && CanRun) ||
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
        if (Cursor.lockState == CursorLockMode.Locked && canMove)
        {
            Lookvertical = -Input.GetAxis("Mouse Y");
            Lookhorizontal = Input.GetAxis("Mouse X");

            rotationX += Lookvertical * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            Camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Lookhorizontal * lookSpeed, 0);

            if (isRunning && Moving) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, RunningFOV, SpeedToFOV * Time.deltaTime);
            else cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, InstallFOV, SpeedToFOV * Time.deltaTime);
        }
    }

}