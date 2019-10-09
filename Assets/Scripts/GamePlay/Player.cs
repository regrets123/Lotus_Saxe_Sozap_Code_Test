using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string PlayerName { get; set; }
    public float NewScore { get; set; }
    public float Score { get; set; }
    public bool Alive { get; set; }
    public Color TrailColor { get; set; }
    public Gradient TrailGradient { get; set; }
    [SerializeField]
    private int index;
    [SerializeField]
    private Transform playerHud, startPos;
    private float attackCooldown = 4;
    private float phaseOutCooldown = 6;


    private void TrackHudPosition()
    {   //tracks each players Hud to the ships position.
        playerHud.position = transform.position;
    }

    private void Start()
    {
        GameManager.Instance.AddPlayer(gameObject, index);  // need centralized references to the players since they are in different scenes you cant just drag them in the inspector.
        TrailColor = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
        TrailGradient = transform.GetChild(1).GetComponent<TrailRenderer>().colorGradient;
        gameObject.GetComponent<TrailHandler>().AssignTrailColor(this);
        if (index == 2 || index == 3) gameObject.SetActive(false); //start wont trigger if the object isnt active, another solution would be to spawn from prefabs.

    }

    private void Update()
    {
        Move();    //check playerinput.
        TrackHudPosition();
    }

    public void ToggleHud(bool toggleState)
    {
        if (toggleState)
        { playerHud.gameObject.SetActive(true); }
        else
        { playerHud.gameObject.SetActive(false); }
    }

    private void Attack()
    {
        Debug.Log("Pewpew shooting sadface!");

        //add points to score, not new score.
        //Needs Cooldown, projectile, path (vector2),
    }

    private void Death()
    {
        gameObject.SetActive(false);
        ToggleHud(false);
        GetComponent<TrailHandler>().TrailCut(false);
        Alive = false;
        GameManager.Instance.CheckWin = true;
        
    }

    public void ResetPlayer(bool keepScore)
    {   //reset stats for next round/game, right b4 game starts.

        if(!keepScore)
        {
            Score = 0;
            NewScore = 0;
        }
        else
        {
            Score = NewScore;
        }
        gameObject.transform.position = startPos.position;
        gameObject.SetActive(true);
        Alive = true;        
        attackCooldown = 4;
        phaseOutCooldown = 6;
        gameObject.GetComponent<TrailHandler>().RestartTrail();
        gameObject.GetComponent<TrailHandler>().StartSpawning();
    }

    private IEnumerator Invulnerability()
    {
        //bool for no death? visual effect, heart indicatior.
        Debug.Log("Cant hurt me now!!!");
        yield return new WaitForSeconds(1.5f); //check time in example game.
    }

    private void Move()
    {   //here we check all input and move/act player accordingly.

        float xMoveSpeed = 3f;
        float rotationSpeed = 150f;
        if (GameManager.Instance.gameState == GameManager.GameState.running)
        {         
            if (Input.GetAxisRaw("Horizontal") != 0)  //this should work with gamepad and joysticks aswell.
            {
                if (Input.GetAxisRaw("Horizontal") < 0.1)
                { transform.Rotate(0, 0, rotationSpeed * Time.deltaTime); }
                else
                { transform.Rotate(0, 0, -(rotationSpeed * Time.deltaTime)); }
            }
            transform.position += transform.right * Time.deltaTime * xMoveSpeed;
            if (Input.GetAxisRaw("Vertical") != 0) 
            {
                if (Input.GetAxisRaw("Horizontal") < 0.01)
                { Attack(); }
                else
                { StartCoroutine(Invulnerability()); }
            };
        }
        else if(GameManager.Instance.gameState == GameManager.GameState.load)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)  //this should work with gamepad and joysticks aswell.
            {
                if (Input.GetAxisRaw("Horizontal") < 0.01)
                { transform.Rotate(0, 0, rotationSpeed * Time.deltaTime); }
                else
                { transform.Rotate(0, 0, -(rotationSpeed * Time.deltaTime)); }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   //Checks collision between ships and different objects. In the test example, it doesnt check collision vs other ships, so it feels random who gets hit by the enemy tail first. If ships go exactly straight into and die at the same time, the test game gets stuck.
        //TODO Solution, kill both players, then matches can end in draw and no one should get points? Both Death method needs to resolve befor win check, else the player with first index wins. Buffer them? Or delay?       
        
        Debug.Log(PlayerName +"Crashed with "+ collision.gameObject.name);
        if(collision.gameObject.tag == "Tails")
        { collision.gameObject.GetComponent<TrailCollision>().SplitPoint(transform); }
        collision.gameObject.SetActive(false);
        Death();

    }

}
