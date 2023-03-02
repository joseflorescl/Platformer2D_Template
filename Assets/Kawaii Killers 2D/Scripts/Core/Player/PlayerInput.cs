using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float Horizontal => horizontal;
    public bool JumpPressed => jumpPressed;
    public bool JumpReleasedUp => jumpReleasedUp;
    public bool JumpHeldDown => jumpHeldDown;

    float horizontal;
    bool readyToClear;
    bool jumpPressed;
    bool jumpReleasedUp;
    bool jumpHeldDown;

    private void Update()
    {
        if (readyToClear)
            ClearInput();

        /*
         * Aquí viene algo muy interesante: recordar que el uso del input se hace en los métodos de FixUpd, 
         * pero los métodos Update y FixUpd pueden estar desincronizados: podríamos llegar a perder un input en un Update 
         * y cuando se ejecute el FixUpd que lo quería usar, ya fue limpiado y el monito no saltó o no se movió.
         */

        horizontal += InputManager.GetPlayerMovement();
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);

        jumpPressed = jumpPressed || InputManager.GetJump();
        jumpReleasedUp = jumpReleasedUp || InputManager.GetJumpReleasedUp();
        jumpHeldDown = jumpHeldDown || InputManager.GetJumpHeldDown();
    }

    void FixedUpdate()
    {
        //In FixedUpdate() we set a flag that lets inputs to be cleared out during the 
        //next Update(). This ensures that all code gets to use the current inputs
        //if (jumpPressed)
        //    print("PlayerInput: Jump Pressed...");

        readyToClear = true;
    }

    public void ClearInput()
    {
        horizontal = 0;
        jumpPressed = false;
        jumpReleasedUp = false;
        jumpHeldDown = false;

        readyToClear = false;
    }

    public void ClearJumpPressed()
    {
        jumpPressed = false;
    }
}
