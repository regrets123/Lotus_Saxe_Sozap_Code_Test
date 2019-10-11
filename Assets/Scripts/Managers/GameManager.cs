using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{   //Handles gamestates and holds references between scenes/manager scripts. 
    //Since its a small game it can be argued that It can all run in the same scene and just use different canvases for the different states/ui-screens. However, If you expand on projects and have more than developer, 
    //I find multiple screens far superior due to pushing scenes can easy erase informaiton from others working on the same scene. So if you split scenes between the different game parts, its safer as the team/project expands, example UI/Gameplay/Manager scenes.

    private static GameManager _instance = null;
    public GameObject[] playerObjects;
    public int ActivePlayers { get; set; }
    public RoundsManager roundsManager;
    public TrailCleaner cleaner;
    public bool CheckWin = false;

    public int round;
    public enum GameState { mainMenu, running, paused, load, scoreboard};

    public GameState gameState = GameState.mainMenu;
    

    private void Update()
    {
            //switch depending on gamestate do different methods on space and enter.
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {   //we run wincheck here so that if the game ends in draw all death methods resolve first, making sure that win isnt determined by playerIndex.
        if(CheckWin)
        {
            WinConditionCheck();
        }        
    }

    public static GameManager Instance
    { //setting up static singelton reference
        get
        {
            if (_instance == null)
            { //checking for duplicates
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    //instanziating new one.
                    GameObject gameManager = new GameObject();
                    _instance = gameManager.AddComponent<GameManager>();
    
                }
            }
            return _instance;
        }
    }

    public void WinConditionCheck()
    {   //called upon player death. If it finds game over we trigger Scoreboard.
        int remainingPlayers = 0;

        for (int i = 0; i < ActivePlayers; i++)
        {
            if(playerObjects[i].GetComponent<Player>().Alive) //if only one player is alive
            {
                remainingPlayers++;
            }            
        }
        if (remainingPlayers < 2 )
        {
            for (int i = 0; i < ActivePlayers; i++)
            {
                if (playerObjects[i].GetComponent<Player>().Alive) // we give that player 30 points, then end the round.
                {
                    playerObjects[i].GetComponent<Player>().Alive = false; //so it cant double trigger.
                    playerObjects[i].GetComponent<Player>().NewScore += 30f;
                }
            }
            gameState = GameState.scoreboard;
            roundsManager.PostRound();
        }
        CheckWin = false;
    }

    public void AddPlayer(GameObject player, int playerNumber)
    {
        playerObjects[playerNumber] = player;
    }
}