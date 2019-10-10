using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailHandler : MonoBehaviour
{   //handles switching between finished spawen trails and live trails still spawning behind players. 

    public GameObject trailPrefab, collisionPrefab;
    [SerializeField]
    private TrailCollision _trail;
    private Color _color;
    public Player player { get; set; }

    public void AssignTrailColor(Player player)
    {  //saving the gradient for new trails
        _color = player.TrailColor;
    }

    public void StartSpawning()
    {   //call upon gamestart to time spawning with trail generation properly after countdown.
        StartCoroutine(StartDelay());
        NewTrail(transform,GameManager.Instance.cleaner.transform.GetChild(1));
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.02f);
        StartCoroutine(UpdateTrail());
    }

    private IEnumerator NextTrailDelay(Transform transform)
    {
        yield return new WaitForSeconds(1f);
        NewTrail(transform, transform);
    }

    public IEnumerator TrailCut()
    {   //faking a "cut" in the trail by creating a new trail and stopping the other trail.

        float trailLength;

        //if (recursive)
        trailLength = Random.Range(3, 5); 
        //else
        //{ trailLength = 0; }
        ////StartCoroutine(StopEmitting(trailLength));
        //transform.GetChild(1).GetComponent<TrailRenderer>().emitting = false;

        yield return new WaitForSeconds(trailLength);
        _trail.Expanding = false;
        UpdateTrailParents();
        
        if(GameManager.Instance.gameState == GameManager.GameState.running)
            StartCoroutine(NextTrailDelay(GameManager.Instance.cleaner.transform.GetChild(0)));

    }

    private void UpdateTrailParents()
    {   //moving them to a empty object to keep the trails sorted for removing after round.
        Transform staticTrails = GameManager.Instance.cleaner.transform.GetChild(0); 
        _trail.gameObject.transform.SetParent(staticTrails);
        transform.GetChild(1).transform.SetParent(staticTrails);
    }

    public void NewTrail(Transform trailParent, Transform collisionParent)
    {   //instanziates new trail and trail collider, setting colors and parents.
        if(GameManager.Instance.gameState == GameManager.GameState.running)
        {
            GameObject newTrail = Instantiate(trailPrefab, transform);
            GameObject newColliders = Instantiate(collisionPrefab, collisionParent);
            newTrail.GetComponent<TrailRenderer>().startColor = _color;
            newTrail.GetComponent<TrailRenderer>().endColor = _color;
            _trail = newColliders.GetComponent<TrailCollision>();
            _trail.TrailRenderer = newTrail.GetComponent<TrailRenderer>();
            _trail.StaticTrails = collisionParent.parent.GetChild(0).transform;
            _trail.Expanding = true;
            _trail.TrailRenderer.emitting = true;
            StartCoroutine(NewEdgeCollider());  //start new trailCollider generation
            StartCoroutine(TrailCut());      //And queues next cut.
        }
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

}
