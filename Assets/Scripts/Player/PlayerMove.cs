using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float playerHeight;
    public float moveSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float crouchSpeed;
    public float airMultiplier;
    public float jumpForce;
    public float groundDrag;
   
    private Rigidbody controller;
    
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    public bool exitingSlope;

    public KeyCode runKey;
    public KeyCode crouchKey;

    private bool isGrounded;

    private float horizontalMove;
    private float verticalMove;
    private Vector3 moveDirection;
    private Vector3 vel;

    private void Start()
    {
        controller = GetComponent<Rigidbody>();
        vel = controller.velocity;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);
        StateHandler();
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space)) controller.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        if (isGrounded) controller.drag = groundDrag;
        else controller.drag = 0f;
        
        SpeedLimiting();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (OnSlope() && !exitingSlope)
        {
            controller.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (controller.velocity.y > 0)
                controller.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (isGrounded)
        {
            controller.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
            controller.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        controller.useGravity = !OnSlope();
    }
    
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void SpeedLimiting()
    {
        Vector3 flatVel = new Vector3(vel.x, 0f, vel.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            vel = new Vector3(limitedVel.x, vel.y, limitedVel.z);
        }
    }

    private void CalculateMovement()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        moveDirection = transform.forward * verticalMove + transform.right * horizontalMove;
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey)) moveSpeed = crouchSpeed;
        else if (Input.GetKey(runKey)) moveSpeed = runSpeed;
        else moveSpeed = walkSpeed;
    }
}
