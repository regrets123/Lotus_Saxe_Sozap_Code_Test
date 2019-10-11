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
    public float HitScore { get; set; }
    public Color TrailColor { get; set; }
    [SerializeField]
    private GameObject _projectilePrefab;

    [SerializeField]
    private Transform _startPos, _projectileSpawn;
    public Transform _playerHud;
    public int index;
    public bool AttackOnCooldown { get; set; }
    public bool PhaseOnCooldown { get; set; }
    private InputHandler _inputHandler;
    private CircleCollider2D[] colliderArray;


    private void TrackHudPosition()
    {   //tracks each players Hud to the ships position.
        _playerHud.position = transform.position;
    }

    private void Start()
    {
        colliderArray = GetComponents<CircleCollider2D>();
        GameManager.Instance.AddPlayer(gameObject, index);  // need centralized references to the players since they are in different scenes you cant just drag them in the inspector.
        AttackOnCooldown = false;
        PhaseOnCooldown = false;
        _inputHandler = gameObject.GetComponent<InputHandler>();
        TrailColor = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<TrailHandler>().AssignTrailColor(this);
        gameObject.GetComponent<TrailHandler>().player = this;
        if (index == 2 || index == 3) gameObject.SetActive(false); //start wont trigger if the object isnt active, another solution would be to spawn from prefabs.

    }

    private void Update()
    {
        Move();    //check playerinput.
        TrackHudPosition();
    }
    private void LateUpdate()
    {
        if(GameManager.Instance.gameState == GameManager.GameState.running && !Alive)
        {
            gameObject.SetActive(false);
        }
    }

    public void ToggleHud(bool toggleState)
    {
        if (toggleState)
        { _playerHud.gameObject.SetActive(true); }
        else
        { _playerHud.gameObject.SetActive(false); }
    }

    public IEnumerator Attack()
    {   //since we can only shoot straight ahead, the aim is simply position + rotation. 
        if (!AttackOnCooldown)
        { 
            _playerHud.GetChild(0).gameObject.SetActive(false);
            AttackOnCooldown = true;
            GameObject currentBullet = Instantiate(_projectilePrefab, GameManager.Instance.cleaner.transform.GetChild(0));
            currentBullet.transform.position = _projectileSpawn.position;
            currentBullet.transform.rotation = transform.rotation;
            currentBullet.GetComponent<Projectile>().Shoot(this);
            yield return new WaitForSeconds(6);
            AttackOnCooldown = false;
            _playerHud.GetChild(0).gameObject.SetActive(true);
        }
        yield return null;
    }

    private void Death()
    {
        for (int i = 0; i < 3; i++)
        {
            colliderArray[i].enabled = false;
        }

        ToggleHud(false);
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
        { Score = NewScore + HitScore; }
        gameObject.transform.position = _startPos.position;
        gameObject.transform.rotation = _startPos.rotation;
        gameObject.SetActive(true);
        _playerHud.GetChild(2).GetComponent<Text>().text = "0";
        HitScore = 0;
        Alive = true;
        AttackOnCooldown = false;
        PhaseOnCooldown = false;
        for (int i = 0; i < 3; i++)
        {
            colliderArray[i].enabled = true;
        }
        if (transform.childCount > 1) //remove old trails on players to spawn new. 
        { Destroy(transform.GetChild(1).gameObject); }
    }

    public IEnumerator Invulnerability()
    {   //turns of colliders of player so we cant be hit, 
        PhaseOnCooldown = true;
        CircleCollider2D[] temp = GetComponents<CircleCollider2D>();
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].enabled = false;
        }
        _playerHud.GetChild(1).gameObject.SetActive(false);

        StartCoroutine(Vulnurable(temp));
        yield return new WaitForSeconds(6f);
        PhaseOnCooldown = false;
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
        _inputHandler.ParseInput(xMoveSpeed,rotationSpeed,this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {  //check collision with walls and trails. 


        if (collision.gameObject.tag == "Trail" || collision.gameObject.tag == "Walls")
        {
            Debug.Log(PlayerName + "Crashed with " + collision.gameObject.name);
            TrailHandler handler = TrailManager.Instance.activeTrails[index].Handler;
            handler.Renderer.emitting = false;
            if (collision.gameObject.tag == "Trail")
            {
                TrailData tempdata = collision.gameObject.GetComponent<TrailCollision>().TrailData;
                tempdata.Collider.SplitPoint(transform, tempdata.Handler.transform);
                collision.gameObject.SetActive(false);
            }
            else
            {
                handler.NewAtColl();
            }
            Death();
        }

    }
}
