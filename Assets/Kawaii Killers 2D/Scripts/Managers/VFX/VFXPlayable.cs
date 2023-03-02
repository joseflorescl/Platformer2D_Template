using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VFXPlayable : MonoBehaviour
{
    protected virtual void Awake()
    {
        Init();
    }

    protected abstract void Init();
    public abstract void PlayOneShot(Vector3 position);
    public abstract void PlayOneShot();
    public abstract void Play();
    public abstract void Stop();    
}
