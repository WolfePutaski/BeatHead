using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SC_LevelController_Level1 : MonoBehaviour
{
    GameObject player;

    public PlayableAsset firstScene;
    public PlayableDirector playableDirector;
    public bool startFromBeginning;

    [System.Serializable]
    public class SceneTrigger
    {

        public GameObject BossDoorTrigger;
        public bool bossTriggered;

        public GameObject teachDeflect;

        public PlayableAsset Scene_FightingHeavy;
        
    }

    [System.Serializable]
    public class ComputerScene
    {
        public GameObject doorToLock;

        public GameObject computer;
        public PlayableAsset Scene_Lightout;
        public GameObject teachBasic_Text;
        public SC_InteractableObject_Switch object_Switch;
        

        public bool isTriggered;
        public bool lightOn;

        public PlayableAsset text_DefendComputer;
        public bool defendComputerTriggered;
    }

    [System.Serializable]
    public class DefendingScene
    {
        public PlayableAsset text_FixPC;
        public PlayableAsset text_FixP_Done;
        public PlayableAsset text_FinishPC;
        public bool FinishPcCHecked;


        public bool fixPCTriggered;

        public PlayableAsset text_EnemyGun;
        public bool enemyGunSeen;
    }

    [System.Serializable]
    public class EndingScene
    {
        public GameObject boss;

        public PlayableAsset text_EnemyFinished;
        public bool enemyFinishedCheck;
        public PlayableAsset seeBoss;
        public GameObject seeBossTrigger;
        public GameObject seeBossTriggerA;
        public GameObject seeBossTriggerB;
        public bool seeBossCheck;
        public PlayableAsset bossDead;
        public bool bossDeadTriggered;

        public PlayableAsset leaveLevel;
        public GameObject endLevelTrigger;
        public bool LeaveLevelChecked;
        
    }

    public SceneTrigger sceneTrigger;
    public ComputerScene computerScene;
    public DefendingScene defendingScene;
    public EndingScene endingScene;

    // Start is called before the first frame update

    private void Awake()
    {
        if (startFromBeginning)
        {
            playableDirector.Play(firstScene);
        }
    }
    void Start()
    {
        player = FindObjectOfType<SC_PlayerProperties>().gameObject;

    }

    // Update is called once per frame
    void Update()
    {

        HeavyEnemySceneUpdate();

        ComputerSceneUpdate();

        EndingSceneUpdate();


    }

    //EVENTS:
    //Kill all -> Hack PC -> Light Down -> Defend -> Get out


    public void HeavyEnemySceneUpdate()
    {
        if (Physics2D.OverlapCircle(sceneTrigger.BossDoorTrigger.transform.position, 1, LayerMask.GetMask("Player"))
            && !sceneTrigger.bossTriggered)
        {
            playableDirector.playableAsset = sceneTrigger.Scene_FightingHeavy;
            //playableDirector.Play
            playableDirector.Play();
            sceneTrigger.bossTriggered = true;
        }
        if (Physics2D.OverlapCircle(sceneTrigger.BossDoorTrigger.transform.position, 1, LayerMask.GetMask("Player")))
        {
            sceneTrigger.teachDeflect.SetActive(true);
        }
    }

    public void ComputerSceneUpdate()
    {
        if (computerScene.computer.GetComponent<SC_InteractableObject_Computer>().state == SC_InteractableObject_Computer.CompueterState.Active && !computerScene.isTriggered)
        {
            computerScene.isTriggered = true;
            computerScene.teachBasic_Text.SetActive(false);
            playableDirector.playableAsset = computerScene.Scene_Lightout;
            playableDirector.Play();
        }

        if (computerScene.isTriggered && !computerScene.lightOn)
        {
            computerScene.computer.GetComponent<SC_InteractableObject_Computer>().interactable = false;
            computerScene.computer.GetComponent<SC_InteractableObject_Computer>().textHelp.SetActive(false);

        }

        if (!computerScene.isTriggered)
        {
            computerScene.doorToLock.GetComponent<SC_Door>().isActive = false;
        }

        if (computerScene.object_Switch.isOn)
        {
            computerScene.lightOn = true;
            if(GameObject.Find("DarkLight") != null)
            {
                computerScene.computer.GetComponent<SC_InteractableObject_Computer>().soundOn = false;
                GameObject.Find("DarkLight").SetActive(false);
                computerScene.computer.GetComponent<SC_InteractableObject_Computer>().textHelp.SetActive(false);

            }

        }

        if (computerScene.lightOn && computerScene.computer.GetComponent<SC_InteractableObject_Computer>().state == SC_InteractableObject_Computer.CompueterState.Active && !computerScene.defendComputerTriggered)
        {
            computerScene.computer.GetComponent<SC_InteractableObject_Computer>().soundOn = true;
            playableDirector.playableAsset = computerScene.text_DefendComputer;
            FindObjectOfType<SC_EnemySquadSpawner>().unlimitedSpawn = true;

            playableDirector.Play();
            computerScene.defendComputerTriggered = true;
        }

        if (computerScene.defendComputerTriggered == true && computerScene.lightOn)
        {

            if (!defendingScene.enemyGunSeen)
            {
                if (FindObjectOfType<SC_EnemyRanged>() != null)
                {
                    defendingScene.enemyGunSeen = true;
                    playableDirector.Play(defendingScene.text_EnemyGun);
                }

            }

            if (computerScene.computer.GetComponent<SC_InteractableObject_Computer>().state == SC_InteractableObject_Computer.CompueterState.Done && !defendingScene.FinishPcCHecked)
            {
                FindObjectOfType<SC_EnemySquadSpawner>().unlimitedSpawn = false;
                playableDirector.Play(defendingScene.text_FinishPC);
                defendingScene.FinishPcCHecked = true;
            }

            if (defendingScene.fixPCTriggered == false && computerScene.computer.GetComponent<SC_InteractableObject_Computer>().state == SC_InteractableObject_Computer.CompueterState.Broken)
            {
                defendingScene.fixPCTriggered = true;
                playableDirector.Play(defendingScene.text_FixPC);
            }

            if (computerScene.computer.GetComponent<SC_InteractableObject_Computer>().state == SC_InteractableObject_Computer.CompueterState.Active && defendingScene.fixPCTriggered)
            {
                playableDirector.Play(defendingScene.text_FixP_Done);
                defendingScene.fixPCTriggered = false;
            }


        }



    }

    public void EndingSceneUpdate()
    {
        if (defendingScene.FinishPcCHecked)
        {
            if (!endingScene.enemyFinishedCheck) //check finished enemy
            {
                if (FindObjectOfType<SC_EnemySquadSpawner>().activeEnemyCount == 0)
                {
                    endingScene.enemyFinishedCheck = true;
                    playableDirector.Play(endingScene.text_EnemyFinished);
                }
            }

            if (endingScene.enemyFinishedCheck && !endingScene.seeBossCheck)
            {
                endingScene.seeBossTrigger.SetActive(true);
                //Collider2D[] hit = new Collider2D[2];
                //ContactFilter2D contactFilter = new ContactFilter2D();
                //contactFilter.layerMask = LayerMask.GetMask("Player");


                if (Physics2D.OverlapArea(endingScene.seeBossTriggerA.transform.position, endingScene.seeBossTriggerB.transform.position, LayerMask.GetMask("Player")))
                {
                    Debug.Log("Scenetrigger");
                    endingScene.boss.SetActive(true);
                    endingScene.seeBossCheck = true;
                    playableDirector.Play(endingScene.seeBoss);
                }

            }

            if (endingScene.seeBossCheck)
            {
                if (endingScene.boss.activeSelf && endingScene.boss.GetComponent<SC_EnemyBossDrugGuy_Properties>().state == SC_EnemyBossDrugGuy_Properties.DrugBossState.Dead && !endingScene.bossDeadTriggered)
                {
                    playableDirector.Play(endingScene.bossDead);
                    endingScene.bossDeadTriggered = true;
                    endingScene.endLevelTrigger.SetActive(true);
                }

                if (endingScene.bossDeadTriggered)
                {
                    if(Physics2D.OverlapCircle(endingScene.endLevelTrigger.transform.position,0.01f,LayerMask.GetMask("Player")) && !endingScene.LeaveLevelChecked)
                    {
                        playableDirector.Play(endingScene.leaveLevel);
                        endingScene.LeaveLevelChecked = true;
                    }
                }
            }
        }

    }
}
