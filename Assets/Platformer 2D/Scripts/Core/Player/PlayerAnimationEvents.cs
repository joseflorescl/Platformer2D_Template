using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private Transform rightFootOnGround;
    [SerializeField] private Transform leftFootOnGround;
   
    public void RightFootOnGround()
    {
        GameManager.Instance.PlayerStep(rightFootOnGround.position);
    }

    public void LeftFootOnGround()
    {
        GameManager.Instance.PlayerStep(leftFootOnGround.position);
    }


}
