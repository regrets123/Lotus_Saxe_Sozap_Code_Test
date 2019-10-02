using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsController : MonoBehaviour
{
    private List<Player> _standings;

    private void Start()
    {
        _standings = new List<Player>();
    }

    public void UpdateList()
    {
        for (int i = 0; i < 4; i++)
        {
            _standings.Add(GameManager.Instance.playerObjects[i].GetComponent<Player>()); 
        }        
    }
}
