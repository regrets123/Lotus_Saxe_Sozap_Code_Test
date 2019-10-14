using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailData
{   //to track all trailparts during the different stages of creating, cut, splitting, subsplits.
    public TrailHandler Handler { get; }
    public TrailCollision TrailCollider { get;}
    public TrailRenderer VisualTrail { get; }
    public TrailData beforeSplitTrail;

    public TrailData (TrailHandler handler, TrailCollision collider,TrailRenderer renderer)
    {   
        Handler = handler;
        TrailCollider = collider;
        VisualTrail = renderer;
    }
}
