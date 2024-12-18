using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSounds : MonoBehaviour
{
    public AudioSource hit;
    public AudioSource Attack;
    public AudioSource walk;
    public AudioSource die;
    public Animator animator;
    private bool isDead=false;
    private string previousState="";
    void Update()
    {
        //If current skeleton animation state is walking,
        //play walking sound if it isn't already playing
        if (GetCurrentClipName().Equals("Walk"))
        {   
            if (!walk.isPlaying)
            {
                walk.Play();
                previousState = "Walk";
            }
        }
        //If current skeleton animation state is walking,
        //play walking sound if it isn't already playing
        else if (GetCurrentClipName().Equals("Attack"))
        {
            // Check previous state to avoid repetition of sound
            // in the same on attack
            if (!Attack.isPlaying && previousState != "Attack"){
                Attack.Play();
                previousState = "Attack";
            }
        }

        //If current skeleton animation state is taking hit,
        //play taking hit sound if it isn't already playing
        else if (GetCurrentClipName().Equals("TakenHit"))
        { 
            if(!hit.isPlaying) {
                hit.Play();
                previousState = "TakenHit";
            }
        }
        //If current skeleton state is dead then play death sound
        else if (GetCurrentClipName().Equals("Dead")|| GetCurrentClipName().Equals("stillDead"))
        {  
            if(!die.isPlaying && !isDead)
            {
                die.Play();
                isDead = true;
            }
        }

    }

    // This function gets current animation state of the skeleton
    string GetCurrentClipName()
    {
        int layerIndex = 0;
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }
}
