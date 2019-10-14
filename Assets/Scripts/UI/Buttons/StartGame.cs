using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private Text [] _playerNames = new Text[4];
    [SerializeField]
    private Text _numberOfPlayer;

    public void StarTheGame()
    {   //takes the updated names from UI elements and assigns them to the Player Scripts.
        GameManager.Instance.ActivePlayers = Convert.ToInt16(_numberOfPlayer.text);
        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++)
        {
            GameManager.Instance.playerObjects[i].GetComponent<Player>().PlayerName = _playerNames[i].text;
        }
        StartCoroutine(RoundsManager.Instance.StartCountdown());
        RoundsManager.Instance.Deselect();
    }
}
