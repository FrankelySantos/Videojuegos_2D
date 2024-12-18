using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RockFall : MonoBehaviour
{
    public Transform player;
    public Transform rock;
    public float offset=0.1f;

    void Update()
    {
        //Get rigid body object of the rock
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>() ;

        //If player is near the rock, wake up the rigid body object
        if (player.position.x + offset >= rock.position.x)
        {
           rb.WakeUp();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If rock felled on the ground,
        // disable it's box collider
        if (collision.gameObject.tag.Equals("Ground"))
        {

            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
