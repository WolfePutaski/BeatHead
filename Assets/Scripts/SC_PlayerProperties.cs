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
    public int maxBigHP;
    public int BigHP;
    public float BigHPRefill;
    public float BigHPRefillCount;
    public float BigHPDegenRate;

    public float HP; //execution will gain health or when all enemy is gone;
    public float maxHP;
    public float defaultRegenDelay;
    public float HPRegenRate;
    public float mashCount;
    bool isDowned;
    float HPregenDelayCount;
    bool onRecovering;

    GameObject HPBar;
    public GameObject[] HPBlock;

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

        BigHP = maxBigHP;
        HP = maxHP;
        posture = Mathf.Clamp(maxPosture,0, maxPosture);
        postureRegenRate = postureRegenRate_default;

        //HUD
        postureBar = GameObject.Find("Player_PostureBar");
        HPBar = GameObject.Find("Player_HPBar");
        HPBlock = GameObject.FindGameObjectsWithTag("Player_HPBlock");
    }

    // Update is called once per frame
    void Update()
    {

        BigHPRegen();
        UpdateHealth();
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

    void BigHPRegen()
    {
        if (BigHP < maxBigHP)
        {
            if (BigHPRefillCount < 1 && BigHPRefillCount > 0)
            {
                BigHPRefillCount -= Time.deltaTime * BigHPDegenRate;
            }
            if (BigHPRefillCount >= 1)
            {
                BigHPRefillCount = 0;
                BigHP++;
            }
            if (BigHPRefillCount <= 0)
            {
                BigHPRefillCount = 0;
            }
        }
        
    }

    void UpdateHealth()
    {
        if (HP < maxHP && HPregenDelayCount <= 0)
        {
            HP += Time.deltaTime * HPRegenRate;
        }

        if (HPregenDelayCount > 0)
        {
            HPregenDelayCount -= Time.deltaTime;
        }

        if (HP < 0)
        {
            HP = 0;
            BigHP--;
            isDowned = true;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            playerAnim.SetTrigger("WentDown");

        }

        //Downed
        if (isDowned)
        {
            
            if (mashCount > 0)
            {
                mashCount -= Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                mashCount += 1;
            }

            if (mashCount >= 10)
            {
                isDowned = false;
                onRecovering = true;
                playerAnim.SetTrigger("GetUp");
                HP = 10;
                mashCount = 0;
                StartCoroutine(GetUpDelay());
 
            }
        }

        if (onRecovering)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    IEnumerator GetUpDelay()
    {

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .5f);
        yield return new WaitForSeconds(3);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        onRecovering = false;
        gameObject.layer = LayerMask.NameToLayer("Player");

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
        //HP
        if (!isDowned)
        {
            float HPPercentage = (HP / maxHP);
            HPBar.GetComponent<Image>().fillAmount = HPPercentage;
        }
        if (isDowned)
        {
            HPBar.GetComponent<Image>().fillAmount = mashCount/10;
        }


        //BigHP
        for (int i = 0; i < maxBigHP; i++)
        {
            Image img = HPBlock[i].GetComponent<Image>();

            if (i < BigHP) //active
            {
                HPBlock[i].SetActive(true);
                img.color = new Color(1f, 1f, 1f);
                img.fillAmount = 1;
            }
            //last one
            if (i == BigHP)
            {
                img.color = new Color(0.6f, 0.6f, 0.6f);
                img.fillAmount = BigHPRefillCount;
            }
            //more than last
            if (i > BigHP)
            {
                HPBlock[i].SetActive(false);
            }
        }
        //Posture
        float posturePercentage = (1 - posture / maxPosture);
        postureBar.GetComponent<Image>().fillAmount = posturePercentage;

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
