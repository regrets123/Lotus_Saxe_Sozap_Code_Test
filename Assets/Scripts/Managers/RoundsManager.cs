using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RoundsManager : MonoBehaviour
{   //Handles logic loop between each round and triggers animations.


    [SerializeField]
    private Canvas _roundsResults, _mainMenu, _counter;
    [SerializeField]
    private FillBar[] _fillBars;
    private int _roundsCount;
    private int _winner = 0;
    public GameObject textAnimator;

    void Start()
    {
        GameManager.Instance.roundsManager = this;
        _roundsCount = 1;
    }


    public IEnumerator StartCountdown()
    {    //Triggers from buttonclick at mainmenu. Activates countdown animations and coroutines. 

        _mainMenu.enabled = false;
        GameManager.Instance.gameState = GameManager.GameState.load;
        _counter.enabled = true;
        if(_roundsCount == 1)
        {
            textAnimator.GetComponent<Text>().text = "First to 150p Wins";
        }
        else
        {
            textAnimator.GetComponent<Text>().text = ("Round " + _roundsCount);
        }
        textAnimator.GetComponent<Animator>().SetTrigger("Start");

        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++)
        {
            GameManager.Instance.playerObjects[i].GetComponent<Player>().ToggleHud(true);
        }
        yield return new WaitForSeconds(5.5f);
        StartGame();
    }

    public void StartGame()
    {   //Changes gamestate to running for the player movement methods to start tracking and updating positions. 

        bool resetScore;
        if (_roundsCount == 1)
        { resetScore = true; }
        else
        { resetScore = false; }

        _counter.enabled = false;
        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++) //Ressurect all players from previous rounds, and reset score and positions.
        {
            Player temp = GameManager.Instance.playerObjects[i].GetComponent<Player>();
            temp.ResetPlayer(resetScore);
        }
        GameManager.Instance.gameState = GameManager.GameState.running;

    }

    public void PostRound()
    {   //Sorts ranking and keeps track of which round we at.

        _roundsCount++;
        _roundsResults.enabled = true;
        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "Next Round";

        List <GameObject> tempList = new List <GameObject>();
        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++)
        {
            GameObject player = GameManager.Instance.playerObjects[i];
            tempList.Add(player);
            transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
        }
        var sortedList = tempList.OrderByDescending(x => x.GetComponent<Player>().Score).ToList();
        
        for (int i = 0; i < sortedList.Count; i++)
        {
             ScoreBoardUpdate(i, sortedList[i].GetComponent<Player>());
        }
    }

    private void ScoreBoardUpdate(int index, Player playerData)
    {   //Updates Scoreboard Ui Elements, backgroundcolor 

        Transform uiElement = transform.GetChild(0).GetChild(0).GetChild(index);
        Color playerColor = playerData.transform.GetChild(1).GetComponent<TrailRenderer>().colorGradient.colorKeys[0].color;
        for (int i = 0; i < 2; i++)
        {
            if(i == 0)
            {
                Color faded = playerColor;
                faded.a = 0.5f;
                uiElement.GetChild(i).GetComponent<Image>().color = faded;
            }
            else
            {
                uiElement.GetChild(i).GetComponent<Image>().color = playerColor;
            }
        }
        uiElement.GetChild(2).GetComponent<Text>().text = playerData.PlayerName;
        uiElement.GetChild(3).GetComponent<Text>().text = Convert.ToString(playerData.NewScore);

        _fillBars[index].StartValue = playerData.Score;
        _fillBars[index].EndValue = playerData.NewScore;
        StartCoroutine(FillDelay(index));
    }

    private IEnumerator FillDelay(int index)
    {
        yield return new WaitForSeconds(0.25f);
        _fillBars[index].StartFilling = true;
    }

    private void StartCeleb()
    {   //TODO Trigger animations of playerUiElement and fireworks etc.

        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "Menu";
    }

    public void ReturnToMain()
    {
        _winner = 0;
    }
}
