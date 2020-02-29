using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int defaultHPBlock;
    public int HPBlock;
    public float HP; //execution will gain health or when all enemy is gone;
    public float maxHP;
    public float defaultRegenDelay;
    float HPregenDelayCount;

    [Header("Posture")]
    public float maxPosture;
    public float posture;
    public float postureRegenRate_default;
    float postureRegenRate;
    float postureRegenDelayCount;

    GameObject postureBar;


    [Header("AttackRequest")]

    public int MaxMeleeAttackers = 1;
    public List<GameObject> MeleeAttackers;

    public int MaxRangedAttackers = 1;
    public List<GameObject> RangedAttackers;

    // Start is called before the first frame update
    void Start()
    {
        playerPhysics = GetComponent<Rigidbody2D>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        playerBlock = gameObject.GetComponent<SC_PlayerBlock>();
        playerMovement = GetComponent<SC_PlayerMovement>();
        cameraController = FindObjectOfType<SC_CameraController>();

        HP = maxHP;
        posture = Mathf.Clamp(maxPosture,0, maxPosture);
        postureRegenRate = postureRegenRate_default;

        //HUD
        postureBar = GameObject.Find("Player_PostureBar");
    }

    // Update is called once per frame
    void Update()
    {

        HPRegen();
        PostureRegen();
        HUDUpdate();
        

        
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

    public void Attacked(float damage,float postureDamage, float push)
    {
        if (playerBlock.isBlocking)
        {
            if (playerBlock.onDeflect) //can deflect
            {
                //Debug.Log("Deflect!");
                //playerAnim.SetTrigger("Deflected");
            }
            else //weak block
            {
                playerMovement.canMove = false;
                posture -= postureDamage;
                if (posture <= 0)
                {
                    playerAnim.SetTrigger("Stunned");
                    //posture = maxPosture / 3;
                }
                else
                {
                    playerAnim.SetTrigger("AttackBlocked");
                }
                playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
                playerPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
            }
            

        }
        else //take damage
        {
            playerMovement.canMove = false;
            playerAnim.SetTrigger("IsHurt");
            cameraController.Shake();
            playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
            playerPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
            HP -= damage;
            HPregenDelayCount = defaultRegenDelay;

        }
    }

    void HPRegen()
    {
        if (HP < maxHP && HPregenDelayCount <= 0)
        {
            HP += Time.deltaTime;
        }

        if (HPregenDelayCount > 0)
        {
            HPregenDelayCount -= Time.deltaTime;
        }
    }

    void PostureRegen()
    {
        if (posture < maxPosture && postureRegenDelayCount <= 0
            && !playerBlock.isBlocking)
        {
            posture += Time.deltaTime * postureRegenRate;
        }

        if (playerBlock.isBlocking)
        {
            postureRegenRate = postureRegenRate_default / 2;
            postureRegenDelayCount = defaultRegenDelay;

        }
        else
        {
            postureRegenRate = postureRegenRate_default;
            postureRegenDelayCount -= Time.deltaTime;
        }
    }
    
    void HUDUpdate()
    {
        float percentage = (1 - posture / maxPosture);
        postureBar.GetComponent<Image>().fillAmount = percentage;

        if (posture < maxPosture)
        {
            foreach (GameObject a in GameObject.FindGameObjectsWithTag("Player_PostureBar"))
            {
                a.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            }
        }
        else
        {
            foreach (GameObject a in GameObject.FindGameObjectsWithTag("Player_PostureBar"))
            {
                a.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
            }
        }
    }

    void ResetHP()
    {
        HP = maxHP;
    }

    void ResetPosture()
    {
        posture = maxPosture;
    }


}
