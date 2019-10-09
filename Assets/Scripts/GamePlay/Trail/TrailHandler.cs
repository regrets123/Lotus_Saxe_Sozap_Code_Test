using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailHandler : MonoBehaviour
{   //handles switching between finished spawen trails and live trails still spawning behind players. 

    public GameObject trailPrefab, collisionPrefab, trailMeshPrefab;
    [SerializeField]
    private TrailCollision _trail;
    private Gradient _color;
    public Player player { get; set; }
    public Mesh tempMesh;

    public void AssignTrailColor(Player player)
    {  //saving the gradient for new trails
        _color = player.TrailGradient;
    }

    public void RestartTrail()
    {   //makes sure all references are set b4 next game 

        if (_trail.TrailRenderer == null)
        { _trail.TrailRenderer = transform.GetChild(1).GetComponent<TrailRenderer>(); };

    }

    public void StartSpawning()
    {   //call upon gamestart to time spawning with trail generation properly after countdown.
        StartCoroutine(StartDelay());
        StartCoroutine(TrailCut(true));
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.02f);
        StartCoroutine(UpdateTrail());
    }

    private IEnumerator StopEmitting(float time)
    {   //Modifying method to match end of trail to match colliders.
        yield return new WaitForSeconds(time -0.01f);
        transform.GetChild(1).GetComponent<TrailRenderer>().emitting = false;
    }

    public IEnumerator TrailCut(bool recursive)
    {   //faking a "cut" in the trail by creating a new trail and stopping the other trail.

        float trailLength;

        if (recursive)
        { trailLength = Random.Range(3, 5); }
        else
        { trailLength = 0; }       
        StartCoroutine(StopEmitting(trailLength));

        yield return new WaitForSeconds(trailLength);
        _trail.Expanding = false;
        Transform staticTrails = _trail.transform.parent.parent.GetChild(0); //moving them to a empty object to keep them sorted.
        _trail.gameObject.transform.SetParent(staticTrails);
        transform.GetChild(1).transform.SetParent(staticTrails);

        if (recursive)
        StartCoroutine(NewTrail(transform, staticTrails.parent.GetChild(1)));
    }

    public IEnumerator NewTrail(Transform trailParent, Transform collisionParent)
    {   //instanziates new trail and trail collider, setting colors and parents.
        if(GameManager.Instance.gameState == GameManager.GameState.running)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            GameObject newTrail = Instantiate(trailPrefab, trailParent);
            GameObject newColliders = Instantiate(collisionPrefab, collisionParent);
            newTrail.GetComponent<TrailRenderer>().colorGradient = _color;
            _trail = newColliders.GetComponent<TrailCollision>();
            _trail.Expanding = true;
            _trail.TrailRenderer = transform.GetChild(1).GetComponent<TrailRenderer>();
            _trail.TrailRenderer.emitting = false;  //need to pause emitting to match the first collider position.
            _trail.StaticTrails = collisionParent.parent.GetChild(0).transform;

            StartCoroutine(NewEdgeCollider());  //start new trailCollider generation
            StartCoroutine(TrailCut(true));      //And queues next cut.
        }
        else { yield return null; }

    }

    public void NewTrailFromArr(Vector2[] newPositions)
    {   //here we instantiate new edgecollider from the array.

        GameObject newTrail = Instantiate(collisionPrefab, _trail.transform.parent.parent.GetChild(0));
        newTrail.name = "SplitCollider";
        newTrail.GetComponent<EdgeCollider2D>().points = newPositions;
    }

    private IEnumerator NewEdgeCollider()
    {   //We need an extra delay on the collider to avoid crashing into it as we move forward.
        if(GameManager.Instance.gameState == GameManager.GameState.running)
        {
            yield return new WaitForSeconds(0.1f);
            _trail.TrailRenderer.emitting = true;
            StartCoroutine(UpdateTrail());
        }
        else { yield return null; };
    }

    private IEnumerator UpdateTrail()
    {  //Recuring method calls on itself to keep updating a earlier position to keep the trail growing behind the player.
        if (_trail.Expanding && GameManager.Instance.gameState == GameManager.GameState.running)
        {
            Vector2 currentPoint = transform.position;
            yield return new WaitForSeconds(0.03f);
            _trail.AddPoint(currentPoint);
            StartCoroutine(UpdateTrail());
        }
        else { yield return null; };
        
    }

    private IEnumerator DelayUpdate(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(UpdateTrail());      
    }
}
