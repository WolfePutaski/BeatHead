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

    //public float deflectTimer;


    // Start is called before the first frame update
    void Start()
    {
        playerProperties = GetComponent<SC_PlayerProperties>();
        playerAnim = GetComponent<Animator>();
    }

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
                    playerProperties.deflectTimer = playerProperties.deflectionWindow;
                    playerProperties.onDeflect = true;
                }


            }

            playerAnim.SetBool("Is Blocking", playerProperties.isBlocking);
        }

        if (Input.GetMouseButtonUp(1))
        {
            playerProperties.canAttack = true;
            playerProperties.isBlocking = false;
            playerProperties.onDeflect = false;

        }

        DeflectCountdown();

    }

    void DeflectCountdown()
    {
        if (playerProperties.deflectTimer > 0)
        {
            playerProperties.deflectTimer -= Time.deltaTime;
        }
        if (playerProperties.deflectTimer <= 0)
        {
            playerProperties.onDeflect = false;
        }

        if (playerProperties.onDeflect)
        {
            //gameObject.tag = "Deflect";
        }
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
