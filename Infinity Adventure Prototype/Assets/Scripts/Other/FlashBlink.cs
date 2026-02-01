using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashBlink : MonoBehaviour
{
    [SerializeField] private MonoBehaviour damagebleObject;
    [SerializeField] private Material blinkMaterial;
    [SerializeField] private float blinkDuration = 0.2f;

    private float blinkTimer;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;
    private bool isBlinking;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;

        isBlinking = true;
    }

    private void Start()
    {
        if (damagebleObject is Player)
        {
            (damagebleObject as Player).OnFlashBlink += DamagableObject_OnFlashBlink;
        }
    }

    private void DamagableObject_OnFlashBlink(object sender, EventArgs e)
    {
        SetBlinkingMaterial();
    }

    private void Update()
    {
        if (isBlinking)
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer < 0)
            {
                SetDefaultMaterial();
            }
        }
    }

    public void StopBlinking()
    {
        SetDefaultMaterial();
        isBlinking = false;
    }

    private void SetBlinkingMaterial()
    {
        blinkTimer = blinkDuration;
        spriteRenderer.material = blinkMaterial;
    }

    private void SetDefaultMaterial()
    {
        spriteRenderer.material = defaultMaterial;
    }

    private void OnDestroy()
    {
        if (damagebleObject is Player)
        {
            (damagebleObject as Player).OnFlashBlink -= DamagableObject_OnFlashBlink;
        }
    }
}
