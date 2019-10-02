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
    private TrailCollision trail;

    private void Start()
    {
        GameManager.Instance.AddPlayer(gameObject, index);  // need centralized references to the players since they are in different scenes you cant just drag them in the inspector.
        if (index == 2 || index == 3) gameObject.SetActive(false); //start wont trigger if the object isnt active, another solution would be to spawn from prefabs.
        StartCoroutine(UpdateTrail());
    }

    private IEnumerator UpdateTrail()
    {
        Vector2 currentPoint = transform.position;
        yield return new WaitForSecondsRealtime(0.05f);
        trail.AddPoint(currentPoint);
        StartCoroutine(UpdateTrail());
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

    private void CheckCollision()
    {   

    }

}
