using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    EnemyScript EnemyScript;
    public Rigidbody2D player1;
    public Animator animator;
    public int speed = 100;
    public float repelForce = 20;
    public int jumpforce = 3;
    public GameObject healthBar;
    public Transform respawnPostion;
    private float x_input;
    public AudioSource walkGrassAudio;
    public AudioSource damagedAudio;
    public AudioSource loseAudio;
    public AudioSource attackAudio;
    public AudioSource winAudio;
    public AudioSource jumpAudio;
    public AudioSource landAudio;
    public AudioSource healAudio;


    private bool isjumping;

    private void FixedUpdate()
    {
        // Set the velocity and direction of the player
        x_input = Input.GetAxisRaw("Horizontal");
        player1.velocity = new Vector2(x_input * speed * Time.deltaTime, player1.velocity.y);
    }
    void Update()
    {
        // Check if player finished jumping up and starts to fall on Ground
        if (!(player1.velocity.y > 0.0001) && isjumping)
        {
            animator.SetBool("jump", false);
            animator.SetBool("fall", true);
        }

        // Check if player already falled on the ground
        else if (player1.velocity.y > 0.0001 && !isjumping)
        {
            animator.SetBool("fall", true);
        }

        // Start Attack animation when player press 'J'
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetBool("attack",true);

            // Play attack audio if it isn't already playing
            if (!attackAudio.isPlaying)
            {
                attackAudio.Play();
            }
        }
        else
        {
            // Release player from attack state
            animator.SetBool("attack", false);
        }
        
           // Check if player is dying
           if(GetCurrentClipName().Equals("death"))
            {

            //Release from the death state so it can go to the final state
            animator.SetBool("killed",false);

            // Lock Movement
            player1.velocity=new Vector2(0,0);
            player1.freezeRotation = true;
            jumpforce = 0;
            player1.constraints = RigidbodyConstraints2D.FreezePositionX ;
            }

        // Jump when user press 'W' or Space of up arrow
        // if playerisn't already jumping
        if ((Input.GetKeyDown(KeyCode.W)
            ||Input.GetKeyDown(KeyCode.UpArrow)
            ||Input.GetKeyDown(KeyCode.Space))
            &&!isjumping)
        {    
            jumpAudio.Play();
            player1.velocity = Vector2.up * jumpforce;
            isjumping = true;
            animator.SetBool("jump", true);
        }
        else animator.SetBool("jump", false);


        // Move RIght when player press "D" of Right arrow
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //Rotate player to the Right
            transform.rotation = Quaternion.Euler(0, 0, 0);

            //If player on the ground and the soud isn't already playing, play it
            if (!walkGrassAudio.isPlaying && !animator.GetBool("fall") && !animator.GetBool("jump"))
                walkGrassAudio.Play();

            //Transfrom animation state into run
            animator.SetBool("run", true);
        }

        // Move Left when player press "A" of Left arrow
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            animator.SetBool("run", true);
            if(!walkGrassAudio.isPlaying && !animator.GetBool("fall") && !animator.GetBool("jump"))
                walkGrassAudio.Play();
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else animator.SetBool("run", false);


        // Check if the player object reached it's finall state and losing audio finished
        if (GetCurrentClipName().Equals("dead") && !loseAudio.isPlaying)
        {
            // Load game over screen and destroy this script
            SceneManager.LoadSceneAsync(3);
            Destroy(this, 0.1f);
        }
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // If player felled change animation state
        if(collision.gameObject.tag.Equals("Ground")|| collision.gameObject.tag.Equals("Wall"))
        {
            isjumping = false;
            if (collision.gameObject.tag.Equals("Ground"))
            {
                animator.SetBool("fall", false);
                animator.SetBool("jump", false);
                landAudio.Play();
            }
        }

        // If player collided with enemy without attacking
        if (collision.gameObject.tag.Equals("Enemy")){
            if (!GetCurrentClipName().Contains("attack"))
            {
                // Calculate repelling direction
                Vector2 repelDirection = (transform.position - collision.transform.position).normalized;
                repelDirection.y = 0.09f;

                // Apply repelling force
                player1.AddForce(repelDirection * repelForce, ForceMode2D.Force);

                //Play player damage audio
                damagedAudio.Play();

                // Declare new Health bar length (X-aixs)
                Vector2 newScale = healthBar.transform.localScale;
                newScale.x -= 0.1f;

                // Check if health bar length is 0 then the player starts dying animation
                if (newScale.x < 0)
                {
                    newScale.x = 0;
                    animator.SetBool("killed", true);
                    
                    loseAudio.Play();
                }

                // Apply the new health bar length
                healthBar.transform.localScale = newScale;
            }
        }

        // If player collided with health portion then increase his health
        // and remove the portion for scene
        if (collision.gameObject.tag.Equals("Health"))
        {
            //Increase health bar
            Vector2 newScale = healthBar.transform.localScale;
            newScale.x += 0.5f;
            healAudio.Play();
            if (newScale.x > 1)
            {
                newScale.x = 1f;
            }
            healthBar.transform.localScale = newScale;

            // Distroy portion object
            Destroy(collision.gameObject);  
        }

        //If player reached level finish, start second level
        if (collision.gameObject.tag.Equals("Finish"))
        {
            winAudio.Play();
            Destroy(this, 0.2f);
            SceneManager.LoadSceneAsync(2);
        }

        //If palyer finished the whole game, start (YOU WIN!) scene
        if (collision.gameObject.tag.Equals("LastLevel"))
        {
            winAudio.Play();
            Destroy(this, 0.2f);
            SceneManager.LoadSceneAsync(4);
        }

        //If player collided with the enemy in tutorial level (first one)
        if (collision.gameObject.name.Equals("tutorialEnemy"))
        {
            // If player attacking kill the enemy
            if (GetCurrentClipName().Contains("attack"))
            {
                collision.gameObject.GetComponent<EnemyScript>().kill();
            }
            // Else apply damage to the player
            else
            {
                // Calculate repelling direction
                Vector2 repelDirection = (transform.position - collision.transform.position).normalized;
                repelDirection.y = 0.09f;


                // Apply repelling force
                player1.AddForce(repelDirection * repelForce, ForceMode2D.Force);

                //Health bar decrease
                Vector2 newScale = healthBar.transform.localScale;
                newScale.x -= 0.1f;
                damagedAudio.Play();
                if (newScale.x <= 0)
                {
                    newScale.x = 0;
                    loseAudio.Play();
                    animator.SetBool("killed", true);
                }
                healthBar.transform.localScale = newScale;
            }
        }

        // If player fall from the ground return to spawn area
        // and apply damage to the player
        if (collision.gameObject.tag.Equals("DeadZone"))
        {
            //Return to spawn
            gameObject.transform.position = respawnPostion.position;

            //Apply damage
            Vector2 newScale = healthBar.transform.localScale;
            newScale.x -= 0.3f;
            damagedAudio.Play();
            if (newScale.x <= 0)
            {
                newScale.x = 0;
                loseAudio.Play();
                animator.SetBool("killed", true);
            }
            healthBar.transform.localScale = newScale;
        }

        // If player collided with traps, apply damage to the player
        if (collision.gameObject.tag.Equals("Traps"))
        {
                // Add repel force to up
                player1.AddForce(Vector2.up * repelForce/7, ForceMode2D.Force);

                //Decrease health bar
                Vector2 newScale = healthBar.transform.localScale;
                newScale.x -= 0.2f;
                damagedAudio.Play();
                if (newScale.x < 0)
                {
                    newScale.x = 0;
                    loseAudio.Play();
                    animator.SetBool("killed", true);
                }
                healthBar.transform.localScale = newScale;
            }
    }

    // Gets the name of current animation state of player
    public string GetCurrentClipName()
    {
        int layerIndex = 0;
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }
}
