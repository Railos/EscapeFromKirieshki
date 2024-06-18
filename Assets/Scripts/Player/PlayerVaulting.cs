using System;
using System.Collections;
using UnityEngine;

public class PlayerVaulting : MonoBehaviour
{
    public Transform lowestPos;
    public Transform highVaultPos;
    public Transform lowVaultPos;

    private Player player;
    private PlayerController controller;
    private Vector3 vaultableSpot;
    public bool isVaulting = false;

    private Vector3 initialPos;
    public float lowVaultSpeed;
    public float highVaultSpeed;
    public float overObjectVaultSpeed;
    public float vaultResetTime;
    private bool canVault = true;
    private float vaultSpeed;
    private bool performedVault;
    private float width;
    
    private void Start()
    {
        player = GetComponent<Player>();
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player.controls.Land.Jump.triggered && controller.isGrounded && !controller.isProne)
        {
            if (Physics.Raycast(lowestPos.position, transform.forward, 0.25f))
            {
                Debug.Log("raycast hit");
                if (controller.vertical > 0f)
                {
                    Debug.Log("WHAT");
                    if (!Physics.Raycast(highVaultPos.position, transform.forward, 0.25f) &&
                        Physics.Raycast(lowVaultPos.position, transform.forward, 0.25f))
                    {
                        Debug.Log("High Vault");
                        vaultSpeed = highVaultSpeed;
                        StartCoroutine(FindLedgeCLimbSpot());
                    }else if (!Physics.Raycast(highVaultPos.position, transform.forward, 0.25f) &&
                              !Physics.Raycast(lowVaultPos.position, transform.forward, 0.25f))
                    {
                        Debug.Log("Low Vault"); 
                        vaultSpeed = lowVaultSpeed;
                        if (CheckIfVault())
                        {
                            StartCoroutine(nameof(PerformVault));
                        }
                        else
                        {
                            StartCoroutine(FindLedgeCLimbSpot());
                        }
                    }

                    canVault = false;
                    StartCoroutine(nameof(ResetVault));
                    performedVault = false;
                }
            }
        }

    }

    private IEnumerator ResetVault()
    {
        yield return new WaitForSeconds(vaultResetTime);
        canVault = true;
    }

    private bool CheckIfVault()
    {
        RaycastHit _hit;
        initialPos = Vector3.zero;
        if (Physics.Raycast(lowestPos.position, transform.forward, out _hit)) initialPos = _hit.point;
        initialPos = new Vector3(initialPos.x, initialPos.y - 0.1f, initialPos.z);
        while (Physics.Raycast(new Vector3(initialPos.x, initialPos.y + 0.1f, initialPos.z) - (transform.forward * 0.2f), transform.forward, 0.5f))
        {
            initialPos = new Vector3(initialPos.x, initialPos.y + 0.1f, initialPos.z);
        }
        initialPos = new Vector3(initialPos.x, initialPos.y + 1f, initialPos.z);
        width = 0f;
        while (Physics.Raycast(new Vector3(initialPos.x, initialPos.y, initialPos.z) + transform.forward * 0.01f, -transform.up, 1.1f))
        {
            width += 1f;
            initialPos += transform.forward * 0.01f;
        }
        Debug.Log(width);
        Debug.DrawLine(new Vector3(initialPos.x, initialPos.y + 0.01f, initialPos.z), new Vector3(initialPos.x, initialPos.y + 0.15f, initialPos.z) + transform.forward * width * 0.01f + transform.forward * 1.25f, Color.red, 5f);
        if (width <= 33f && !Physics.Raycast(new Vector3(initialPos.x, initialPos.y + 0.15f, initialPos.z), transform.forward, width * 0.01f + 1.25f)
        && !Physics.Raycast(new Vector3(initialPos.x, initialPos.y + 0.01f, initialPos.z) + -transform.right * 0.1f, transform.forward, width * 0.01f + 1.25f)
        && !Physics.Raycast(new Vector3(initialPos.x, initialPos.y + 0.01f, initialPos.z) + transform.right * 0.1f, transform.forward, width * 0.01f + 1.25f)) return true;
        return false;
    }
    
    IEnumerator FindLedgeCLimbSpot()
    {
        RaycastHit _hit;
        initialPos = Vector3.zero;
        if (Physics.Raycast(lowestPos.position, transform.forward, out _hit)) initialPos = _hit.point;
        initialPos = new Vector3(initialPos.x, initialPos.y - 0.1f, initialPos.z);
        while (Physics.Raycast(new Vector3(initialPos.x, initialPos.y + 0.1f, initialPos.z) - (transform.forward * 0.2f), transform.forward, 0.5f))
        {
            initialPos = new Vector3(initialPos.x, initialPos.y + 0.1f, initialPos.z);
            yield return null;
        }

        initialPos += transform.forward * 0.1f;
        vaultableSpot = new Vector3(initialPos.x, initialPos.y + 1.1f, initialPos.z);
        StartCoroutine(PerformLedgeClimb(vaultableSpot));
    }
    

    IEnumerator PerformLedgeClimb(Vector3 vaultableSpot)
    {
        isVaulting = true;
        GetComponent<PlayerController>().Enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        while (Vector3.Distance(transform.position, vaultableSpot) > 0.15f)
        {
            transform.position = Vector3.Lerp(transform.position, vaultableSpot, vaultSpeed * Time.deltaTime);
            yield return null;
        }
        isVaulting = false;
        performedVault = true;
        GetComponent<PlayerController>().Enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    IEnumerator PerformVault()
    {
        isVaulting = true;
        GetComponent<PlayerController>().Enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        Vector3 vaultableSpot = transform.position + transform.forward * width * 0.01f + transform.forward * 1.25f;
        while (Vector3.Distance(transform.position, vaultableSpot) > 0.15f)
        {
            transform.position = Vector3.Lerp(transform.position, vaultableSpot, overObjectVaultSpeed * Time.deltaTime);
            yield return null;
        }
        isVaulting = false;
        performedVault = true;
        GetComponent<PlayerController>().Enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
