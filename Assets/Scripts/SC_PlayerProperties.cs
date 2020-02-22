using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PlayerProperties : MonoBehaviour
{

    //public bool isInvincible; //NoHealthReduction
    public bool OnBlock;

    Rigidbody2D playerPhysics;
    SC_PlayerMovement playerMovement;
    SC_CameraController cameraController;
    SC_PlayerBlock playerBlock;
    Animator playerAnim;



    [Header("Health")]
    public float HP; //execution will gain health or when all enemy is gone;
    public float MaxHP;

    [Header("Posture")]


    [Header("AttackRequest")]

    public int MaxMeleeAttackers = 1;
    public List<GameObject> MeleeAttackers;

    public int MaxRangedAttackers = 1;
    public List<GameObject> RangedAttackers;

    // Start is called before the first frame update
    void Start()
    {
        HP = MaxHP;
        playerPhysics = GetComponent<Rigidbody2D>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        playerBlock = gameObject.GetComponent<SC_PlayerBlock>();
        playerMovement = GetComponent<SC_PlayerMovement>();

        cameraController = FindObjectOfType<SC_CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HP < MaxHP)
        {
            HP += Time.deltaTime;
        }
    }


    public void GetAttackRequest(GameObject requestor)
    {
        MeleeAttackers.RemoveAll(item => item == null);
        if (MeleeAttackers.Count < MaxMeleeAttackers)
        {
            if (!MeleeAttackers.Contains(requestor))
            {
                requestor.SendMessage("AllowtoAttack");
                MeleeAttackers.Add(requestor);
                Debug.Log("Attack Allowing");
            }
        }
        else { }
    }
    public void CancelAttacker(GameObject requestor)
    {
        MeleeAttackers.Remove(requestor);
    }

    public void Attacked(float damage,float push)
    {
        if (playerBlock.isBlocking)
        {
            if (playerBlock.onDeflect) //can deflect
            {
                Debug.Log("Deflect!");
            }
            else //weak block
            {
                playerAnim.SetTrigger("AttackBlocked");
                playerMovement.canMove = false;
                playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
                playerPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
            }
            

        }
        else //take damage
        {
            playerAnim.SetTrigger("IsHurt");
            playerMovement.canMove = false;
            cameraController.Shake();
            playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
            playerPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
            HP -= damage;

        }
    }

}
