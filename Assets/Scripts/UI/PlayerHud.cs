using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{

    [SerializeField]
    private Transform playerHud;

    private void TrackHudPosition()
    {   //tracks each players Hud to the ships position.
        playerHud.position = transform.position;
    }

    void Update()
    {
        TrackHudPosition();
    }
}
