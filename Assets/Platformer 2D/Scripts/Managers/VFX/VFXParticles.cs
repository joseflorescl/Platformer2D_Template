using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXParticles : VFXPlayable
{
    [SerializeField] private GameObject fxParticlesPrefab;

    ParticleSystem fxParticlesInstance;
    
    protected override void Init()
    {
        var obj = Instantiate(fxParticlesPrefab, transform);
        fxParticlesInstance = obj.GetComponentInChildren<ParticleSystem>();
    }

    public override void PlayOneShot(Vector3 position)
    {
        fxParticlesInstance.transform.position = position;
        fxParticlesInstance.Play();
    }

    public override void PlayOneShot()
    {
        fxParticlesInstance.Play();
    }


    public override void Play()
    {
        fxParticlesInstance.Play();
    }

    public override void Stop()
    {
        fxParticlesInstance.Stop();
    }

}
