using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : Singelton<TrailManager>
{   //Manager is a central list to keep track of all colliders and trails. Could also be used to manage pool of trails to avoid initialisation during gameplay. 

    public TrailData[] activeTrails;
    public List<TrailData> staticTrails;

    private void Start()
    {
        activeTrails = new TrailData[4];
        staticTrails = new List<TrailData>();
    }
}
