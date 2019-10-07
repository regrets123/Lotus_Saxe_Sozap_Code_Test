using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundsManager : MonoBehaviour
{   //Handles logic loop between each round. 


    [SerializeField]
    private Canvas _roundsResults, _mainMenu, _counter;
    public GameObject textAnimator;


    void Start()
    {
        GameManager.Instance.roundsManager = this;
    }

    public IEnumerator StartCountdown()
    {
        _mainMenu.enabled = false;
        GameManager.Instance.gameState = GameManager.GameState.load;
        _counter.enabled = true;
        textAnimator.GetComponent<Animator>().SetTrigger("Start");

        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++)
        {
            GameManager.Instance.playerObjects[i].GetComponent<Player>().ToggleHud(true);
        }

        yield return new WaitForSeconds(5.5f);
        StartGame();



    }



    public void StartGame()
    {
        _mainMenu.enabled = false;
        _counter.enabled = false;
        GameManager.Instance.gameState = GameManager.GameState.running;

    }

}
