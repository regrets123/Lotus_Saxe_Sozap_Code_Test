using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour
{   //Manager is a central list to keep track of all colliders and trails.

    [SerializeField]
    private Transform _activeParent, _staticParent;
    public TrailData[] activeTrails;
    public List<TrailData> staticTrails;
    private static TrailManager _instance = null;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        activeTrails = new TrailData[4];
        staticTrails = new List<TrailData>(); 
    }


    public static TrailManager Instance
    { //setting up static singelton reference
        get
        {
            if (_instance == null)
            { //checking for duplicates
                _instance = FindObjectOfType<TrailManager>();
                if (_instance == null)
                {
                    //instanziating new one.
                    GameObject gameManager = new GameObject();
                    _instance = gameManager.AddComponent<TrailManager>();

                }
            }
            return _instance;
        }
    }


}
