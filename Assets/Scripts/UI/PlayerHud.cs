using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{

    [SerializeField]
    private Transform playerHud;

    private void TrackHudPosition()
    {   //tracks each players Hud to the ships position.

        Vector2 hudPos = Camera.main.WorldToScreenPoint(transform.position);
        playerHud.position = hudPos;
        //TODO this needs testing
    }

    void Update()
    {
        TrackHudPosition();
    }
}
