using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailHandler : MonoBehaviour
{   //handles switching between finished spawen trails and live trails still spawning behind players. 
    //All coroutines needs to be able to stop if we collide earlier than they end.

    public GameObject trailPrefab, collisionPrefab;
    [SerializeField]
    private TrailData trailData;
    private Color _color;
    public Player player { get; set; }
    public TrailRenderer Renderer { get; set; }

    public void AssignTrailColor(Player player)
    {  //saving the gradient for new trails
        _color = player.TrailColor;
    }

    public void StartSpawning()
    {   //call upon gamestart to time spawning with trail generation properly after countdown.
        if (GameManager.Instance.gameState == GameManager.GameState.running && player.Alive)
        {
           // StartCoroutine(StartDelay());
            NewTrail();
        }
    }

    private IEnumerator StartDelay()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.running && player.Alive)
        {
            yield return new WaitForSeconds(0.02f);
            StartCoroutine(UpdateTrail());
        }
        else
        { yield return null; }
        
    }

    private IEnumerator NextTrailDelay(Transform transform)
    {   //we need a delay between collider and trail renderer spawn for them to match visualy positions.
        if (GameManager.Instance.gameState == GameManager.GameState.running && player.Alive)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(StartDelay());
            NewTrail();
        }
        else
        { yield return null; }

    }

    public IEnumerator TrailCut()
    {   //faking a "cut" in the trail by creating a new trail and stopping the other trail.
        if (GameManager.Instance.gameState == GameManager.GameState.running && player.Alive)
        {

            float trailLength;
            trailLength = Random.Range(3, 5);

            yield return new WaitForSeconds(trailLength);
            trailData.Collider.Expanding = false;
            UpdateTrailParents();
            StartCoroutine(NextTrailDelay(GameManager.Instance.cleaner.transform.GetChild(0)));
        }
        else
        { yield return null; }
    }
    public void NewAtColl()
    {   //instant cut method for starting new trail if someone hit the player trail so it can be split instantly. 

        trailData.Collider.Expanding = false;
        Renderer.emitting = false;
        UpdateTrailParents();
        NewTrail();

    }

    private void UpdateTrailParents()
    {   //moving them to a empty object to keep the trails sorted for removing after round.
        TrailManager.Instance.staticTrails.Add(TrailManager.Instance.activeTrails[player.index]);

        Transform staticTrails = GameManager.Instance.cleaner.transform.GetChild(0);
        trailData.Collider.gameObject.transform.SetParent(staticTrails);
        Renderer.transform.SetParent(staticTrails);
        
    }

    public void NewTrail()
    {   //instanziates new trail and trail collider, setting colors and parents.
        if(GameManager.Instance.gameState == GameManager.GameState.running && player.Alive)
        {
            GameObject newTrail = Instantiate(trailPrefab, transform);   //renderer
            GameObject newColliders = Instantiate(collisionPrefab, GameManager.Instance.cleaner.transform.GetChild(0));  //collider
            TrailData data = new TrailData(this,newColliders.GetComponent<TrailCollision>());   //saving both to a referencepoint.
            TrailManager.Instance.activeTrails[gameObject.GetComponent<Player>().index] = data;
            trailData = data;
            data.Collider.TrailData = data;

            Renderer = newTrail.GetComponent<TrailRenderer>();
            Renderer.startColor = _color;
            Renderer.endColor = _color;
            data.Collider.Expanding = true;
            Renderer.emitting = true;
            StartCoroutine(NewEdgeCollider());  //start new trailCollider generation
            StartCoroutine(TrailCut());      //And queues next cut.
        }
    }


    public void NewTrailFromArr(Vector2[] newPositions)
    {   //here we instantiate new edgecollider from the array.

        GameObject newTrail = Instantiate(collisionPrefab, GameManager.Instance.cleaner.transform.GetChild(0));
        newTrail.name = "SplitCollider";
        newTrail.GetComponent<EdgeCollider2D>().points = newPositions;
    }

    private IEnumerator NewEdgeCollider()
    {   //We need an extra delay on the collider to avoid crashing into it as we move forward.
        if(GameManager.Instance.gameState == GameManager.GameState.running)
        {
            yield return new WaitForSeconds(0.1f);
            Renderer.emitting = true;
            StartCoroutine(UpdateTrail());
        }
        else { yield return null; };
    }

    private IEnumerator UpdateTrail()
    {  //Recuring method calls on itself to keep updating a earlier position to keep the trail growing behind the player.
        if (trailData.Collider.Expanding && GameManager.Instance.gameState == GameManager.GameState.running)
        {
            Vector2 currentPoint = transform.position;
            yield return new WaitForSeconds(0.03f);
            trailData.Collider.AddPoint(currentPoint);
            StartCoroutine(UpdateTrail());
        }
        else { yield return null; };
        
    }

}
