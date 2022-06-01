using UnityEngine;

// class animates sprites in 4 cardinal directions using current velocity
public class AnimatorScript : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rigidbody2d;
    private float moveX;
    private float moveY;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        moveX = rigidbody2d.velocity.x;
        moveY = rigidbody2d.velocity.y;

        // determine movement direction and status using current velocity
        if (moveX != 0 || moveY != 0)
        {
            anim.SetBool("isMoving", true);
            anim.SetFloat("moveX", moveX);
            anim.SetFloat("moveY", moveY);
        }
        // set isMoving to false so Animator will serve up Idle animation
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}