using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrailCollision : MonoBehaviour
{   //Handles collision point updates of the trail and splitting points.

    private EdgeCollider2D _collider2D;
    private List<Vector2> _pointHolder;
    [SerializeField]
    public TrailRenderer TrailRenderer { get; set; }
    public Transform StaticTrails { get; set; }
    public bool Expanding { get; set; }

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
            if (_pointHolder.Count % 2 == 0)   //if we make sure the actual collider array is uneven, we can make a symetrical split matching the "hole" collision point.
            {
                //Debug.Log("The list of positions is " + _pointHolder.Count + " elements long. This number is even, so we dont add yet.");
            }
            else
            {
                //Debug.Log("The list of positions is " + _pointHolder.Count + " elements long. This number is uneven, so we add it to collider.");
                Vector2[] newPoints = new Vector2[_pointHolder.Count];
                for (int i = 0; i < newPoints.Length; i++)
                {
                    Vector2 temp = _pointHolder[i];
                    newPoints[i] = _pointHolder[i];
                }
                _collider2D.points = newPoints;
            }
        }
    }


    private void SplitGardientTrail(float firstIndex, float secondIndex, float fullTrail)
    {
        //since gradient is from 0 to 1 on timescale, we need to divide index by total arrayLenght.
        float firstAlphaKeyTime = 1 - (firstIndex / fullTrail); //time seems reversed on gradient so i have to subtract from 1.
        float secondAlphaKeyTime = 1 - (secondIndex / fullTrail);

        AnimationCurve trailWidth = new AnimationCurve();
        Keyframe firstKey = new Keyframe(firstAlphaKeyTime, 0, 0, 0);
        Keyframe secondKey = new Keyframe(firstAlphaKeyTime  +0.001f, 0.16f, 0, 0);
        Keyframe thirdKey = new Keyframe(secondAlphaKeyTime - 0.001f, 0.16f, 0, 0);
        Keyframe fourthKey = new Keyframe(secondAlphaKeyTime , 0, 0, 0);
        trailWidth.AddKey(firstKey);
        trailWidth.AddKey(secondKey);
        trailWidth.AddKey(thirdKey);
        trailWidth.AddKey(fourthKey);
        TrailRenderer.widthCurve = trailWidth;

    }

    private void SplitGardientTrail(float firstIndex, float fullTrail,bool atStart)
    {
        float firstAlphaKeyTime = (firstIndex / fullTrail); //time seems reversed on gradient so i have to subtract from 1.

        AnimationCurve trailWidth = new AnimationCurve();
        if(atStart)
        {
            Keyframe firstKey = new Keyframe(firstAlphaKeyTime - 0.01f, 0.16f, 0, 0);
            Keyframe secondKey = new Keyframe(firstAlphaKeyTime - 0.009f, 0, 0, 0);
            Keyframe thirdKey = new Keyframe(1, 0, 0, 0);
            Keyframe fourthKey = new Keyframe(1, 0, 0, 0);
            trailWidth.AddKey(firstKey);
            trailWidth.AddKey(secondKey);
            trailWidth.AddKey(thirdKey);
            trailWidth.AddKey(fourthKey);
            TrailRenderer.widthCurve = trailWidth;
        }
        else
        {
            Keyframe firstKey = new Keyframe(firstAlphaKeyTime - 0.01f, 0.16f, 0, 0);
            Keyframe secondKey = new Keyframe(firstAlphaKeyTime - 0.009f, 0, 0, 0);
            Keyframe thirdKey = new Keyframe(1, 0, 0, 0);
            Keyframe fourthKey = new Keyframe(1, 0, 0, 0);
            trailWidth.AddKey(firstKey);
            trailWidth.AddKey(secondKey);
            trailWidth.AddKey(thirdKey);
            trailWidth.AddKey(fourthKey);
            TrailRenderer.widthCurve = trailWidth;
        }       

    }

    private void SplitCollider(TrailHandler handler, int i)
    {   //for the hole to be symetrical we need an uneven array and remove equal amount of index at both sides of collision.

        if (i < 6) //if we collide with the first x.
        {
            Vector2[] firstNewCollider = new Vector2[_collider2D.points.Length - (i + 5)]; //if its the first node we just make the new list one index shorter and offset with x.
            for (int j = 0; j < firstNewCollider.Length; j++)
            {
                firstNewCollider[j] = _collider2D.points[j + 5 + i];
            }
            handler.NewTrailFromArr(firstNewCollider);
            SplitGardientTrail(firstNewCollider.Length, _collider2D.points.Length, true); 
            return;
        }
        else if (i > _collider2D.points.Length - 6) //if we colide with the last x
        {
            Vector2[] firstNewCollider = new Vector2[_collider2D.points.Length - (_collider2D.points.Length-(i-5))]; // if its the last one we copy all but last aka, x.
            for (int j = 0; j < firstNewCollider.Length; j++)
            {
                firstNewCollider[j] = _collider2D.points[j];
            }
            handler.NewTrailFromArr(firstNewCollider);
            SplitGardientTrail(firstNewCollider.Length, _collider2D.points.Length,false);
            return;
        }
        else
        {   //if its somewhere else we cut it at the point of collision, skipping x.
            Vector2[] firstNewCollider = new Vector2[i - 5];
            Vector2[] secondNewCollider = new Vector2[_collider2D.points.Length - (i + 5)];
            for (int j = 0; j < firstNewCollider.Length; j++)
            {
                firstNewCollider[j] = _collider2D.points[j];
            }
            for (int k = 0; k < secondNewCollider.Length; k++)
            {
                secondNewCollider[k] = _collider2D.points[i + k + 5];
            }
            handler.NewTrailFromArr(firstNewCollider);
            handler.NewTrailFromArr(secondNewCollider);
            SplitGardientTrail(firstNewCollider.Length, _collider2D.points.Length - secondNewCollider.Length, _collider2D.points.Length);
            return;
        }
    }

    public void SplitPoint(Transform collidingPlayer)
    {   //we gonna find the closest collision point, find the closest position on the trailCollider, then split at that index.
        Vector2 compareMe = collidingPlayer.position;
        Vector2 breakPoint = _collider2D.ClosestPoint(compareMe);
        TrailHandler handler = collidingPlayer.GetComponent<TrailHandler>();
        float distance = Mathf.Infinity;
        int splitPoint = 999;

        for (int i = 0; i < _collider2D.points.Length; i++)
        {
            float tempdistance = Vector2.Distance(_collider2D.points[i], breakPoint);
            if (tempdistance < distance)
            {
                distance = tempdistance;
                splitPoint = (i);
            }
        }
        Debug.Log("splitpoint is at index:" + splitPoint);
        SplitCollider(handler, splitPoint);

    }


}
