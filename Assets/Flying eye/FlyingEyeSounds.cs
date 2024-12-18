using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeSounds : MonoBehaviour
{
    public AudioSource hit;
    public AudioSource attack;
    public AudioSource fly;
    public AudioSource die;
    public Animator animator;
    private bool isDead = false;
    private string prevouseState;
    public Transform player;
    public float detectionRange = 5f;

    // Update is called once per frame
    void Update()
    {

        // Check if the player is within detection range
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            if (GetCurrentClipName().Equals("Flight"))
            {
                if (!fly.isPlaying)
                {
                    fly.Play();
                    prevouseState = "Flight";
                }
            }
            else if (GetCurrentClipName().Equals("Attack"))
            {
                if (!attack.isPlaying && prevouseState != "Attack")
                {
                    attack.Play();
                    prevouseState = "Attack";
                }
            }
            else if (GetCurrentClipName().Equals("TakeHit"))
            {

                if (!hit.isPlaying)
                {
                    hit.Play();
                    prevouseState = "Hit";
                }
            }
            else if (GetCurrentClipName().Equals("dead") || GetCurrentClipName().Equals("death"))
            {
                if (!die.isPlaying && !isDead)
                {
                    die.Play();
                    isDead = true;
                }
            }
        }
    }
    string GetCurrentClipName()
    {
        int layerIndex = 0;
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }
}
