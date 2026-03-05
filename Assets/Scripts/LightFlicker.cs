using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Light Settings")]
    public Light lightToFlicker;           
    public AudioSource flickerSound;      
    
    [Header("Flicker Settings")]
    public float baseIntensity = 1f;       
    public float flickerIntensity = 0.3f;  
    public float minFlickerTime = 0.1f;    
    public float maxFlickerTime = 3f;      
    public float flickerDuration = 0.1f;  
    
    [Header("Other Settings")]
    public bool randomStart = true;        
    public bool colorFlicker = false;     
    public Color baseColor = Color.white;  
    public Color flickerColor = Color.yellow; 

    private Coroutine flickerCoroutine;
    private bool isFlickering = false;

    void Start()
    {
        if (lightToFlicker == null)
            lightToFlicker = GetComponent<Light>();
        
        if (lightToFlicker != null)
        {
            baseIntensity = lightToFlicker.intensity;
            baseColor = lightToFlicker.color;
        }
        
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
        
        if (lightToFlicker != null)
        {
            lightToFlicker.intensity = baseIntensity;
            lightToFlicker.color = baseColor;
        }
        
        isFlickering = false;
    }

    IEnumerator FlickerLoop()
    {
        if (randomStart)
            yield return new WaitForSeconds(Random.Range(0f, maxFlickerTime));

        while (true)
        {
            float waitTime = Random.Range(minFlickerTime, maxFlickerTime);
            yield return new WaitForSeconds(waitTime);
            
            yield return StartCoroutine(DoFlicker());
        }
    }

    IEnumerator DoFlicker()
    {
        if (lightToFlicker == null) yield break;
        
        isFlickering = true;
        
        if (flickerSound != null)
            flickerSound.Play();
        
        float originalIntensity = lightToFlicker.intensity;
        Color originalColor = lightToFlicker.color;
        
        int flickerCount = Random.Range(1, 4); // 1-3 quick flickers
        
        for (int i = 0; i < flickerCount; i++)
        {
            lightToFlicker.intensity = flickerIntensity;
            if (colorFlicker)
                lightToFlicker.color = flickerColor;
            
            yield return new WaitForSeconds(flickerDuration * 0.3f);
            
            lightToFlicker.intensity = originalIntensity;
            if (colorFlicker)
                lightToFlicker.color = originalColor;
            
            yield return new WaitForSeconds(flickerDuration * 0.7f);
        }
        
        lightToFlicker.intensity = baseIntensity;
        lightToFlicker.color = baseColor;
        
        isFlickering = false;
    }

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