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
    private CircleCollider2D hitBox;


    private void Start()
    {
        hitBox = GetComponent<CircleCollider2D>();
        GameManager.Instance.AddPlayer(gameObject, index);  // need centralized references to the players since they are in different scenes you cant just drag them in the inspector.
        if (index == 2 || index == 3) gameObject.SetActive(false); //start wont trigger if the object isnt active, another solution would be to spawn from prefabs.
        PlayerName = GameManager.Instance.playerNames[index];
    }

    private void Update()
    {
        Move();
        //check playerinput.
    }


    private void Attack()
    {
        //Needs Cooldown, projectile, path (vector2),
    }

    private void Die()
    {
        //Killer? need to check example game, multiplayer death?
    }

    private IEnumerable Invulnerability()
    {
        //bool for no death? visual effect, heart indicatior.
        yield return new WaitForSeconds(2f); //check time in example game.
    }

    private void Move()
    {
        float xMoveSpeed = 3f;
        float rotationSpeed = 100f;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(PlayerName +"Crashed with "+ collision.gameObject.name);
        collision.gameObject.GetComponent<TrailCollision>().SplitPoint(transform);
        hitBox.enabled = false;
        GetComponent<TrailHandler>().TrailCut(false);
        //Destroy(collision.gameObject);

    }

}
