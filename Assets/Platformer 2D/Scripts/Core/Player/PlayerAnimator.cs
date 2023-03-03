using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;    

    int speedParamID;
    int isOnGroundID;

    private void Start()
    {
        speedParamID = Animator.StringToHash("Speed");
        isOnGroundID = Animator.StringToHash("Is On Ground");
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerStartMoving += PlayerStartMovingHandler;
        GameManager.Instance.OnPlayerStopMoving += PlayerStopMovingHandler;
        GameManager.Instance.OnPlayerIsGroundedChanged += PlayerIsGroundedChangedHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerStartMoving -= PlayerStartMovingHandler;
        GameManager.Instance.OnPlayerStopMoving -= PlayerStopMovingHandler;
        GameManager.Instance.OnPlayerIsGroundedChanged -= PlayerIsGroundedChangedHandler;
    }    

    private void PlayerStartMovingHandler()
    {
        anim.SetFloat(speedParamID, 1);
    }

    private void PlayerStopMovingHandler()
    {
        anim.SetFloat(speedParamID, 0);
    }


    private void PlayerIsGroundedChangedHandler(bool value)
    {
        anim.SetBool(isOnGroundID, value);
    }
    
}
