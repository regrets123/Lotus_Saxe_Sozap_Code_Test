using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{   //Handles all player logic and variables

    public Color TrailColor { get; set; }
    public string PlayerName { get; set; }
    public bool Alive, AttackOnCooldown, PhaseOnCooldown;
    public float NewScore, Score, HitScore;
    public int index;
    public List<Coroutine> coroutines;

    [SerializeField]
    private Transform _startPos, _projectileSpawn, _projectilePrefab, _playerHud;
    [SerializeField]
    private Animator _animator;
    private InputHandler _inputHandler;
    private CircleCollider2D[] _colliderArray;


    private void TrackHudPosition()
    {   //tracks each players Hud to the ships position.
        _playerHud.position = transform.position;
    }

    private void Start()
    {
        coroutines = new List<Coroutine>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _colliderArray = GetComponents<CircleCollider2D>();
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
        if (GameManager.Instance.gameState == GameManager.GameState.running && !Alive)
        {
            gameObject.SetActive(false);
        }
    }

    public void ToggleHud(bool toggleState)
    {
        _playerHud.gameObject.SetActive(toggleState);
        _playerHud.gameObject.SetActive(toggleState);
    }

    public IEnumerator Attack()
    {   //since we can only shoot straight ahead, the aim is simply position + rotation. 
        if (!AttackOnCooldown)
        {
            _playerHud.GetChild(0).gameObject.SetActive(false);
            AttackOnCooldown = true;
            GameObject currentBullet = Instantiate(_projectilePrefab.gameObject, GameManager.Instance.cleaner.transform.GetChild(0));
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
            _colliderArray[i].enabled = false;
        }
        ToggleHud(false);
        Alive = false;
        GameManager.Instance.CheckWin = true;
    }

    public void StopPlayerCoroutines()
    {   //seems they have to be stopped on the level of monobehaviour they started at, no matter where they are stored. (didnt work if we stopped coroutines at player.StopAllCoroutines)
        _inputHandler.StopAllCoroutines();
        coroutines.Clear();
    }

    public void OnProjectileHit()
    {   //called on when a player hits another player with a Projectile.
        HitScore += 10;
        _playerHud.GetChild(2).GetComponent<Text>().text = Convert.ToString(HitScore);
    }


    public void ToggleAbilityHud(bool toggleState)
    {
        _playerHud.GetChild(0).gameObject.SetActive(toggleState);
        _playerHud.GetChild(1).gameObject.SetActive(toggleState);
    }

    public void UpdatePlayerScore()
    {
        NewScore += HitScore +Score; 
    }

    public void ResetPlayer(bool keepScore)
    {   //reset stats for next round/game, right b4 game starts.

        if (!keepScore)
        {
            Score = 0;
            NewScore = 0;
            HitScore = 0;
        }
        else
        { Score = NewScore; } 
        transform.position = _startPos.position;
        transform.rotation = _startPos.rotation;
        gameObject.SetActive(true);
        _playerHud.GetChild(2).GetComponent<Text>().text = "0";
        HitScore = 0;
        NewScore = 0;
        Alive = true;
        AttackOnCooldown = false;
        PhaseOnCooldown = false;
        for (int i = 0; i < 3; i++)
        {
            _colliderArray[i].enabled = true;
        }
        if (transform.childCount > 1) //remove old trails on players to spawn new. 
        { Destroy(transform.GetChild(1).gameObject); }
    }

    public IEnumerator Invulnerability()
    {   //turns of colliders of player so we cant be hit and triggers visual animation.
        _animator.SetTrigger("Invulnurability");
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
        _inputHandler.ParseInput(xMoveSpeed, rotationSpeed, this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {  //check collision with walls and trails. 

        if (collision.gameObject.tag == "Trail" && Alive == true || collision.gameObject.tag == "Walls" && Alive == true)
        {
            TrailHandler handler = TrailManager.Instance.activeTrails[index].Handler;
            handler.Renderer.emitting = false;
            if (collision.gameObject.tag == "Trail")
            {
                TrailData tempdata = collision.gameObject.GetComponent<TrailCollision>().TrailData;
                if(tempdata.beforeSplitTrail == null)
                {
                    tempdata.TrailCollider.SplitPoint(transform, tempdata.Handler.transform);
                }
                else
                {
                    tempdata.TrailCollider.SubSplit(tempdata.beforeSplitTrail,transform);
                }

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
