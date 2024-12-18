using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public Animator animtor;
    public GameObject health;
    public GameObject mainChar;
    public float repelForce=50f;
    public float attackRange;
    public Vector3 barOffset;
    private bool isPlayerNearby = false;

    private void Update()
    {
        //Health bar position update to be on the top of the skeleton
        health.transform.position = transform.position+ barOffset;

        // Check if the player is within detection range
        float distanceToPlayer = Vector2.Distance(transform.position, mainChar.transform.position);
        if (distanceToPlayer <= detectionRange)
        {
            isPlayerNearby = true;
            animtor.SetBool("SeenEnemy", true);
        }
        else
        {
            isPlayerNearby = false;
            animtor.SetBool("SeenEnemy", false);
        }

        // If player is nearby, move towards the player on the X-axis
        if (isPlayerNearby)
        {
           
            if (mainChar.transform.position.x < transform.position.x)
            {
                // Player is to the left, move left
                    transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                // Player is to the right, move right
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            
            // If player entered the attack range of the skeleton,
           // attack the player
           if (Mathf.Abs(mainChar.transform.position.x - transform.position.x)<=attackRange) {
                animtor.SetBool("Attack", true);
            }
            else
            {
                animtor.SetBool("Attack", false);
            }

           // Stop moving when attacking the player
            if (!GetCurrentClipName().Equals("Attack"))
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }
        }

        // Release animator from the taking hit state 
        //(set bool with false to make animator return to it's parent state)
        if (GetCurrentClipName().Equals("TakenHit"))
        {
            animtor.SetBool("TakenHit", false);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // If skeleton collided with the player and player is attacking,
        // decrease skeleton health
        if (collision.gameObject.tag.Equals("Player"))
        {
            if (GetPlayerClipName().Contains("attack"))
            {
                // Change animator state
                animtor.SetBool("TakenHit", true);

                // Decrease health bar
                Vector2 newScale = health.transform.localScale;
                newScale.x -= 0.1f;
                if (newScale.x <= 0)
                {
                    newScale.x = 0;
                    animtor.SetBool("Dead", true);
                    Destroy(gameObject.GetComponent<Rigidbody2D>());
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    Destroy(this, 0.1f);
                }
                health.transform.localScale = newScale;
            }
        }
        // Destroy skeleton when it falls to the dead zone
        else if (collision.gameObject.tag.Equals("DeadZone"))
        {
            Vector2 newScale = health.transform.localScale;
            newScale.x = 0;
            animtor.SetBool("Dead", true);
            Destroy(gameObject, 0.1f);
            health.transform.localScale = newScale;
    }

    }


    // Gets skeleton animation state name
    string GetCurrentClipName()
    {
        int layerIndex = 0;
        AnimatorClipInfo[] clipInfo = animtor.GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }

    //Gets player animation state name
    string GetPlayerClipName()
    {
        int layerIndex = 0;
        AnimatorClipInfo[] clipInfo = mainChar.GetComponent<Animator>().GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }
}
