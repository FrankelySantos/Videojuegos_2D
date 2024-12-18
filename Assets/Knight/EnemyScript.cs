using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Animator animator;
    public PolygonCollider2D PolygonCollider2D;
    public AudioSource enemyDeath;

    void Update()
    {
        // If dying animation is playing
        if (GetCurrentClipName().Equals("death"))
        {
            animator.SetBool("killed",false);
        }

        // If dying animation finished and enemys is now dead
        if (GetCurrentClipName().Equals("dead"))
        {
           Destroy(gameObject);
        }
    }

    // public function used when player kill this enemy
   public void kill()
    {
        Debug.Log("Attacked");
        Destroy(PolygonCollider2D);
        animator.SetBool("killed", true);
        enemyDeath.Play();
    }

    // Gets current animation state of the enemy object
    public string GetCurrentClipName()
    {
        int layerIndex = 0;
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }
}
