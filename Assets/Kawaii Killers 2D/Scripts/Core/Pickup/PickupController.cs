using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupController : MonoBehaviour, IVFXEntity
{
    public Vector3 Position => transform.position;

    protected abstract bool PickupCatched(); // Retorna true, se debe destruir el pickup


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // El layer Pickup solo puede colisionar consigo mismo
        // Por eso el objeto del player tiene un collider especializado para esto.
        // Nada más puede colisionar con un powerup, por eso no se valida con un if el tipo de objeto que viene en collision
        bool destroy = PickupCatched();
        
        if (destroy)
            //PoolManager.Instance.Release(gameObject); // Esto es cuando se use object pool
            Destroy(gameObject);
    }

}
