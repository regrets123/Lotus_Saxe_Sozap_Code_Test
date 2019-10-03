using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailHandler : MonoBehaviour
{

    public GameObject _trailPrefab, _collisionPrefab;
    [SerializeField]
    private TrailCollision _trail;
    private Gradient _color;
    public Player player { get; set; }

    private void Start()
    {
        player = GetComponent<Player>();  //saving the gradient for new trails.
        _color = transform.GetChild(1).GetComponent<TrailRenderer>().colorGradient;
        StartCoroutine(UpdateTrail());
        StartCoroutine(TrailCut(true));
    }

    private IEnumerator StopEmitting(float time)
    {   //Modifying method to match end of trail to match colliders.
        yield return new WaitForSecondsRealtime(time - 0.1f);
        transform.GetChild(1).GetComponent<TrailRenderer>().emitting = false;
    }

    public IEnumerator TrailCut(bool recursive)
    {   //faking a "cut" in the trail by creating a new trail and stopping the other trail.
        //TODO add a breakcondition in each of these methods so they dont continue when they shouldnt, aka end of player or end of game.
        float trailLength;
        if (recursive)
        { trailLength = Random.Range(3, 5); }
        else
        { trailLength = 0; }       
        StartCoroutine(StopEmitting(trailLength));
        yield return new WaitForSecondsRealtime(trailLength);
        _trail.Expanding = false;
        Transform staticTrails = _trail.transform.parent.parent.GetChild(0); //moving them to a empty object to keep them sorted.
        _trail.gameObject.transform.SetParent(staticTrails);
        transform.GetChild(1).transform.SetParent(staticTrails);

        if(recursive)
        StartCoroutine(NewTrail(transform, staticTrails.parent.GetChild(1)));
    }

    public IEnumerator NewTrail(Transform trailParent, Transform collisionParent)
    {   //instanziates new trail and trail collider, setting colors and parents.

        yield return new WaitForSecondsRealtime(0.5f);
        GameObject newTrail = Instantiate(_trailPrefab, trailParent);
        GameObject newColliders = Instantiate(_collisionPrefab, collisionParent);
        newTrail.GetComponent<TrailRenderer>().colorGradient = _color;
        _trail = newColliders.GetComponent<TrailCollision>();
        _trail.Expanding = true;
        _trail.TrailRenderer = transform.GetChild(1).GetComponent<TrailRenderer>();
        _trail.TrailRenderer.emitting = false;  //need to pause emitting to match the first collider position.
        _trail.StaticTrails = collisionParent.parent.GetChild(0).transform;

        StartCoroutine(NewEdgeCollider());  //start new trailCollider generation
        StartCoroutine(TrailCut(true));      //And queues next cut.
    }

    public void NewTrailFromArr(Vector2[] newPositions)
    {   //here we instantiate new edgecollider from the array.

        GameObject newTrail = Instantiate(_collisionPrefab, _trail.transform.parent.parent.GetChild(0));
        newTrail.name = "SplitCollider";
        newTrail.GetComponent<EdgeCollider2D>().points = newPositions;
    }

    private IEnumerator NewEdgeCollider()
    {   //We need an extra delay on the collider to avoid crashing into it as we move forward.
        yield return new WaitForSecondsRealtime(0.2f);
        _trail.TrailRenderer.emitting = true;
        StartCoroutine(UpdateTrail());
    }

    private IEnumerator UpdateTrail()
    {  //Recuring method calls on itself to keep updating a earlier position to keep the trail growing behind the player.
        if (_trail.Expanding)
        {
            Vector2 currentPoint = transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            _trail.AddPoint(currentPoint);
            StartCoroutine(UpdateTrail());
        }
        yield return null;
    }

    private IEnumerator DelayUpdate(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        StartCoroutine(UpdateTrail());      
    }
}
