using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SC_PlayerProperties))]
public class SC_PlayerBlock : MonoBehaviour
{
    SC_PlayerProperties playerProperties;
    Animator playerAnim;
    //ParticleSystem deflectPart;

    //public bool canBlock;
    //public bool isBlocking;

    //[Header("Deflection")]
    //public float deflectionWindow;
    //public bool onDeflect;

    public float deflectTimer;
    public float deflectDelayTimer;


    // Start is called before the first frame update
    void Start()
    {
        playerProperties = GetComponent<SC_PlayerProperties>();
        playerAnim = GetComponent<Animator>();
    }

    //Howw Delay Works
    //If player release block when deflect timer is not over, start delay; if player start holding block before the delay is over, the deflect will not active and delay start again. This prevent spamming


    // Update is called once per frame
    void Update()
    {
        if (playerProperties.canBlock)
        {
            {
                if (Input.GetMouseButtonDown(1))
                {
                    playerProperties.canAttack = false;
                    playerProperties.isBlocking = true;
                    if (deflectDelayTimer > 0)
                    {
                        deflectDelayTimer = playerProperties.deflectDelay;
                    }
                    else
                    {
                        deflectTimer = playerProperties.deflectionWindow;
                        playerProperties.onDeflect = true;
                    }
                }


            }

            playerAnim.SetBool("Is Blocking", playerProperties.isBlocking);
        }

        if (Input.GetMouseButtonUp(1))
        {
            playerProperties.canAttack = true;
            playerProperties.isBlocking = false;
            playerProperties.onDeflect = false;
            playerAnim.ResetTrigger("Deflected");
            if(deflectTimer > 0)
            {
            deflectDelayTimer = playerProperties.deflectDelay;
            }
        }

        DeflectCountdown();

    }

    void DeflectCountdown()
    {
        if (deflectTimer > 0)
        {
            deflectTimer -= Time.deltaTime;
        }
        if (deflectTimer <= 0)
        {
            playerProperties.onDeflect = false;
        }

        if (playerProperties.onDeflect)
        {
            //gameObject.tag = "Deflect";
        }

        if (deflectDelayTimer > 0)
        {
            deflectDelayTimer -= Time.deltaTime;
        }
        else
        {
            deflectDelayTimer = 0;
        }
    }

    void Deflect()
    {
        playerAnim.SetTrigger("Deflected");
        deflectTimer = 0;
        deflectDelayTimer = 0;
        PlayParticle();
    }

    void DeflectSuccess()
    {
        playerProperties.PlaySound("Player_DeflectSuccess");
        playerProperties.deflectSuccessPart.Play();
    }

    void Unblock()
    {
        playerProperties.isBlocking = false;
    }

    void Deactive_canBlock()
    {
        playerProperties.canBlock = false;
    }

    void Active_canBlock()
    {
        playerProperties.canBlock = true;
    }

    void PlayParticle()
    {
        playerProperties.deflectPart.Play();
    }


}
