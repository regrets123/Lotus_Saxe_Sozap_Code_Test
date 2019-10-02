using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollision : MonoBehaviour
{
    private EdgeCollider2D _collider2D;
    private List<Vector2> _pointHolder;



    // Start is called before the first frame update
    void Start()
    {
        _collider2D = GetComponent<EdgeCollider2D>();
        _pointHolder = new List<Vector2>();
    }

    public void AddPoint(Vector2 vector)
    {
        _pointHolder.Add(vector);
        Vector2[] newPoints = new Vector2[_pointHolder.Count];
        for (int i = 0; i < newPoints.Length; i++)
        {
            Vector2 temp = _pointHolder[i];
            newPoints[i] = _pointHolder[i];
        }
        _collider2D.points = newPoints;

        //TODO at around 500 positions I start getting performance issues with 1 player. I could try making my own collision detection with vector points drawing circle colliders from a pool of colliders spawned at start.
        //Or just mimic their system but with a list instead of an array avoiding the 500 index forloop forced on me by unity using an array.
        //Depends on how good the players are and how much processing power accessible.
    }





}
