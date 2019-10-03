using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollision : MonoBehaviour
{
    private EdgeCollider2D _collider2D;
    private List<Vector2> _pointHolder;
    [SerializeField]
    public TrailRenderer TrailRenderer { get; set; }
    public Transform StaticTrails { get; set; }
    public bool Expanding { get; set; }



    // Start is called before the first frame update
    private void Awake()
    {
        Expanding = true;
    }

    void Start()
    {
        _collider2D = GetComponent<EdgeCollider2D>();
        _pointHolder = new List<Vector2>();
    }

    public void AddPoint(Vector2 vector)
    {
        if (Expanding)
        {
            _pointHolder.Add(vector);
            Vector2[] newPoints = new Vector2[_pointHolder.Count];
            for (int i = 0; i < newPoints.Length; i++)
            {
                Vector2 temp = _pointHolder[i];
                newPoints[i] = _pointHolder[i];
            }
            _collider2D.points = newPoints;
        }
    }




    public void SplitTrail(Transform collidingPlayer)
    {   //we gonna find the closest collision point, find the closest position on the trailCollider, then run a loop to find the array position and cut at that index, creating 2 new arrays,
        //then use the index to edit a new gradiant.
        Vector2 compareMe = collidingPlayer.position;
        Vector2 breakPoint = _collider2D.ClosestPoint(compareMe);
        TrailHandler handler = collidingPlayer.GetComponent<TrailHandler>();

        for (int i = 0; i < _collider2D.points.Length; i++)
        {   //explosion radius will be one node. so we remove the node we collide with and make a new array.
            float distance = Vector2.Distance(_collider2D.points[i], breakPoint);
            if (distance < 0.15f)    //errormargin for performance, tweak this if we dont find or find wrong node.
            {
                if(i == 0)
                {
                    Vector2[] firstNewCollider = new Vector2[_collider2D.points.Length - 2]; //if its the first node we just make the new list one index shorter and offset with 2.
                    for (int j = 0; j < firstNewCollider.Length; j++)
                    {
                        firstNewCollider[j] = _collider2D.points[j + 2];
                        handler.NewTrailFromArr(firstNewCollider);
                        return;
                    }
                }
                else if (i == _collider2D.points.Length-1)
                {
                    Vector2[] firstNewCollider = new Vector2[_collider2D.points.Length - 2]; // if its the last one we copy all but last aka, 2 index less.
                    for (int j = 0; j < firstNewCollider.Length; j++)
                    {
                        firstNewCollider[j] = _collider2D.points[j];
                        handler.NewTrailFromArr(firstNewCollider);
                        return;
                    }
                }
                else
                {   //if its somewhere else we cut it at the point of collision, skipping 3 index. 
                    Vector2[] firstNewCollider = new Vector2[i - 1];
                    Vector2[] secondNewCollider = new Vector2[_collider2D.points.Length - (i + 1)];
                    for (int j = 0; j < firstNewCollider.Length; j++)
                    {
                        firstNewCollider[j] = _collider2D.points[j];
                    }
                    for (int k = 0; k < secondNewCollider.Length; k++)
                    {
                        secondNewCollider[k] = _collider2D.points[i+k+1];
                    }
                    handler.NewTrailFromArr(firstNewCollider);
                    handler.NewTrailFromArr(secondNewCollider);
                }
            }
        }
    }


}
