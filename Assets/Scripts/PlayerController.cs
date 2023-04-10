using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D playerRigidBody;
    public float movementSpeed;
    public Animator playerAnimator;
    public static PlayerController instance;
    public string areaTransitionName;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            // set the static instance value to be "this" script.
            // this means that, at game start, this player will be the only unique instance of the player in the game
            instance = this;
        } else 
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            playerRigidBody.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * movementSpeed;
        } else
        {
            playerRigidBody.velocity = Vector2.zero;
        }

        playerAnimator.SetFloat("moveX", playerRigidBody.velocity.x);
        playerAnimator.SetFloat("moveY", playerRigidBody.velocity.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1
            || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1) {
            if (canMove) {
                playerAnimator.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal") );
                playerAnimator.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical") );
            }
        }

        // keep the player inside the map bounds
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);
    }

    public void SetBounds(Vector3 botLeft, Vector3 topRight)
    {
        // we set the bot left and top right bounds, plus/minus half the player height/width
        // this is so that we don't have half the player sticking outside the map.
        bottomLeftLimit = botLeft + new Vector3(0.5f, 0.7f, 0);
        topRightLimit = topRight + new Vector3(-0.5f, -0.7f, 0);
    }
}
