using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableByCatcher
{
    public Collider2D ColliderOnGround { get; }
    void MoveByCatcher(Vector2 move);
}
