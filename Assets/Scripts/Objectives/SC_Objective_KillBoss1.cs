using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


//List: Defeat Boss >>> (((Find Syringe >>> Stab (3 Times)))) >>> DONE
public enum Phases
{
    DefeatBoss1, FindSyringe, Stab, End
}
[RequireComponent(typeof(SC_PlayerObjectiveController))]
public class SC_Objective_KillBoss1 : MonoBehaviour
{
    string objectiveText = "Defeat The Jackal";
    string text_FindSyringe1 = "Find the Drug Syringe!\nIt should be hidden somewhere!";
    string text_FindSyringe2 = "He's not down yet!\n Find another Drug Syringe!";
    string text_FindSyringe3 = "He's almost out!\n Find the last Drug Syringe!";
    string text_SyringeDropped = "Syringe is Dropped! Pick it Up!";
    string text_InjectSyringe = "Inject Syringe with Execution!";
    string text_;


    SC_PlayerProperties playerProperties;
    Animator playerAnim;
    SC_PlayerAttack playerAttack;
    public RuntimeAnimatorController defaultPlayerAnim;
    public int BossPhase; // 0,1,2,3
    public bool SyringeFound;
    public bool findingSyringe;
    public bool hasSyringe;
    public AnimatorOverrideController animatorOverride_Stab;
    //public AnimationClip StabAnimation;

    public GameObject currentSyringe;



    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(objectiveText);
        playerProperties = gameObject.GetComponent<SC_PlayerProperties>();
        playerAnim = gameObject.GetComponent<Animator>();
        playerAttack = gameObject.GetComponent<SC_PlayerAttack>();
        defaultPlayerAnim = playerAnim.runtimeAnimatorController;
        animatorOverride_Stab = Resources.Load<AnimatorOverrideController>("AnimatorOverride_Player_SyringeStab");


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            PickSyringe();
        }

        Collider2D hit = Physics2D.OverlapCircle(playerProperties.attackPos.position, playerProperties.attackRadius, LayerMask.GetMask("Enemies_ToExecute"));

        if (Physics2D.OverlapCircle(playerProperties.attackPos.position, playerProperties.attackRadius, LayerMask.GetMask("Enemies_ToExecute")))
        {
            if (hit.gameObject.name == "Enemy_BossMad" && hasSyringe)
            {
                    playerAnim.runtimeAnimatorController = animatorOverride_Stab;
                    GetComponent<SC_PlayerObjectiveController>().isShowingNewObjective = false;
                
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

            GetComponent<SC_PlayerObjectiveController>().isShowingNewObjective = true;
            GetComponent<SC_PlayerObjectiveController>().showObjective = true;

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







    }

    public void LostSyringe()
    {
        hasSyringe = false;
        GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(text_FindSyringe1);

    }

    public void PickSyringe()
    {
        //GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(text_InjectSyringe);
        GetComponent<SC_PlayerObjectiveController>().objectiveText.GetComponent<TextMeshProUGUI>().text = text_InjectSyringe;
        currentSyringe = null;
        hasSyringe = true;
    }
}
