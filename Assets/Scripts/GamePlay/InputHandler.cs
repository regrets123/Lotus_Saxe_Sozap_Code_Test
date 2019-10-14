using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{   //handles different keyinput depending on playerindex conversion.

    public void ParseInput(float xMoveSpeed,float rotationSpeed, Player controllingPlayer)
    {
        if (GameManager.Instance.gameState == GameManager.GameState.running)
        {
            if (Input.GetAxisRaw(ReturnHorizontalInput(controllingPlayer.index)) != 0)  //this should work with gamepad and joysticks aswell.
            {
                if (Input.GetAxisRaw(ReturnHorizontalInput(controllingPlayer.index)) < 0.1)
                {
                    transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                }
                else
                { transform.Rotate(0, 0, -(rotationSpeed * Time.deltaTime)); }
            }
            transform.position += transform.right * Time.deltaTime * xMoveSpeed;
            if (Input.GetAxisRaw(ReturnVerticalInput(controllingPlayer.index)) != 0)
            {
                if (Input.GetAxisRaw(ReturnVerticalInput(controllingPlayer.index)) > 0.01)
                { if (controllingPlayer.AttackOnCooldown == false) controllingPlayer.coroutines.Add(StartCoroutine(controllingPlayer.Attack())); }
                else
                { if (controllingPlayer.PhaseOnCooldown == false) controllingPlayer.coroutines.Add(StartCoroutine(controllingPlayer.Invulnerability())); }
            };
        }
        else if (GameManager.Instance.gameState == GameManager.GameState.load)
        {
            if (Input.GetAxisRaw(ReturnHorizontalInput(controllingPlayer.index)) != 0)  //this should work with gamepad and joysticks aswell.
            {
                if (Input.GetAxisRaw(ReturnHorizontalInput(controllingPlayer.index)) < 0.01)
                { transform.Rotate(0, 0, rotationSpeed * Time.deltaTime); }
                else
                { transform.Rotate(0, 0, -(rotationSpeed * Time.deltaTime)); }
            }
        }
    }

    private string ReturnHorizontalInput(int index)
    {   //swaps inputkey detection depending on playerindex
        if (index == 0)
        { return "Horizontal"; }
        else
        { return ("Horizontal"+index); }

    }
    
    private string ReturnVerticalInput(int index)
    {    //swaps inputkey detection depending on playerindex
        if (index == 0)
        { return "Vertical"; }
        else
        { return ("Vertical" + index); }
    }
        
}
