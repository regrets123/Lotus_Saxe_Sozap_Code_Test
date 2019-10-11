using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailData
{
    public TrailHandler Handler { get; }
    public TrailCollision Collider { get;}

    public TrailData (TrailHandler handler, TrailCollision collider)
    {
        Handler = handler;
        Collider = collider;
    }
}
