using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Light Settings")]
    public Light lightToFlicker;           // drag the light component here
    public AudioSource flickerSound;      // optional flicker sound
    
    [Header("Flicker Settings")]
    public float baseIntensity = 1f;       // normal light intensity
    public float flickerIntensity = 0.3f;  // intensity during flicker
    public float minFlickerTime = 0.1f;    // minimum time between flickers
    public float maxFlickerTime = 3f;      // maximum time between flickers
    public float flickerDuration = 0.1f;   // how long each flicker lasts
    
    [Header("Advanced Options")]
    public bool randomStart = true;        // start flickering at random time
    public bool colorFlicker = false;      // also flicker the color slightly
    public Color baseColor = Color.white;  // normal light color
    public Color flickerColor = Color.yellow; // color during flicker

    private Coroutine flickerCoroutine;
    private bool isFlickering = false;

    void Start()
    {
        // Get light component if not assigned
        if (lightToFlicker == null)
            lightToFlicker = GetComponent<Light>();
        
        // Set initial values
        if (lightToFlicker != null)
        {
            baseIntensity = lightToFlicker.intensity;
            baseColor = lightToFlicker.color;
        }
        
        // Start flickering
        StartFlickering();
    }

    public void StartFlickering()
    {
        if (flickerCoroutine != null)
            StopCoroutine(flickerCoroutine);
        
        flickerCoroutine = StartCoroutine(FlickerLoop());
    }

    public void StopFlickering()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
        
        // Reset light to normal
        if (lightToFlicker != null)
        {
            lightToFlicker.intensity = baseIntensity;
            lightToFlicker.color = baseColor;
        }
        
        isFlickering = false;
    }

    IEnumerator FlickerLoop()
    {
        // Random initial delay if enabled
        if (randomStart)
            yield return new WaitForSeconds(Random.Range(0f, maxFlickerTime));

        while (true)
        {
            // Wait for random time before next flicker
            float waitTime = Random.Range(minFlickerTime, maxFlickerTime);
            yield return new WaitForSeconds(waitTime);
            
            // Perform flicker
            yield return StartCoroutine(DoFlicker());
        }
    }

    IEnumerator DoFlicker()
    {
        if (lightToFlicker == null) yield break;
        
        isFlickering = true;
        
        // Play sound if available
        if (flickerSound != null)
            flickerSound.Play();
        
        // Store original values
        float originalIntensity = lightToFlicker.intensity;
        Color originalColor = lightToFlicker.color;
        
        // Quick flicker sequence
        int flickerCount = Random.Range(1, 4); // 1-3 quick flickers
        
        for (int i = 0; i < flickerCount; i++)
        {
            // Flicker off/dim
            lightToFlicker.intensity = flickerIntensity;
            if (colorFlicker)
                lightToFlicker.color = flickerColor;
            
            yield return new WaitForSeconds(flickerDuration * 0.3f);
            
            // Flicker back on
            lightToFlicker.intensity = originalIntensity;
            if (colorFlicker)
                lightToFlicker.color = originalColor;
            
            yield return new WaitForSeconds(flickerDuration * 0.7f);
        }
        
        // Ensure light is back to normal
        lightToFlicker.intensity = baseIntensity;
        lightToFlicker.color = baseColor;
        
        isFlickering = false;
    }

    // Public methods to control flickering from other scripts
    public void SetFlickerRate(float minTime, float maxTime)
    {
        minFlickerTime = minTime;
        maxFlickerTime = maxTime;
    }
    
    public void SetIntensity(float normal, float flicker)
    {
        baseIntensity = normal;
        flickerIntensity = flicker;
        
        if (lightToFlicker != null && !isFlickering)
            lightToFlicker.intensity = baseIntensity;
    }

    void OnDestroy()
    {
        StopFlickering();
    }
}