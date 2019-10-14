using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : Singelton<GameManager>
{   //Handles gamestates and holds references between scenes/manager scripts. 

    public GameObject[] playerObjects;
    public int ActivePlayers { get; set; }
    public TrailCleaner cleaner;
    public bool CheckWin = false;
    public int round;
    public enum GameState { mainMenu, running, paused, load, scoreboard};
    public GameState gameState = GameState.mainMenu;
    private static GameManager _instance = null;

    private void Start()
    {   //loads all scenes for build.
        SceneManager.LoadSceneAsync(1,LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }

    private void LateUpdate()
    {   //we run wincheck here so that if the game ends in draw all death methods resolve first, making sure that win isnt determined by playerIndex.
        if(CheckWin)
        {
            WinConditionCheck();
        }        
    }

    public void WinConditionCheck()
    {   //called upon player death. If it finds game over we trigger Scoreboard.
        int remainingPlayers = 0;
        Player[] playersInGame = new Player[ActivePlayers];

        for (int i = 0; i < ActivePlayers; i++)
        {
            playersInGame[i] = playerObjects[i].GetComponent<Player>();
            if (playersInGame[i].Alive) //if only one player is alive
            {
                remainingPlayers++;
            }            
        }
        if (remainingPlayers < 2 )
        {
            for (int i = 0; i < ActivePlayers; i++)
            {
                if (playersInGame[i].Alive) // we give that player 30 points, then end the round.
                {
                    playersInGame[i].Alive = false; //so it cant double trigger.
                    playersInGame[i].NewScore += 30f;
                }
            }
            gameState = GameState.scoreboard;
            RoundsManager.Instance.PostRound();
        }
        CheckWin = false;
    }

    public void AddPlayer(GameObject player, int playerNumber)
    {
        playerObjects[playerNumber] = player;
    }
}