using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PlayerProperties : MonoBehaviour
{

    public bool isInvincible; //NoHealthReduction
    public bool OnBlock;

    Rigidbody2D playerPhysics;
    SC_CameraController cameraController;
    Animator playerAnim;


    [Header("Health")]
    public float HP;
    public float MaxHP;

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
        if (!OnBlock)
        {
            cameraController.Shake();
            playerAnim.SetTrigger("IsHurt");
            HP -= damage;
            playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
            playerPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
        }
    }

}
