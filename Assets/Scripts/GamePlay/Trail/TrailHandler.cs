using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailHandler : MonoBehaviour
{   //handles switching between finished spawen trails and live trails still spawning behind players.

    [SerializeField]
    public Player player { get; set; }
    public TrailRenderer Renderer { get; set; }
    public GameObject trailPrefab, collisionPrefab;
    public TrailData trailData;
    private Color _color;

    public void AssignTrailColor(Player player)
    {  //saving the gradient for new trails
        _color = player.TrailColor;
    }

    public void StartSpawning()
    {   //call upon gamestart to time spawning with trail generation properly after countdown.
        if (GameManager.Instance.gameState == GameManager.GameState.running && player.Alive)
        {
            //if (_routines == null) //since we need to be able to interrupt current running routines at collision, we store the routines. 
            //{
            //    _routines = new List<Coroutine>();
            //}
            StartCoroutine(StartDelay());
            //_routines.Add(co);
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
            yield return new WaitForSeconds(0.5f);
            Coroutine co = StartCoroutine(StartDelay());
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
            trailData.TrailCollider.Expanding = false;
            UpdateTrailParents();
            Coroutine co = StartCoroutine(NextTrailDelay(GameManager.Instance.cleaner.transform.GetChild(0)));
        }
        else
        { yield return null; }
    }
    public void NewAtColl()
    {   //instant cut method for starting new trail if someone hit the player trail so it can be split instantly. Needs to stop all running coroutines since we will start a new set of them.

        StopCoroutines();
        trailData.TrailCollider.Expanding = false;
        Renderer.emitting = false;
        UpdateTrailParents();
        NewTrail();

    }

    private void UpdateTrailParents()
    {   //moving them to a empty object to keep the trails sorted for removing after round.
        TrailManager.Instance.staticTrails.Add(TrailManager.Instance.activeTrails[player.index]);

        Transform staticTrails = GameManager.Instance.cleaner.transform.GetChild(0);
        trailData.TrailCollider.transform.SetParent(staticTrails);
        Renderer.transform.SetParent(staticTrails);
        
    }

    public void NewTrail()
    {   //instanziates new trail and trail collider, setting colors and parents.
        if(GameManager.Instance.gameState == GameManager.GameState.running && player.Alive)
        {
            GameObject newTrail = Instantiate(trailPrefab, transform);   //renderer
            GameObject newColliders = Instantiate(collisionPrefab, GameManager.Instance.cleaner.transform.GetChild(1));  //collider
            Renderer = newTrail.GetComponent<TrailRenderer>();
            TrailData data = new TrailData(this,newColliders.GetComponent<TrailCollision>(), Renderer);   //saving both to a referencepoint.
            TrailManager.Instance.activeTrails[gameObject.GetComponent<Player>().index] = data;
            trailData = data;
            data.TrailCollider.TrailData = data;

            
            Renderer.startColor = _color;
            Renderer.endColor = _color;
            data.TrailCollider.Expanding = true;
            Renderer.emitting = true;

            Coroutine co = StartCoroutine(UpdateTrail());  //start new trailCollider generation
            Coroutine co1 = StartCoroutine(TrailCut());      //And queues next cut.
        }
    }

    public void StopCoroutines()
    {   //called on upon collision of a trail thats still attatched to a player, to avoid several sets of methods running and starting splits on the same trail. Also used at end of round.        
        StopAllCoroutines();
    }

    public void NewTrailFromArr(Vector2[] newPositions,TrailData originalTrailOwnerData)
    {   //here we instantiate new edgecollider from the array.

        GameObject newTrail = Instantiate(collisionPrefab, GameManager.Instance.cleaner.transform.GetChild(0));
        newTrail.name = "SplitCollider";
        newTrail.GetComponent<EdgeCollider2D>().points = newPositions;
        TrailData tempdata = new TrailData(originalTrailOwnerData.Handler, newTrail.GetComponent<TrailCollision>(), originalTrailOwnerData.VisualTrail);
        tempdata.beforeSplitTrail = originalTrailOwnerData;
        newTrail.GetComponent<TrailCollision>().TrailData = tempdata;

    }

    private IEnumerator UpdateTrail()
    {  //Recuring method calls on itself to keep updating a earlier position to keep the trail growing behind the player.
        if (trailData.TrailCollider.Expanding && GameManager.Instance.gameState == GameManager.GameState.running)
        {
            Vector2 currentPoint = transform.position;
            yield return new WaitForSeconds(0.03f);
            trailData.TrailCollider.AddPoint(currentPoint);
            StartCoroutine(UpdateTrail());  //Coroutine co =    Might wanna remove these, cuz there will be alot of them
            //routines.Add(co);               
        }
        else { yield return null; };
        
    }

}
