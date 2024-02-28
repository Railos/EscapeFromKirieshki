using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;

    private void Update()
    {
        // get mouse input
        float mouseX = Player.Instance.controls.Land.MouseX.ReadValue<float>() * multiplier;
        float mouseY = Player.Instance.controls.Land.MouseY.ReadValue<float>() * multiplier;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseX, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseY, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
