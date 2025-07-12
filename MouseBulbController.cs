using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBulbController : MonoBehaviour
{
    [Header("Mouse Look At")]
    public Camera mainCamera;
    public float maxDistance = 100f;
    public LayerMask hitLayers; // Optional: limit what gets hit

    [Header("Flicker Settings")]
    public Light bulbLight;             // Drag your Light component here
    public float minIntensity = 0.6f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 15f;

    [Header("Audio")]
    public AudioSource buzzAudio; // Drag your AudioSource here (optional if attached)

    void Start()
    {
        // Auto-get AudioSource if not assigned
        if (buzzAudio == null)
        {
            buzzAudio = GetComponent<AudioSource>();
        }

        if (buzzAudio != null && !buzzAudio.isPlaying)
        {
            buzzAudio.loop = true;
            buzzAudio.Play();
        }
    }

    void Update()
    {
        // --- Mouse Look Logic ---
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
        {
            Vector3 direction = hit.point - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // --- Flicker Logic ---
        if (bulbLight != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
            bulbLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }
}
