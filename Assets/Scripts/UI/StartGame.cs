using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private Text [] playerNames = new Text[4];

    public void StarTheGame()
    {   //Makes a new array matching the size of current active players.
        string[] playersInGame = new string[Convert.ToInt16(GameManager.Instance.ActivePlayers)];
        for (int i = 0; i < playersInGame.Length; i++)
        {
           playersInGame[i] = playerNames[i].text;
        }
        GameManager.Instance.playerNames = playersInGame;   //Then we update the list at gameManager. 
        //TODO initialize sceneload and game stuff.
        
    }
}
