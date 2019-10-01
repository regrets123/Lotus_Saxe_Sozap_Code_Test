using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string PlayerName { get; set; }
    public string Score { get; set; }

    private void Attack()
    {
        //Needs Cooldown, projectile, path (vector2),
    }

    private void Die ()
    {
        //Killer? need to check example game, multiplayer death?
    }

    private IEnumerable Invulnerability ()
    {
        //bool for no death? visual effect, heart indicatior.
        yield return new WaitForSeconds(2f); //check time in example game.
    }

    private void Update()
    {
            //check playerinput.
    }
}
