using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : PickupController
{    

    private void Start()
    {
        GameManager.Instance.CoinCreated(this);
    }

    protected override bool PickupCatched()
    {
        GameManager.Instance.CoinCatched(this);
        return false;
    }
}
