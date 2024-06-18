using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{

    [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
    [SerializeField] private float timeBeforeJump;
    public float staminaDrainJump;
    public float howLongPressButton;
    
    public bool canJump;
    private Player player;
    private PlayerController controller;

    private Controls controls;
    private float timePassed;
    private bool readyToJump;
    
    private void Start()
    {
        player = GetComponent<Player>();
        controller = GetComponent<PlayerController>();
        timePassed = howLongPressButton;
    }

    private void Update()
    {
        CheckForButtonPress();
        JumpCheckings();
    }
    
    public void JumpCheckings()
    {
        if (player.controls.Land.Jump.WasReleasedThisFrame())
        {
            if (!readyToJump)
            {
                timePassed = howLongPressButton;
                return;
            }
            if (GetComponent<PlayerVaulting>().isVaulting) return;
            if (!canJump) return;
            if (player.Stamina < player.minStaminaForAction) return;
            if (!controller.Enabled) return;
            if (!controller.isGrounded) return;
            if (controller.isProne) return;
            Jump();
        }
    }
    
    private void CheckForButtonPress()
    {
        if (!canJump) return;
        if (player.controls.Land.Jump.inProgress)
        {
            timePassed -= Time.deltaTime;
            if (timePassed > 0f)
            {
                readyToJump = true;
            }else
            {
                readyToJump = false;
            }
        }
    }
    
    public void Jump()
    {
        canJump = false;
        controller.characterController.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        player.Stamina -= staminaDrainJump;
        if (controller.isCrouching) Player.Instance.Stamina -= controller.staminaDrainCrouch;
        controller.isCrouching = false;
        timePassed = howLongPressButton;
        Invoke(nameof(WaitForJump), timeBeforeJump);
    }
    
    private void WaitForJump()
    {
        canJump = true;
    }
}
