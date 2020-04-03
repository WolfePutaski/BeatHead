using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class SC_PlayerProperties : MonoBehaviour
{

    //public bool isInvincible; //NoHealthReduction
    public bool OnBlock;

    Rigidbody2D playerPhysics;
    SC_CameraController cameraController;
    Animator playerAnim;
    AudioSource audioSource;

    [Header("Health")]
    public int maxBigHP;
    public int BigHP;
    public float BigHPKillRefillAmount;
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
    public List<GameObject> HPBlock;

    [Header("Posture")]
    public float maxPosture;
    public float posture;
    public float postureRegenRate_default;
    float postureRegenRate;
    float postureRegenDelayCount;

    public GameObject postureBar;

    [Header("Movement")]

    public int dashCountMax;
    public float dashRegenTime;
    public float dashForce;
    public float defaultSpeed = 0;
    public float slowWalkSpeed = 0;
    public float playerSpeed = 0;
    public bool canMove;

    [Header("Attacking")]
    public bool canAttack = true;
    public Transform attackPos;
    public Transform executionPos;
    public float attackDashForce;
    public float attackPushForce;
    public float startTimeBtwAttack;


    public float attackRadius;
    public float damage;


    [Header("Blocking")]
    public bool canBlock;
    public bool isBlocking;

    [Header("Deflection")]
    public ParticleSystem deflectPart;
    public ParticleSystem deflectSuccessPart;
    public float deflectionWindow;
    public float deflectDelay;
    public bool onDeflect;

    [Header("Audio")]
    public List<AudioClip> audioClips;
    //public float deflectTimer;


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
        cameraController = FindObjectOfType<SC_CameraController>();
        audioSource = GetComponent<AudioSource>();
        GameObject deflectP = GameObject.Find("Deflect Part");
        deflectPart = deflectP.GetComponent<ParticleSystem>();

        BigHP = maxBigHP;
        HP = maxHP;
        posture = Mathf.Clamp(maxPosture,0, maxPosture);
        postureRegenRate = postureRegenRate_default;

        //HUD
        postureBar = GameObject.Find("Player_PostureBar");
        HPBar = GameObject.Find("Player_HPBar");
        //HPBlock = new List<GameObject>();
        //HPBlock.AddRange(GameObject.FindGameObjectsWithTag("Player_HPBlock"));

    }

    // Update is called once per frame
    void Update()
    {

        BigHPRegen();
        UpdateHealth();
        PostureRegen();
        HUDUpdate();
        
    }

    private void LateUpdate()
    {
        ColorUpdate();
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

    public void Attacked(float damage,float postureDamage, float push) //get attacked
    {
        canMove = false;

        if (isBlocking)
        {
            if (onDeflect) //can deflect
            {
                //Debug.Log("Deflect!");
                //playerAnim.SetTrigger("Deflected");
            }
            else //weak block
            {
                posture -= postureDamage;
                if (posture <= 0)
                {
                    PlaySound("Player_GuardBreak");
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
            PlaySound("ExecutionHit");
            playerAnim.SetTrigger("IsHurt");
            cameraController.Shake();
            playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
            playerPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
            HP -= damage;
            HPregenDelayCount = defaultRegenDelay;

        }
    }

    public void Shot(float damage) //get attacked
    {
        canMove = false;

        {
            PlaySound("Player_Shot");
            playerAnim.SetTrigger("IsHurt");
            cameraController.Shake();
            playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
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
        else
        {
            BigHPRefillCount = 0;
        }

        
    }

    void BigHPRefillUpdate()
    {
        BigHPRefillCount += BigHPKillRefillAmount;
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

        if (HP <= 0)
        {
            HP = 0.01f;
            BigHP--;
            BigHPRefillCount = 0;
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
            if (Input.GetKeyDown(KeyCode.Space) && BigHP > 0)
            {
                mashCount += 1;
            }

            if (mashCount >= 10)
            {
                isDowned = false;
                onRecovering = true;
                playerAnim.SetTrigger("GetUp");
                HP = maxHP;
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

        yield return new WaitForSeconds(3);
 
        onRecovering = false;
        gameObject.layer = LayerMask.NameToLayer("Player");

    }

    void PostureRegen()
    {
        if (posture < 0)
        {
            posture = 0;
        }
        if (posture < maxPosture && postureRegenDelayCount <= 0
            && !isBlocking)
        {
            posture += Time.deltaTime * postureRegenRate;
        }

        if (isBlocking)
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
                img.fillAmount = 0;
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

        if (BigHP <= 0)
        {
            GameObject.Find("GAME OVER").GetComponent<TextMeshProUGUI>().enabled = true;
        }
    }

    void ColorUpdate()
    {
        if (onRecovering)
        {
            Color tmp = gameObject.GetComponent<SpriteRenderer>().color;
            tmp.a = .5f;
            gameObject.GetComponent<SpriteRenderer>().color = tmp; //beware, no function to return to default color yet
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

    void Return_Camera()
    {
        cameraController.activeCam = "Main Camera";
    }

    void ResetAllAnimTrigger()
    {
        foreach (AnimatorControllerParameter p in playerAnim.parameters)
        {

            if (p.type == AnimatorControllerParameterType.Trigger)
                playerAnim.ResetTrigger(p.name);
        }
    }

    public void PlaySound(string audioName)
    {
        audioSource.PlayOneShot(audioClips.Find(x => x.name == audioName));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }

}
