using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    private Player _owner;
    private bool _alive;

    private void Start()
    {
        _alive = true;
    }
    public void Shoot(Player shooter)
    {   //call on instantiation to keep reference to owner so it can track who gets the points if it hits a player.
        _owner = shooter;
        transform.rotation = shooter.transform.rotation;
    }

    private void Update()
    {
        transform.position += transform.right * Time.deltaTime * 6f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   //only give points if hitting player. then remove it.
        if (collision.gameObject.tag == "Player" && _alive || collision.gameObject.tag =="Walls" && _alive || collision.gameObject.tag == "Trail" && _alive)
        {
            _alive = false;
            _owner.OnProjectileHit();
            Destroy(gameObject);
        }
        
    }
}
