using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyVFX : MonoBehaviour
{
    private ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (particleSystem && !particleSystem.IsAlive())
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
