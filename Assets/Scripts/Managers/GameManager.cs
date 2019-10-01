using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public string[] playerNames { get; set; }  
    public float ActivePlayers { get; set; }


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
                    //DontDestroyOnLoad(gameManager); //TODO remove this because Id rather use asynchronous loading to keep the loading off mainthread.
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
    }

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

}