using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    // En caso de usar el nuevo Input System aquí es donde se debería modificar

    public static float GetPlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        return horizontal;
    }

    public static bool GetJump()
    {
        bool jump = Input.GetButtonDown("Jump");

        //if (jump)
        //    Debug.Log("InputManager: Jump Pressed...");

        return jump;
    }

    public static bool GetJumpHeldDown()
    {
        bool down = Input.GetButton("Jump");
        return down;
    }

    public static bool GetJumpReleasedUp()
    {
        bool up = Input.GetButtonUp("Jump");
        return up;
    }


    public static bool GetFire()
    {
        bool fire = Input.GetButtonDown("Fire1");
        return fire;
    }

    public static bool GetFireHeldDown()
    {
        bool heldDown = Input.GetButton("Fire1");
        return heldDown;
    }

    public static bool GetFireReleasedUp()
    {
        bool up = Input.GetButtonUp("Fire1");
        return up;
    }


}
