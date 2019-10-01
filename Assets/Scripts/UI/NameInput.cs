using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [SerializeField]
    private Text playerName;

    public void OnEndEdit()
    {   //Find the written text and update the playerName.
        string temp = GetComponent<InputField>().text;
        playerName.text = temp;
    }
}
