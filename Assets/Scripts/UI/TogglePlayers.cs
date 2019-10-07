using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TogglePlayers : MonoBehaviour
{
    [SerializeField]
    private bool _additive = true;  //for going up or down at buttonpress, aka left or right key. 
    private Text _numberOfPlayers;
    [SerializeField]
    private GameObject player3;
    [SerializeField]
    private GameObject player4;

    void Start()
    {
        _numberOfPlayers = transform.parent.GetChild(2).GetComponent<Text>();
    }

    public void OnClick()
    {   //TODO save the addition or subtraction to a gamestate script aswell for initialization of game. 

        int _currentAmount = int.Parse(_numberOfPlayers.text);  //saving as float for math.
        if (_additive)
        {
            //need to check size of current players.
            if ( _currentAmount < 4)
            {
                //raise it, then back again.
                _currentAmount++;
                _numberOfPlayers.text = Convert.ToString(_currentAmount);
                ActivateUi(_currentAmount);
                GameManager.Instance.ActivePlayers = _currentAmount;
            }
        }
        else
        {
            if (_currentAmount > 2 )
            {
                //or lower it. 
                _currentAmount--;
                _numberOfPlayers.text = Convert.ToString(_currentAmount);
                ActivateUi(_currentAmount);
                GameManager.Instance.ActivePlayers = _currentAmount;
            }
        }
    }

    private void ActivateUi(float playersActive)
    {
        //tracks what UI elements to activate.
        switch (playersActive)
        {
            case 2:
                {
                    player3.SetActive(false);
                    GameManager.Instance.playerObjects[2].SetActive(false);
                    break;
                }
            case 3:
                {
                    player3.SetActive(true);
                    player4.SetActive(false);
                    GameManager.Instance.playerObjects[2].SetActive(true);
                    GameManager.Instance.playerObjects[3].SetActive(false);
                    break;
                }
            case 4:
                {

                    player3.SetActive(true);
                    player4.SetActive(true);
                    GameManager.Instance.playerObjects[2].SetActive(true);
                    GameManager.Instance.playerObjects[3].SetActive(true);
                    break;
                }
            default:
                break;
        }
    }
}
