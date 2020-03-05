using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SC_PlayerProperties))]
public class SC_PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerPhysics;
    Animator playerAnim;
    public SC_PlayerProperties playerProperties;
    SC_PlayerBlock playerBlock;

    public float rollForce;
    public float defaultSpeed = 0;
    public float slowWalkSpeed = 0;
    public float playerSpeed = 0;

    public bool canMove;

    // Start is called before the first frame update
    void Awake()
    {
        playerProperties = gameObject.GetComponent<SC_PlayerProperties>();
        playerPhysics = gameObject.GetComponent<Rigidbody2D>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        playerBlock = gameObject.GetComponent<SC_PlayerBlock>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
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
        playerPhysics.AddForce(Vector2.right * transform.localScale.x * rollForce, ForceMode2D.Impulse);
        playerAnim.SetTrigger("Pressed Roll");
        

    }

    void Movement()
    {

        if (playerBlock.isBlocking)
        {
            playerSpeed = slowWalkSpeed;
        }
        else
        {
            playerSpeed = defaultSpeed;
        }

        playerPhysics.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * playerSpeed * Time.deltaTime, playerPhysics.velocity.y);



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


    void NotAllowMove()
    {
        canMove = false;
    }

    void AllowMove()
    {
        canMove = true;
    }

    void OnDodge()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    void ReturnDodge()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
}

