using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoundsManager : Singelton<RoundsManager>
{   //Handles UI, logic between each round and triggers animations.

    public GameObject textAnimator, fireWorks;
    [SerializeField]
    private Canvas _roundsResults, _mainMenu, _counter;
    [SerializeField]
    private FillBar[] _fillBars;
    [SerializeField]
    private EventSystem _eventSystem;
    private int _roundsCount;
    private float _highestScore;


    void Start()
    {
        _roundsCount = 1;
    }

    public IEnumerator StartCountdown()
    {    //Triggers from buttonclick at mainmenu. Activates countdown animations and coroutines. 

        _mainMenu.enabled = false;
        _roundsResults.enabled = false;
        GameManager.Instance.gameState = GameManager.GameState.load;
        _counter.enabled = true;
        if(_roundsCount == 1)
        {
            textAnimator.GetComponent<Text>().text = "First to 150p Wins";
        }
        else
        {   //updates countdown text and removes old tails
            textAnimator.GetComponent<Text>().text = ("Round " + _roundsCount);

        }
        textAnimator.GetComponent<Animator>().SetTrigger("Start");

        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++)
        {
            Player player = GameManager.Instance.playerObjects[i].GetComponent<Player>();
            if (_roundsCount > 1)
            { player.ResetPlayer(true); }           
            else
            { player.ResetPlayer(false); }
            player.ToggleHud(true);
            player.ToggleAbilityHud(false);

        }
        yield return new WaitForSeconds(5.5f);
        StartGame();    
    }

    public void StartGame()
    {   //Changes gamestate to running for the player movement methods to start tracking and updating positions. 

        GameManager.Instance.gameState = GameManager.GameState.running;
        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++) //Ressurect all players from previous rounds, and reset score and positions.
        {
            Player player = GameManager.Instance.playerObjects[i].GetComponent<Player>();
            player.ToggleAbilityHud(true);
            player.gameObject.GetComponent<TrailHandler>().StartSpawning();
        }
    }

    public void Deselect()
    {   //Eventsystem saves last interacted UI element, so if u press space or enter it triggers it again, no matter if canvas is active.
        _eventSystem.SetSelectedGameObject(null);
    }

    public void PostRound()
    {   //Sorts ranking and keeps track of which round we at, resets playerstates, and stops all trail routines.

        _roundsCount++;
        _roundsResults.enabled = true;       
        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "Next Round";

        List <GameObject> tempList = new List <GameObject>();
        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++)
        {
            GameObject player = GameManager.Instance.playerObjects[i];
            player.GetComponent<TrailHandler>().StopAllCoroutines();
            Player playerScript = player.GetComponent<Player>();
            playerScript.StopPlayerCoroutines();
            playerScript.UpdatePlayerScore();
            tempList.Add(player);
            transform.GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
        }
        var sortedList = tempList.OrderByDescending(x => x.GetComponent<Player>().NewScore).ToList();
        _highestScore = sortedList[0].GetComponent<Player>().NewScore;


        for (int i = 0; i < sortedList.Count; i++)
        {
             ScoreBoardUpdate(i, sortedList[i].GetComponent<Player>());
        }
    }

    private void ScoreBoardUpdate(int index, Player playerData)
    {   //Updates Scoreboard Ui Elements, backgroundcolor 

        Transform uiElement = transform.GetChild(0).GetChild(0).GetChild(index);
        Color playerColor = playerData.TrailColor;
        for (int i = 0; i < 2; i++)
        {
            if(i == 0)
            {
                Color faded = playerColor;
                faded.a = 0.5f;
                uiElement.GetChild(0).GetChild(i).GetComponent<Image>().color = faded;
            }
            else
            { uiElement.GetChild(0).GetChild(i).GetComponent<Image>().color = playerColor; }
        }
        _fillBars[index].SetFill(playerData.Score);
        uiElement.GetChild(0).GetChild(2).GetComponent<Text>().text = playerData.PlayerName;
        uiElement.GetChild(0).GetChild(3).GetComponent<Text>().text = Convert.ToString(playerData.Score);

        if (playerData.Score != playerData.NewScore)
        {   //Then animate the gain for the new scores.
            _fillBars[index].StartValue = playerData.Score;
            _fillBars[index].EndValue = playerData.NewScore;
            StartCoroutine(FillDelay(index));
            uiElement.GetChild(0).GetChild(3).GetComponent<Text>().text = Convert.ToString(playerData.NewScore);
        }
    }

    private IEnumerator FillDelay(int index)
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(_fillBars[index].AnimateBar(0.01f, _fillBars[index].StartValue,index));
    }

    public void StartCelebration(int playerIndex)
    {   // Trigger animations of playerUiElement and fireworks etc.
        Transform uiElement = transform.GetChild(0).GetChild(0).GetChild(playerIndex).GetChild(0);
        uiElement.GetComponent<Animator>().SetTrigger("Winner");
        uiElement.GetChild(4).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "Menu";
        if(fireWorks.activeSelf == false)
        {
            fireWorks.SetActive(true);  //start particle effect!
        }
    }

    public void NextRound()
    {   //triggers on next round button when u finished at scoreboard
        if(_highestScore > 149.9)
        {
            BacktoMain();
            _roundsCount = 1;
        }
        else
        {
            GameManager.Instance.cleaner.EraseTrails();
            StartCoroutine(StartCountdown());
        }
        Deselect();
    }

    private void BacktoMain()
    {   //resets all gamevariables to default.

        GameManager.Instance.gameState = GameManager.GameState.mainMenu;
        _roundsResults.enabled = false;
        _mainMenu.enabled = true;
        fireWorks.SetActive(false);
        for (int i = 0; i < _fillBars.Length; i++)
        {   //fastest shortcut to the winner fields in ui.
            _fillBars[i].transform.parent.GetChild(4).gameObject.SetActive(false);
            _fillBars[i].transform.parent.GetComponent<Animator>().SetTrigger("Stop");
        }
        GameManager.Instance.cleaner.EraseTrails();
        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++)
        {
            _fillBars[i].Winner.SetActive(false);
            Player temp = GameManager.Instance.playerObjects[i].GetComponent<Player>();
            temp.ResetPlayer(false);
            temp.ToggleHud(false);

        }
    }
}
