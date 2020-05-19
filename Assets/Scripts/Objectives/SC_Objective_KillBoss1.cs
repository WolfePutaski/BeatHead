using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;


//List: Defeat Boss >>> (((Find Syringe >>> Stab (3 Times)))) >>> DONE

[RequireComponent(typeof(SC_PlayerObjectiveController))]
public class SC_Objective_KillBoss1 : MonoBehaviour
{
    string StartObjectiveText = "Defeat Donald Mad Dog";
    public string text_FindSyringe1 = "Find the Drug Syringe!";
    public string text_FindSyringe2 = "Find another Drug Syringe!";
    public string text_FindSyringe3 = "Find the last Drug Syringe!";
    string text_SyringeDropped = "Syringe is Dropped! Pick it Up!";
    //string text_InjectSyringe = "Inject Syringe with Execution!";
    public List<string> text_FindSyringeVariants;
    string text_;

    PlayableDirector playableDirector;
    SC_PlayerProperties playerProperties;
    Animator playerAnim;
    SC_PlayerAttack playerAttack;
    public RuntimeAnimatorController defaultPlayerAnim;
    public int BossPhase; // 0,1,2,3
    public bool SyringeFound;
    public bool hasSyringe;
    public AnimatorOverrideController animatorOverride_Stab;
    //public AnimationClip StabAnimation;

    public GameObject currentSyringe;
    public GameObject syringePrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerProperties = gameObject.GetComponent<SC_PlayerProperties>();
        playerAnim = gameObject.GetComponent<Animator>();
        playerAttack = gameObject.GetComponent<SC_PlayerAttack>();
        defaultPlayerAnim = playerAnim.runtimeAnimatorController;
        animatorOverride_Stab = Resources.Load<AnimatorOverrideController>("AnimatorOverride_Player_SyringeStab");
        syringePrefab = Resources.Load<GameObject>("Syringe");
        playableDirector = FindObjectOfType<PlayableDirector>();

        GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(StartObjectiveText);

        text_FindSyringeVariants = new List<string>();
        text_FindSyringeVariants.Add(text_FindSyringe1);
        text_FindSyringeVariants.Add(text_FindSyringe2);
        text_FindSyringeVariants.Add(text_FindSyringe3);


    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.KeypadPlus))
        //{
        //    PickSyringe();
        //}

        Collider2D hit = Physics2D.OverlapCircle(playerProperties.attackPos.position, playerProperties.attackRadius, LayerMask.GetMask("Enemies_ToExecute"));

        if (Physics2D.OverlapCircle(playerProperties.attackPos.position, playerProperties.attackRadius, LayerMask.GetMask("Enemies_ToExecute")))
        {
            if (hit.gameObject.name == "Enemy_BossMad" && hasSyringe)
            {
                    playerAnim.runtimeAnimatorController = animatorOverride_Stab;
                    //GetComponent<SC_PlayerObjectiveController>().isShowingNewObjective = false;
                
            }
            else
            {
                //playerAnim.runtimeAnimatorController = defaultPlayerAnim;
            }

        }
        //else
        //{
        //    playerAnim.runtimeAnimatorController = defaultPlayerAnim;
        //}

        if (hasSyringe)
        {
            //GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(text_InjectSyringe);

            //GetComponent<SC_PlayerObjectiveController>().isShowingNewObjective = true;
            //GetComponent<SC_PlayerObjectiveController>().showObjective = true;

            if (FindObjectOfType<SC_EnemyBossDrugGuy_Properties>() != null)
            {
                FindObjectOfType<SC_EnemyBossDrugGuy_Properties>().isInjected = true;
            }

        }


        if (hasSyringe == false && SyringeFound)
        {
                currentSyringe = FindObjectOfType<SC_InteractableObject_Syringe>().gameObject;
        }

        SyringeFound = (FindObjectOfType<SC_InteractableObject_Syringe>());


        if (FindObjectOfType<SC_EnemyBossDrugGuy_Properties>() != null)
        {
            var bossProperties = FindObjectOfType<SC_EnemyBossDrugGuy_Properties>();

            if (bossProperties.state == SC_EnemyBossDrugGuy_Properties.DrugBossState.Dead)
            {
                GetComponent<SC_PlayerObjectiveController>().ObjectiveClear();
            }

            if (bossProperties.state == SC_EnemyBossDrugGuy_Properties.DrugBossState.Normal && bossProperties.enemyProperties.HP <= 0)
            {
                GetComponent<SC_PlayerObjectiveController>().ObjectiveClear();
            }

            BossPhase = bossProperties.currentPhase - 1;

        }

        if (hasSyringe && playerProperties.HP == 0)
        {
            DropSyringe();
        }


    }

    public void DropSyringe()
    {
        Instantiate(syringePrefab,playerProperties.transform);
        hasSyringe = false;
        GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(text_SyringeDropped);
    }

    public void LostSyringe()
    {
        hasSyringe = false;
        //GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(text_FindSyringeVariants[BossPhase]);

    }

    public void PickSyringe()
    {
        //GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(text_InjectSyringe);
        //GetComponent<SC_PlayerObjectiveController>().objectiveText.GetComponent<TextMeshProUGUI>().text = text_InjectSyringe;
        currentSyringe = null;
        hasSyringe = true;
    }
}
