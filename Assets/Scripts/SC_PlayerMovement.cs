using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SC_PlayerProperties))]
public class SC_PlayerMovement : MonoBehaviour
{
    Rigidbody2D playerPhysics;
    Animator playerAnim;
    ParticleSystem dashPart;
    SC_PlayerProperties playerProperties;
    GameObject[] dashArrows;

    public int dashCount;
    float dashRegenTimer;

    // Start is called before the first frame update
    void Awake()
    {
        playerProperties = gameObject.GetComponent<SC_PlayerProperties>();
        playerPhysics = gameObject.GetComponent<Rigidbody2D>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();

        dashCount = playerProperties.dashCountMax;
        //dashPart = GameObject.Find("DashPart").GetComponent<ParticleSystem>();

        dashArrows = GameObject.FindGameObjectsWithTag("Player_DashArrow");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerProperties.canMove)
        {
            Movement();

            if (Input.GetKeyDown(KeyCode.V) && Input.GetAxisRaw("Horizontal") != 0 && dashCount > 0)
            {
                Roll();
            }
        }

        DashRegen();
        HUDUpdate();


    }

    void Roll()
    {
        playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
        playerPhysics.AddForce(Vector2.right * transform.localScale.x * playerProperties.dashForce, ForceMode2D.Impulse);
        //dashPart.Play();
        playerAnim.SetTrigger("Pressed Roll");

        dashCount--;
        dashRegenTimer = playerProperties.dashRegenTime;


    }

    void DashRegen()
    {
            if (dashRegenTimer <= 0)
            {
                dashRegenTimer = playerProperties.dashRegenTime;
                dashCount = Mathf.Clamp(dashCount + 1, 0, playerProperties.dashCountMax);
            }
            else
            {
                if (dashCount < playerProperties.dashCountMax)
                {
                    dashRegenTimer -= Time.deltaTime;
                }
            }
       


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

    void HUDUpdate()
    {
        for (int i = 0; i < dashArrows.Length; i++)
        {
            if (i+1 <= dashCount)
            {

                dashArrows[i].GetComponent<Image>().color = new Color(1,1,1,1);

            }
            else
            {
                dashArrows[i].GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 0.3f);
            }
        }
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

