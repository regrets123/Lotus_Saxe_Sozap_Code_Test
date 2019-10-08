using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ResultsController : MonoBehaviour
{   //Updates and sorts the standings and rankings of the players, finds winner. 

    private List<Player> _standings;

    private void Start()
    {
        _standings = new List<Player>();
    }

    public void CreateStandingsList()
    {
        for (int i = 0; i < GameManager.Instance.ActivePlayers; i++)
        {
            _standings.Add(GameManager.Instance.playerObjects[i].GetComponent<Player>()); 
        }        
    }

    public void SortStandings()
    {
        if(_standings[0] == null)
        {
            CreateStandingsList();
        }

        var newList = _standings.OrderByDescending(item => item.Score).ToList();
        _standings = newList;

    }

    public void DefineWinner()
    {
        for (int i = 0; i < _standings.Count; i++)
        {
            if(_standings[i].Score == 150)
            {
                //TODO Run winner animation!
            }
        }
    }
}
