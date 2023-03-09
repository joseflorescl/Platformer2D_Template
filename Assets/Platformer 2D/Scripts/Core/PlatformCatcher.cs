using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCatcher : MonoBehaviour
{
    [SerializeField] private Collider2D colliderOnPlatform; // Inic en el Inspector
    [SerializeField] private ContactFilter2D contactFilter; // Para filtrar en la función GetContacts

    HashSet<Collider2D> caughtColliders = new();
    ContactPoint2D[] contactPoints = new ContactPoint2D[20];

    public void MoveCaughtObjects(Vector2 move)
    {
        int contactCount = colliderOnPlatform.GetContacts(contactFilter, contactPoints);

        if (contactCount == 0)
            return;

        // Metemos los colliders en contacto en un HashSet
        caughtColliders.Clear();
        for (int i = 0; i < contactCount; i++)
        {
            caughtColliders.Add(contactPoints[i].collider);
        }
        
        foreach (Collider2D coll2D in caughtColliders)
        {
            if (coll2D.TryGetComponent(out IMoveableByCatcher moveableByCatcher))
            {
                // Solo si el character está SOBRE el collider de la plataforma, entonces lo movemos junto con la plataforma,
                //  porque se puede dar el caso que el character esté en contacto por el lado con la plataforma, pero por eso
                //  no lo vamos a mover junto con ella
                if (moveableByCatcher.ColliderOnGround == colliderOnPlatform)
                    moveableByCatcher.MoveByCatcher(move);
            }
        }
    }
}
