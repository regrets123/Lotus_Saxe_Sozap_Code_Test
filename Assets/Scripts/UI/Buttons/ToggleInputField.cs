using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInputField : MonoBehaviour
{
    [SerializeField]
    private GameObject _inputField, _placeHolder;

    public void Toggle()
    { //if off turn on, and vice versa. 
        if (_inputField.activeSelf)
        {
            _inputField.SetActive(false);
            _placeHolder.SetActive(true);
        }
        else
        {
            _inputField.SetActive(true);
            _placeHolder.SetActive(false);
        }
    }
}
