using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TogglePlayers : MonoBehaviour
{   //Ui script for button to toggle

    [SerializeField]
    private GameObject _player3, _player4;
    [SerializeField]
    private bool _additive = true;  //for going up or down at buttonpress, aka left or right key. 
    private Text _numberOfPlayers;

    void Start()
    {
        _numberOfPlayers = transform.parent.GetChild(2).GetComponent<Text>();
    }

    public void OnClick()
    {  
        int _currentAmount = int.Parse(_numberOfPlayers.text);  //saving as float for math.
        if (_additive)
        {
            //need to check size of current players.
            if ( _currentAmount < 4)
            {    //raise it, then back again.
                _currentAmount++;
                _numberOfPlayers.text = Convert.ToString(_currentAmount);
                ActivateUi(_currentAmount);
                GameManager.Instance.ActivePlayers = _currentAmount;
            }
        }
        else
        {
            if (_currentAmount > 2 )
            {    //or lower it. 
                _currentAmount--;
                _numberOfPlayers.text = Convert.ToString(_currentAmount);
                ActivateUi(_currentAmount);
                GameManager.Instance.ActivePlayers = _currentAmount;
            }
        }
        RoundsManager.Instance.Deselect();
    }

    private void ActivateUi(float playersActive)
    {
        //tracks what UI elements to activate.
        switch (playersActive)
        {
            case 2:
                {   _player3.SetActive(false);
                    GameManager.Instance.playerObjects[2].SetActive(false);
                    break;
                }
            case 3:
                {   _player3.SetActive(true);
                    _player4.SetActive(false);
                    GameManager.Instance.playerObjects[2].SetActive(true);
                    GameManager.Instance.playerObjects[3].SetActive(false);
                    break;
                }
            case 4:
                {   _player3.SetActive(true);
                    _player4.SetActive(true);
                    GameManager.Instance.playerObjects[2].SetActive(true);
                    GameManager.Instance.playerObjects[3].SetActive(true);
                    break;
                }
            default:
                break;
        }
    }
}
