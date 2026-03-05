using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableSpriteShadows : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            sprite.receiveShadows = true;
        }
    }
}