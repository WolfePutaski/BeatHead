using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SC_PlayerProperties))]
public class SC_PlayerBlock : MonoBehaviour
{
    SC_PlayerProperties playerProperties;
    Animator playerAnim;
    ParticleSystem deflectPart;

    public bool canBlock;
    public bool isBlocking;

    [Header("Deflection")]
    public float deflectionWindow;
    public bool onDeflect;
    
    public float deflectTimer;


    // Start is called before the first frame update
    void Start()
    {
        playerProperties = GetComponent<SC_PlayerProperties>();
        playerAnim = GetComponent<Animator>();
        GameObject deflectP = GameObject.Find("Deflect Part");
        deflectPart = deflectP.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canBlock)
        {
            {
                if (Input.GetMouseButtonDown(1))
                {
                    isBlocking = true;
                    deflectTimer = deflectionWindow;
                    onDeflect = true;
                }

                if (Input.GetMouseButtonUp(1))
                {
                    isBlocking = false;
                    onDeflect = false;

                }
            }

            playerAnim.SetBool("Is Blocking", isBlocking);
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
            onDeflect = false;
        }

        if (onDeflect)
        {
            //gameObject.tag = "Deflect";
        }
    }

    void Unblock()
    {
        isBlocking = false;
    }

    void NotAllowBlock()
    {
        canBlock = false;
    }

    void AllowBlock()
    {
        canBlock = true;
    }

    void PlayParticle()
    {
        deflectPart.Play();
    }


}
