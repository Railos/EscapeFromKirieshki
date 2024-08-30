using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject camHolder;
    public float InstallFOV;
    [HideInInspector] public float Lookvertical;
    [HideInInspector] public float Lookhorizontal;
    [SerializeField] float RunningFOV;
    [SerializeField] float SpeedToFOV = 4.0f;
    [SerializeField, Range(10, 120)] public float lookXDownLimit = 80.0f;
    [SerializeField, Range(10, 120)] public float lookXUpLimit = 80.0f;
    [SerializeField, Range(1f, 10)] public float lookSpeed = 2.0f;
    private float xRotation, yRotation;
    public float InstallLookSpeed;

    private PlayerController controller;

    private void Start()
    {
        InstallLookSpeed = lookSpeed;
        controller = GetComponent<PlayerController>();
        InstallFOV = controller.cam.fieldOfView;
    }
    
    private void LateUpdate()
    {
        MouseMoving();
    }
    
    private void MouseMoving()
    {
        Lookvertical = -controller.controls.Land.MouseY.ReadValue<float>() * lookSpeed * Time.deltaTime * 10f;
        Lookhorizontal = controller.controls.Land.MouseX.ReadValue<float>() * lookSpeed * Time.deltaTime * 10f;

        yRotation += Lookhorizontal;

        xRotation -= Lookvertical;
        xRotation = Mathf.Clamp(xRotation, -lookXDownLimit, lookXUpLimit);

        // rotate cam and orientation
        camHolder.transform.rotation = Quaternion.Euler(-xRotation, yRotation, transform.eulerAngles.z);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);

        if (controller.isRunning && controller.Moving) controller.cam.fieldOfView = Mathf.Lerp(controller.cam.fieldOfView, RunningFOV, SpeedToFOV * Time.deltaTime);
        else controller.cam.fieldOfView = Mathf.Lerp(controller.cam.fieldOfView, InstallFOV, SpeedToFOV * Time.deltaTime);
    }
}
