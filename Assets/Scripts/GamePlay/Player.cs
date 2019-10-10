using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string PlayerName { get; set; }
    public float NewScore { get; set; }
    public float Score { get; set; }
    public bool Alive { get; set; }
    public Color TrailColor { get; set; }
    [SerializeField]
    private GameObject _projectilePrefab;
    [SerializeField]
    private int _index;
    [SerializeField]
    private Transform _startPos, _projectileSpawn;
    public Transform _playerHud;
    private bool _attackOnCooldown = false;
    private bool _phaseOnCooldown = false;


    private void TrackHudPosition()
    {   //tracks each players Hud to the ships position.
        _playerHud.position = transform.position;
    }

    private void Start()
    {
        GameManager.Instance.AddPlayer(gameObject, _index);  // need centralized references to the players since they are in different scenes you cant just drag them in the inspector.
        TrailColor = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<TrailHandler>().AssignTrailColor(this);
        if (_index == 2 || _index == 3) gameObject.SetActive(false); //start wont trigger if the object isnt active, another solution would be to spawn from prefabs.

    }

    private void Update()
    {
        Move();    //check playerinput.
        TrackHudPosition();
    }

    public void ToggleHud(bool toggleState)
    {
        if (toggleState)
        { _playerHud.gameObject.SetActive(true); }
        else
        { _playerHud.gameObject.SetActive(false); }
    }

    private IEnumerator Attack()
    {   //since we can only shoot straight ahead, the aim is simply position + rotation. 
        if (!_attackOnCooldown)
        {
            _playerHud.GetChild(0).gameObject.SetActive(false);
            _attackOnCooldown = true;
            GameObject currentBullet = Instantiate(_projectilePrefab, GameManager.Instance.cleaner.transform.GetChild(0));
            currentBullet.transform.position = _projectileSpawn.position;
            currentBullet.transform.rotation = transform.rotation;
            currentBullet.GetComponent<Projectile>().Shoot(this);
            yield return new WaitForSeconds(6);
            _attackOnCooldown = false;
            _playerHud.GetChild(0).gameObject.SetActive(true);
        }
        yield return null;
    }

    private void Death()
    {
        gameObject.SetActive(false);
        ToggleHud(false);
        GetComponent<TrailHandler>().TrailCut();
        Alive = false;
        GameManager.Instance.CheckWin = true;

    }

    public void ResetPlayer(bool keepScore)
    {   //reset stats for next round/game, right b4 game starts.

        if (!keepScore)
        {
            Score = 0;
            NewScore = 0;
        }
        else
        { Score = NewScore; }
        gameObject.transform.position = _startPos.position;
        gameObject.transform.rotation = _startPos.rotation;
        gameObject.SetActive(true);
        Alive = true;
        _attackOnCooldown = false;
        _phaseOnCooldown = false;
        if (transform.childCount > 1) //remove old trails on players to spawn new. 
        { Destroy(transform.GetChild(1).gameObject); }
    }

    private IEnumerator Invulnerability()
    {   //turns of colliders of player so we cant be hit, 
        _phaseOnCooldown = true;
        CircleCollider2D[] temp = GetComponents<CircleCollider2D>();
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].enabled = false;
        }
        _playerHud.GetChild(1).gameObject.SetActive(false);

        StartCoroutine(Vulnurable(temp));
        yield return new WaitForSeconds(6f);
        _phaseOnCooldown = false;
        _playerHud.GetChild(1).gameObject.SetActive(true);
    }

    private IEnumerator Vulnurable(CircleCollider2D[] colliders)
    {   //turns on the colliders and starts animation.
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }

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
                { if (_attackOnCooldown == false) StartCoroutine(Attack()); }
                else
                { if (_phaseOnCooldown == false) StartCoroutine(Invulnerability()); }
            };
        }
        else if (GameManager.Instance.gameState == GameManager.GameState.load)
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
    {  //check collision with walls and trails 

        Debug.Log(PlayerName + "Crashed with " + collision.gameObject.name);
        if (collision.gameObject.tag == "Tails")
        {
            collision.gameObject.GetComponent<TrailCollision>().SplitPoint(transform);
            collision.gameObject.SetActive(false);
            Death();
        }
        if (collision.gameObject.tag == "Walls") Death();


    }
}
