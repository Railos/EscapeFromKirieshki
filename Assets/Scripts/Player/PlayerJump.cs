using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{

    [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
    [SerializeField] private float timeBeforeJump;
    public float staminaDrainJump;
    
    public bool canJump;
    private Player player;
    private PlayerController controller;

    private Controls controls;
    
    private void Start()
    {
        player = GetComponent<Player>();
        controller = GetComponent<PlayerController>();
    }

    public void Jump()
    {
        canJump = false;
        controller.characterController.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        player.Stamina -= staminaDrainJump;
        if (controller.isCrouching) Player.Instance.Stamina -= controller.staminaDrainCrouch;
        controller.isCrouching = false;
        Invoke(nameof(WaitForJump), timeBeforeJump);
    }
    
    private void WaitForJump()
    {
        canJump = true;
    }
}
