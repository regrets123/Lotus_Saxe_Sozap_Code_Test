using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string PlayerName { get; set; }
    public float Score { get; set; }
    [SerializeField]
    private int index;
    [SerializeField]
    private Transform playerHud;
    private float attackCooldown = 4;
    private float phaseOutCooldown = 6;


    private void TrackHudPosition()
    {   //tracks each players Hud to the ships position.
        playerHud.position = transform.position;
    }


    private void Start()
    {
        GameManager.Instance.AddPlayer(gameObject, index);  // need centralized references to the players since they are in different scenes you cant just drag them in the inspector.
        if (index == 2 || index == 3) gameObject.SetActive(false); //start wont trigger if the object isnt active, another solution would be to spawn from prefabs.
        PlayerName = GameManager.Instance.playerNames[index];
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
        //Needs Cooldown, projectile, path (vector2),
    }

    private void Die()
    {
        //Killer? need to check example game, multiplayer death?
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
                {
                    transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                }
                else
                {
                    transform.Rotate(0, 0, -(rotationSpeed * Time.deltaTime));
                }
            }
            transform.position += transform.right * Time.deltaTime * xMoveSpeed;

            if (Input.GetAxisRaw("Vertical") != 0) 
            {
                if (Input.GetAxisRaw("Horizontal") < 0.01)
                {
                    Attack();
                }
                else
                {
                    StartCoroutine(Invulnerability());
                }
            };

        }
        else if(GameManager.Instance.gameState == GameManager.GameState.load)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)  //this should work with gamepad and joysticks aswell.
            {
                if (Input.GetAxisRaw("Horizontal") < 0.01)
                {
                    transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                }
                else
                {
                    transform.Rotate(0, 0, -(rotationSpeed * Time.deltaTime));
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(PlayerName +"Crashed with "+ collision.gameObject.name);
        collision.gameObject.GetComponent<TrailCollision>().SplitPoint(transform);
        gameObject.SetActive(false);
        ToggleHud(false);
        GetComponent<TrailHandler>().TrailCut(false);
        //Destroy(collision.gameObject);

    }

}
