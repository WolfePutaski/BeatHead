using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SC_PlayerProperties))]
public class SC_PlayerMovement : MonoBehaviour
{
    Rigidbody2D playerPhysics;
    Animator playerAnim;
    SC_PlayerProperties playerProperties;

    // Start is called before the first frame update
    void Awake()
    {
        playerProperties = gameObject.GetComponent<SC_PlayerProperties>();
        playerPhysics = gameObject.GetComponent<Rigidbody2D>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerProperties.canMove)
        {
            Movement();

            if (Input.GetKeyDown(KeyCode.V)
    && Input.GetAxisRaw("Horizontal") != 0
    )
            {
                Roll();
            }
        }


    }

    void Roll()
    {
        playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
        playerPhysics.AddForce(Vector2.right * transform.localScale.x * playerProperties.rollForce, ForceMode2D.Impulse);
        playerAnim.SetTrigger("Pressed Roll");
        

    }

    void Movement()
    {

        if (playerProperties.isBlocking)
        {
            playerProperties.playerSpeed = playerProperties.slowWalkSpeed;
        }
        else
        {
            playerProperties.playerSpeed = playerProperties.defaultSpeed;
        }

        playerPhysics.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * playerProperties.playerSpeed * Time.deltaTime, playerPhysics.velocity.y);



        if (playerPhysics.velocity.x > 0)

            {
                gameObject.transform.localScale = new Vector2(1, gameObject.transform.localScale.y);
            }
            else if (playerPhysics.velocity.x< 0)
            {
                gameObject.transform.localScale = new Vector2(-1, gameObject.transform.localScale.y);
            }

        playerAnim.SetFloat("Moving", Mathf.Abs(Input.GetAxisRaw("Horizontal")));
    }


    void Active_Movement()
    {
        playerProperties.canMove = true;
    }

    void Deactive_Movement()
    {
        playerProperties.canMove = false;
    }

    void On_Dodge()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    void End_Dodge()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
}

