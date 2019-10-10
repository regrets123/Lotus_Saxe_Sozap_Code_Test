using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCleaner : MonoBehaviour
{
    [SerializeField]
    private Transform trailParent;

    private void Start()
    {
        trailParent =  transform;
        GameManager.Instance.cleaner = this;
    }

    public void EraseTrails()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            Destroy( transform.GetChild(0).GetChild(i).gameObject);
        }
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            Destroy(transform.GetChild(1).GetChild(i).gameObject);
        }
     
    }

}
