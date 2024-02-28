using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsSmooth : MonoBehaviour
{
    [Header("HandsSmooth")]
    [SerializeField] CharacterController controller;
    [SerializeField, Range(1, 10)] float smooth = 4f;
    [SerializeField, Range(0.001f, 1)] float amount = 0.03f;
    [SerializeField, Range(0.001f, 1)] float maxAmount = 0.04f;
    [Header("Rotation")]
    [SerializeField, Range(1, 10)] float RotationSmooth = 4.0f;
    [SerializeField, Range(0.1f, 10)] float RotationAmount = 1.0f;
    [SerializeField, Range(0.1f, 10)] float MaxRotationAmount = 5.0f;
    [SerializeField, Range(0.1f, 10)] float RotationMovementMultipler = 1.0f;

    [Header("CrouchRotation")]
    [SerializeField, Range(0.1f, 20)] float RotationCrouchSmooth = 15.0f;
    [SerializeField, Range(5f, 50)] float RotationCrouchMultipler = 18.0f;

    float CrouchRotation;
    Vector3 InstallPosition;
    Quaternion InstallRotation;
    private PlayerController player;
    private bool crouching;

    private void Start()
    {
        controller = GetComponentInParent<CharacterController>();
        player = Player.Instance.GetComponent<PlayerController>();
        InstallPosition = transform.localPosition;
        InstallRotation = transform.localRotation;
    }
    private void Update()
    {

        float InputX = -Player.Instance.controls.Land.MouseX.ReadValue<float>();
        float InputY = -Player.Instance.controls.Land.MouseY.ReadValue<float>();
        float horizontal = -Player.Instance.controls.Land.Move.ReadValue<Vector2>().x;
        float vertical = Player.Instance.controls.Land.Move.ReadValue<Vector2>().y;

        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY + -controller.velocity.y / 60, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + InstallPosition, Time.deltaTime * smooth);
        
        float TiltX = Mathf.Clamp(InputX * RotationAmount, -MaxRotationAmount, MaxRotationAmount);
        float TiltY = Mathf.Clamp(InputY * RotationSmooth, -MaxRotationAmount, MaxRotationAmount);
        if (!crouching && player.isCrouching)
        {
            StartCoroutine(nameof(DoCrouching));
            crouching = true;
        }
        Vector3 vector = new Vector3(Mathf.Max(vertical * 0.4f, 0) * RotationMovementMultipler, 0, horizontal * RotationMovementMultipler);
        Vector3 finalRotation = new Vector3(-TiltY, 0, TiltX + CrouchRotation) + vector;
        
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(finalRotation) * InstallRotation, Time.deltaTime * RotationSmooth); 
    }

    private IEnumerator DoCrouching()
    {
        while (player.isCrouching)
        {
            CrouchRotation = Mathf.Lerp(CrouchRotation, RotationCrouchMultipler, RotationCrouchSmooth * Time.deltaTime);
            yield return null;
        }

        while (!player.isCrouching)
        {
            CrouchRotation = Mathf.Lerp(CrouchRotation, 0f, RotationCrouchSmooth * Time.deltaTime);
            yield return null;
        }

        crouching = false;
    }
}