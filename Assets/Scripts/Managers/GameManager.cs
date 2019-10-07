using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public GameObject[] playerObjects;
    public string[] playerNames { get; set; }  
    public int ActivePlayers { get; set; }
    public RoundsManager roundsManager;

    public int round;
    public enum GameState { mainMenu, running, paused, load, scoreboard};

    public GameState gameState = GameState.mainMenu;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        StartUp();
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

    private void StartUp()
    {   //sets up the array for default names.
        playerNames = new string[4];
        for (int i = 0; i < playerNames.Length; i++)
        {
            playerNames[i] = "Player" + Convert.ToString(i+1);
        }
        playerObjects = new GameObject[4];

        ActivePlayers = 2;
    }


    public void AddPlayer(GameObject player, int playerNumber)
    {
        playerObjects[playerNumber] = player;
    }
}