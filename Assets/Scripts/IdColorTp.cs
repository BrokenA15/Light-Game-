using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdColorTp : MonoBehaviour
{

    public int colorId = 0;
    [SerializeField] private float requiredChargeTime = 2f; 
    private float chargeTimer = 0f;

    [SerializeField] private LightOn playerLightOn; 
    private bool isCharging = false;
    public bool isReady = false;

    private Material lampMat;
    [SerializeField]
    private Material boxMat;
    private Color currentColor;

    [SerializeField]
    private Rigidbody lampBody;
    [SerializeField]
    private float recoilForce = 5f;
    [SerializeField]
    private float recoilTorque = 3f;

    [SerializeField] 
    private Transform hingePivot;
    [SerializeField] private float rotationSmoothTime = 1f;
    private bool rotationLocked = false;
    private Quaternion lockedRotation;

    [SerializeField]
    private ParticleSystem tpParticles;


    private void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        
        lampMat = new Material(boxMat);
        rend.material = lampMat;
        currentColor = lampMat.color;
        lampMat.SetColor("_BaseColor", Color.black);

        if(lampBody == null )
        lampBody = GetComponent<Rigidbody>();

    }


    public void SetColor(Color newColor)
    {
        if (lampMat != null)
        {
            lampMat.color = newColor;
            lampMat.SetColor("_EmissionColor", newColor * 10f); 
        }
    }

    public void OnMatchedColorHit()
    {
        if (isReady) return; 

        isCharging = true;
        chargeTimer += Time.deltaTime;
        if (chargeTimer >= requiredChargeTime)
        {
            isReady = true;
            OnFullyCharged();
        }
    }

    public void OnRayExit()
    {
        if (!isReady)
        {
            isCharging = false;
            chargeTimer = 0f;
            lampMat.SetColor("_EmissionColor", currentColor * 1f);
        }
    }

    private void OnFullyCharged()
    {
        Debug.Log($" [Lampara {name}] Color correcto cargado — Listo para TP.");
        lampMat.SetColor("_EmissionColor", currentColor * 10f);
        if (hingePivot != null)
        {
            lockedRotation = hingePivot.rotation;
            rotationLocked = true;
        }
    }

    public void TeleportHere(Transform player)
    {
        if (!isReady) return;

        Vector3 fromDirection = (player.position - transform.position).normalized;

        player.SetParent(transform);
        player.localPosition = Vector3.zero;

        ApplyRecoil(fromDirection);

        if (hingePivot != null)
            lockedRotation = hingePivot.rotation;

        rotationLocked = true;

        if(tpParticles  != null)
        {
            tpParticles.Play();
        }

        Debug.Log($"[TP] Jugador teletransportado a {name}");
    }

    public void ApplyRecoil(Vector3 fromDirection)
    {
        if (lampBody == null) return;
        


        lampBody.AddForce(fromDirection.normalized * recoilForce, ForceMode.Impulse);

        Vector3 randomTorque = new Vector3(
            UnityEngine.Random.Range(-1f,1f),
            UnityEngine.Random.Range(-0.5f, 0.5f),
            UnityEngine.Random.Range(-1f, 1f)
        ) * recoilTorque;

        lampBody.AddTorque(randomTorque, ForceMode.Impulse);

    }

    void Update()
    {
        if (!rotationLocked && hingePivot != null && playerLightOn != null)
        {
            Vector3 direction = playerLightOn.transform.position - hingePivot.position;
            direction.y = 0f; // solo eje horizontal
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                hingePivot.rotation = Quaternion.Slerp(hingePivot.rotation, targetRot, Time.deltaTime / rotationSmoothTime);
            }
        }
        else if (rotationLocked && hingePivot != null)
        {
            // mantener rotación bloqueada
            hingePivot.rotation = lockedRotation;
        }

        if (isCharging && !isReady)
        {
            float t = Mathf.InverseLerp(0f, requiredChargeTime, chargeTimer);   
            lampMat.SetColor("_EmissionColor", currentColor * Mathf.Lerp(1f, 10f, t));
        }
    }
}
