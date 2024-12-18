using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public float attackRange = 2f;
    public float repelForce = 10f;
    public GameObject mainChar;
    public Rigidbody2D rigidBody;
    private bool isPlayerNearby = false;
    private bool isAttacking = false;
    public GameObject health;
    public Vector3 barOffset;
    public Animator animator;
    bool isDead;

    private void Update()
    {
        // Change health bar postion to be above the flying eye object
        health.transform.position = transform.position + barOffset;

        // If flying eye isn't dead
        if(!isDead){
        // Check if the player is within detection range
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            isPlayerNearby = true;
            rigidBody.WakeUp();
        }
        else
        {
            isPlayerNearby = false;
                rigidBody.Sleep();
         }

        // If player is nearby and the flying eye not already attacking, move it towards the player
        if (isPlayerNearby && !isAttacking)
        {
            if (player.position.x < transform.position.x)
            {
                // Player is to the left, rotate left
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                // Player is to the right, rotate right
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            // Apply velocity on the flying eye to move
            Vector3 direction = (player.position - transform.position).normalized;
            rigidBody.velocity = direction * moveSpeed;
        }

            // Check if within attack range and not already attacking
            if (isPlayerNearby && distanceToPlayer <= attackRange && !isAttacking)
            {
                isAttacking = true;
                animator.SetBool("Attack", true);
            }
            else
            {
                isAttacking = false;
                animator.SetBool("Attack", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If collided with the player and the player is attacking,
        //apply damage to the flying eye
        if (collision.gameObject.tag.Equals("Player"))
        {
            if (GetPlayerClipName().Contains("attack"))
            {
                //Change animation state
                animator.SetBool("Hit", true);

                //Decrease health bar
                Vector2 newScale = health.transform.localScale;
                newScale.x -= 0.1f;
                if (newScale.x <= 0)
                {
                    newScale.x = 0;
                    animator.SetBool("Fall", true);
                    isDead = true;
                    Destroy(gameObject.GetComponent<Rigidbody2D>());
                    BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();
                    boxCollider.enabled=false;
                }
                health.transform.localScale = newScale;
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        //To release animator object from the taking hit state
        animator.SetBool("Hit", false);
    }

    //Gets Player animation state name
    string GetPlayerClipName()
    {
        int layerIndex = 0;
        AnimatorClipInfo[] clipInfo = mainChar.GetComponent<Animator>().GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }
}
