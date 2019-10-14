using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrailCollision : MonoBehaviour
{   //Handles collision point updates of the trail and splitting points.

    public TrailData TrailData { get; set; }
    public Transform StaticTrails { get; set; }
    public bool Expanding { get; set; }
    public int playerIndex { get; set; }
    public EdgeCollider2D collider2D;
    private List<Vector2> _pointHolder;

    private void Awake()
    {
        Expanding = true;
    }

    void Start()
    {
        collider2D = GetComponent<EdgeCollider2D>();
        _pointHolder = new List<Vector2>();
    }

    public void AddPoint(Vector2 vector)
    {
        if (Expanding)
        {
            _pointHolder.Add(vector);
            if (_pointHolder.Count % 2 != 0)   //if we make sure the actual collider array is uneven, we can make a symetrical split matching the "hole" collision point.
            {
                Vector2[] newPoints = new Vector2[_pointHolder.Count];
                for (int i = 0; i < newPoints.Length; i++)
                {
                    Vector2 temp = _pointHolder[i];
                    newPoints[i] = _pointHolder[i];
                }
                collider2D.points = newPoints;
            }
        }
    }

    public void SplitPoint(Transform collidingPlayer, Transform trailOwner)
    {   //we gonna find the closest collision point, find the closest position on the trailCollider, then split at that index.

        int breakIndex = FindBreakPoint(collidingPlayer.position, collider2D);
        TrailHandler originalTrail = trailOwner.GetComponent<TrailHandler>();

        SplitCollider(originalTrail, breakIndex);

        if (transform.parent.name != "StaticTrails")
        {
            originalTrail.NewAtColl();
        }
        collidingPlayer.GetComponent<TrailHandler>().NewAtColl();

    }

    public void SubSplit(TrailData originalData, Transform playerPosition)
    {   //if trail was lrdy split, second split simply removes the collider and the visual presentation of that part of the trail. 

        Vector2 vector2 = playerPosition.position;
        int index = FindBreakPoint(vector2, originalData.TrailCollider.collider2D);
        if (index > originalData.TrailCollider.collider2D.pointCount/2)
        {   //finds the corresponding keypoint to delete to "remove" the visual part of the trail
            float SmallTime = Mathf.Infinity;
            int smallIndex = 999;
            for (int i = 0; i < 4; i++)
            {
                if (TrailData.VisualTrail.widthCurve[i].time < SmallTime)
                {
                    SmallTime = TrailData.VisualTrail.widthCurve[i].time;
                    smallIndex = i;
                }
            }
            AnimationCurve temp = TrailData.VisualTrail.widthCurve;
            temp.RemoveKey(smallIndex);
            originalData.VisualTrail.widthCurve = temp;
        }
        else
        {
            //finds the corresponding keypoint to delete to "remove" the visual part of the trail
            float bigTime = 0;
            int bigIndex = 999;
            for (int i = 0; i < 4; i++)
            {
                if (TrailData.VisualTrail.widthCurve[i].time > bigTime)
                {
                    bigTime = TrailData.VisualTrail.widthCurve[i].time;
                    bigIndex = i;
                }
            }
            AnimationCurve temp = TrailData.VisualTrail.widthCurve;
            temp.RemoveKey(bigIndex);
            originalData.VisualTrail.widthCurve = temp;

        }
        gameObject.SetActive(false);
    }

    private int FindBreakPoint(Vector2 playerPosition, EdgeCollider2D collider)
    {
        int splitPoint = 999;
        float distance = Mathf.Infinity;
        Vector2 breakPoint = collider.ClosestPoint(playerPosition);
        for (int i = 0; i < collider.points.Length; i++)
        {
            float tempdistance = Vector2.Distance(collider.points[i], breakPoint);
            if (tempdistance < distance)
            {
                distance = tempdistance;
                splitPoint = (i);
            }
        }
        Debug.Log("Found breakpoint " + splitPoint + " out of " + collider.pointCount);
        return splitPoint;
    }

    private void SplitCollider(TrailHandler originalTrailHandler, int i)
    {   //for the hole to be symetrical we need an uneven array and remove equal amount of index at both sides of collision.

        if (i < 6) //if we collide with the first x.
        {
            Vector2[] firstNewCollider = new Vector2[collider2D.points.Length - (i + 5)]; //if its the first node we just make the new list one index shorter and offset with x.
            for (int j = 0; j < firstNewCollider.Length; j++)
            {
                firstNewCollider[j] = collider2D.points[j + 5 + i];
            }
            originalTrailHandler.NewTrailFromArr(firstNewCollider, originalTrailHandler.trailData);
            SplitGardientTrail(firstNewCollider.Length, collider2D.points.Length, true); 
            return;
        }
        else if (i > collider2D.points.Length - 6) //if we colide with the last x
        {
            Vector2[] firstNewCollider = new Vector2[collider2D.points.Length - (collider2D.points.Length-(i-5))]; // if its the last one we copy all but last aka, x.
            for (int j = 0; j < firstNewCollider.Length; j++)
            {
                firstNewCollider[j] = collider2D.points[j];
            }
            originalTrailHandler.NewTrailFromArr(firstNewCollider, originalTrailHandler.trailData);
            SplitGardientTrail(firstNewCollider.Length, collider2D.points.Length,false);
            return;
        }
        else
        {   //if its somewhere else we cut it at the point of collision, skipping x.
            Vector2[] firstNewCollider = new Vector2[i - 5];
            Vector2[] secondNewCollider = new Vector2[collider2D.points.Length - (i + 5)];
            for (int j = 0; j < firstNewCollider.Length; j++)
            {
                firstNewCollider[j] = collider2D.points[j];
            }
            for (int k = 0; k < secondNewCollider.Length; k++)
            {
                secondNewCollider[k] = collider2D.points[i + k + 5];
            }
            originalTrailHandler.NewTrailFromArr(firstNewCollider, originalTrailHandler.trailData);
            originalTrailHandler.NewTrailFromArr(secondNewCollider, originalTrailHandler.trailData);
            SplitGardientTrail(firstNewCollider.Length, collider2D.points.Length - secondNewCollider.Length, collider2D.points.Length);
            return;
        }
    }

    private void SplitGardientTrail(float firstIndex, float secondIndex, float fullTrail)
    {    //since gradient is from 0 to 1 on timescale, we need to divide index by total arrayLenght.

        float firstAlphaKeyTime = 1 - (firstIndex / fullTrail); //time seems reversed on gradient so we have to subtract from 1.
        float secondAlphaKeyTime = 1 - (secondIndex / fullTrail);

        AnimationCurve trailWidth = new AnimationCurve();
        Keyframe firstKey = new Keyframe(firstAlphaKeyTime, 0, 0, 0);
        Keyframe secondKey = new Keyframe(firstAlphaKeyTime + 0.001f, 0.13f, 0, 0);
        Keyframe thirdKey = new Keyframe(secondAlphaKeyTime - 0.001f, 0.13f, 0, 0);
        Keyframe fourthKey = new Keyframe(secondAlphaKeyTime, 0, 0, 0);
        trailWidth.AddKey(firstKey);
        trailWidth.AddKey(secondKey);
        trailWidth.AddKey(thirdKey);
        trailWidth.AddKey(fourthKey);
        TrailData.Handler.Renderer.widthCurve = trailWidth;
    }

    private void SplitGardientTrail(float firstIndex, float fullTrail, bool atStart)
    {   //here we cut from start or end depending on where we collide, thus different positions on the gradient.

        float firstAlphaKeyTime = (firstIndex / fullTrail);
        AnimationCurve trailWidth = new AnimationCurve();
        if (atStart)
        {
            Keyframe firstKey = new Keyframe(firstAlphaKeyTime - 0.01f, 0.13f, 0, 0);
            Keyframe secondKey = new Keyframe(firstAlphaKeyTime - 0.009f, 0, 0, 0);
            Keyframe thirdKey = new Keyframe(1, 0, 0, 0);
            Keyframe fourthKey = new Keyframe(1, 0, 0, 0);
            trailWidth.AddKey(firstKey);
            trailWidth.AddKey(secondKey);
            trailWidth.AddKey(thirdKey);
            trailWidth.AddKey(fourthKey);
            TrailData.Handler.Renderer.widthCurve = trailWidth;
        }
        else
        {
            Keyframe firstKey = new Keyframe(firstAlphaKeyTime - 0.01f, 0.13f, 0, 0);
            Keyframe secondKey = new Keyframe(firstAlphaKeyTime - 0.009f, 0, 0, 0);
            Keyframe thirdKey = new Keyframe(1, 0, 0, 0);
            Keyframe fourthKey = new Keyframe(1, 0, 0, 0);
            trailWidth.AddKey(firstKey);
            trailWidth.AddKey(secondKey);
            trailWidth.AddKey(thirdKey);
            trailWidth.AddKey(fourthKey);
            TrailData.Handler.Renderer.widthCurve = trailWidth;
        }
    }
}
