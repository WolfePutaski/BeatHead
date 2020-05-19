using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SC_LevelController_Level2 : MonoBehaviour
{
    [System.Serializable]
    public class FindPasswordScene
    {
        public SC_InteractableObject_Computer computer;
        public List<float> frozenTime; //once PC Timer reached this number, it will start Freezing until player find the password;
        public float frozenTimeDeviation;
        public bool isFreezing;
        public bool waitForFinding;


        public List<SC_InteractableObject_Drawer> drawers;

        public PlayableAsset goFind;
        public bool findingFirstTimeChecked;
        public PlayableAsset goFind2;
        public PlayableAsset foundScene;
        public PlayableAsset notFoundScene;

        //PC Progress

        public PlayableAsset text_DefendPC;
        public bool startedDefendPC;
        public PlayableAsset text_FixPC;
        public bool fixPCTriggered;
        public PlayableAsset text_FixP_Done;
        public PlayableAsset text_FinishPC;
        public bool FinishPcHacked;

        public int toFindCount;
        public int foundCount;
    }

    [System.Serializable]
    public class BossDrug
    {
        public List<SC_InteractableObject_Drawer> drawers_Drug;

        //public bool isFindingLast;
        public PlayableAsset goFind;
        public bool isFinding; //for finding the syringe the first time after player deplete boss health or trying to Execute
        public PlayableAsset goFindLast;
        public bool isFindingLast;
        public float maxReminderCountdown;
        public float reminderCountdown;
        public PlayableAsset reminderScene;

        public SC_InteractableObject_Drawer lastDrugLocation;
        public PlayableAsset foundLast;
        public PlayableAsset foundScene;
        public bool hasFound;
        public PlayableAsset notFoundScene;
        public PlayableAsset droppedSyringeScene;
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



    public PlayableDirector playableDirector;
    public PlayableAsset beginningScene;
    public bool startFromBeginning;
    public GameObject player;

    public FindPasswordScene findPasswordScene;
    public BossDrug bossDrug;
    public EndingScene endingScene;

    private void Awake()
    {
        if (startFromBeginning)
        {
            playableDirector.Play(beginningScene);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        findPasswordScene.computer = FindObjectOfType<SC_InteractableObject_Computer>();
        playableDirector = FindObjectOfType<PlayableDirector>();
        player = FindObjectOfType<SC_PlayerProperties>().gameObject;

        for (int i = 0; i < findPasswordScene.frozenTime.Count; i++)
        {
            findPasswordScene.frozenTime[i] = findPasswordScene.frozenTime[i] - Random.Range(-findPasswordScene.frozenTimeDeviation, findPasswordScene.frozenTimeDeviation);
        }


        bossDrug.reminderCountdown = bossDrug.maxReminderCountdown;


    }

    // Update is called once per frame
    void Update()
    {
        FrozenComputer();
        DefendingPC();
        BossFightUpdate();
        FindingDrug();
    }

    public void FrozenComputer()
    {
        for (int i = 0; i < findPasswordScene.drawers.Count; i++)
        {
            findPasswordScene.drawers[i].isObjectiveActive = findPasswordScene.isFreezing;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            findPasswordScene.waitForFinding = true;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            findPasswordScene.waitForFinding = false;
        }

        if (findPasswordScene.frozenTime.Count != 0 && findPasswordScene.computer.progressCount > findPasswordScene.frozenTime[0]) // Freezing
        {

            findPasswordScene.computer.progressCount = findPasswordScene.frozenTime[0];
            findPasswordScene.computer.state = SC_InteractableObject_Computer.CompueterState.Broken;
            findPasswordScene.isFreezing = true;

            if (!findPasswordScene.findingFirstTimeChecked)
            {
                playableDirector.Play(findPasswordScene.goFind);
                findPasswordScene.findingFirstTimeChecked = true;
                //Assigned Contained
                for (int i = 0; i < 2; i++)
                {
                    int randomPick = Random.Range(0, findPasswordScene.drawers.Count);

                    if (!findPasswordScene.drawers[randomPick].isContained)
                    {
                        findPasswordScene.drawers[randomPick].isContained = true;
                    }
                    else
                    {
                        i--;
                    }


                }
            }
            else
            {
                playableDirector.Play(findPasswordScene.goFind2);
            }
        }

        if (findPasswordScene.isFreezing)
        {
            findPasswordScene.computer.state = SC_InteractableObject_Computer.CompueterState.Broken;
            findPasswordScene.computer.textHelp.SetActive(false);
            findPasswordScene.computer.secondsHeld = 0;
        }

        //if (findPasswordScene.computer.state != SC_InteractableObject_Computer.CompueterState.Done)
        //{
        //    for (int i = 0; i < findPasswordScene.drawers.Count; i++)
        //    {
        //        if (findPasswordScene.drawers[i].isSearched && findPasswordScene.drawers[i].onPlayer && findPasswordScene.waitForFinding && findPasswordScene.isFreezing)
        //        {

        //            if (!findPasswordScene.drawers[i].isContained)
        //            {
        //                playableDirector.Play(findPasswordScene.notFoundScene);
        //            }

        //            findPasswordScene.waitForFinding = false;

        //        }
        //    }
        //}


    }

    public void DefendingPC()
    {

        if (findPasswordScene.computer.state == SC_InteractableObject_Computer.CompueterState.Active && !findPasswordScene.startedDefendPC) //started defending PC.
        {
            findPasswordScene.computer.soundOn = true;
            playableDirector.Play(findPasswordScene.text_DefendPC);
            findPasswordScene.startedDefendPC = true;


        }
        if (findPasswordScene.computer.state == SC_InteractableObject_Computer.CompueterState.Done && !findPasswordScene.FinishPcHacked)
        {
            FindObjectOfType<SC_EnemySquadSpawner>().unlimitedSpawn = false;
            playableDirector.Play(findPasswordScene.text_FinishPC);
            findPasswordScene.FinishPcHacked = true;


        }

        if (findPasswordScene.fixPCTriggered == false && findPasswordScene.computer.state == SC_InteractableObject_Computer.CompueterState.Broken && !findPasswordScene.isFreezing)
        {
            findPasswordScene.fixPCTriggered = true;
            playableDirector.Play(findPasswordScene.text_FixPC);
        }

        if (findPasswordScene.computer.state == SC_InteractableObject_Computer.CompueterState.Active && findPasswordScene.fixPCTriggered)
        {
            playableDirector.Play(findPasswordScene.text_FixP_Done);
            findPasswordScene.fixPCTriggered = false;
        }
    }

    public void ItemFound() //call from Drawer Object
    {
        //if is freezing
        if (findPasswordScene.isFreezing)
        {
            findPasswordScene.computer.toBeBroken = false;
            findPasswordScene.computer.progressCount += 1;
            findPasswordScene.computer.state = SC_InteractableObject_Computer.CompueterState.Active;
            findPasswordScene.computer.TimeToBeBroken = findPasswordScene.computer.maxTimeToBeBroken;
            playableDirector.Play(findPasswordScene.foundScene);
            findPasswordScene.frozenTime.RemoveAt(0);
            findPasswordScene.isFreezing = false;

        }

        //if is fighting boss
        if (player.GetComponent<SC_Objective_KillBoss1>() != null)
        {
            if (!player.GetComponent<SC_Objective_KillBoss1>().hasSyringe)
            {

                if (bossDrug.isFindingLast)
                {
                    Debug.Log("Play FoundLast");
                    playableDirector.Play(bossDrug.foundLast);
                    bossDrug.isFindingLast = false;
                }
                else
                {
                    playableDirector.Play(bossDrug.foundScene);
                }

                player.GetComponent<SC_Objective_KillBoss1>().hasSyringe = true;

            }
        }
        
    }

    public void ItemNotFound() //call from Drawer Object
    {

        if (bossDrug.isFindingLast)
        {
            Debug.Log("Play FoundLast");
            playableDirector.Play(bossDrug.foundLast);
            bossDrug.isFindingLast = false;
        }
        else
        {
            playableDirector.Play(findPasswordScene.notFoundScene);
        }
    }

    public void BossFightUpdate()
    {
        if (findPasswordScene.FinishPcHacked)
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
                if (endingScene.boss.GetComponent<SC_EnemyBossDrugGuy_Properties>().state == SC_EnemyBossDrugGuy_Properties.DrugBossState.Dead && !endingScene.bossDeadTriggered)
                {
                    playableDirector.Play(endingScene.bossDead);
                    endingScene.bossDeadTriggered = true;
                    endingScene.endLevelTrigger.SetActive(true);
                }

                if (endingScene.bossDeadTriggered)
                {
                    if (Physics2D.OverlapCircle(endingScene.endLevelTrigger.transform.position, 0.01f, LayerMask.GetMask("Player")) && !endingScene.LeaveLevelChecked)
                    {
                        playableDirector.Play(endingScene.leaveLevel);
                        endingScene.LeaveLevelChecked = true;
                    }
                }
            }

        }
    }

    public void FindingDrug()
    {
        if (player.GetComponent<SC_Objective_KillBoss1>() != null)
        {
            var killBossObjective = player.GetComponent<SC_Objective_KillBoss1>();
            var bossProp = endingScene.boss.GetComponent<SC_EnemyBossDrugGuy_Properties>();

            if (bossProp.DHPLocked && !bossDrug.isFinding) //when boss is executed for first time
            {
                Debug.Log("Play GoFind");
                playableDirector.Play(bossDrug.goFind);
                bossDrug.isFinding = true;



                for (int i = 0; i < bossDrug.drawers_Drug.Count; i++)
                {
                    bossDrug.drawers_Drug[i].isObjectiveActive = true;
                }

                //Assigned Contained
                for (int i2 = 0; i2 < 3; i2++)
                {
                    int randomPick2 = Random.Range(0, bossDrug.drawers_Drug.Count);

                    if (!bossDrug.drawers_Drug[randomPick2].isContained)
                    {
                        bossDrug.drawers_Drug[randomPick2].interactable = true;
                        bossDrug.drawers_Drug[randomPick2].isContained = true;
                    }
                    else
                    {
                        i2--;
                    }
                }
            }

            if (bossDrug.isFinding)
            {
                for (int i = 0; i < bossDrug.drawers_Drug.Count; i++)
                {
                    bossDrug.drawers_Drug[i].isObjectiveActive = !killBossObjective.hasSyringe;
                }
            }

            if (killBossObjective.hasSyringe && !bossDrug.hasFound)
            {
                Debug.Log("Play FOUND");
                playableDirector.Play(bossDrug.foundScene);
                bossDrug.hasFound = true;
            }

            //if (!killBossObjective.hasSyringe && bossProp.state != SC_EnemyBossDrugGuy_Properties.DrugBossState.Dead)
            //{
            //    bossDrug.hasFound = false;
            //}

            for (int i = 0; i < bossDrug.drawers_Drug.Count; i++)
            {
                if (bossDrug.drawers_Drug[i].isSearched)
                {
                    bossDrug.drawers_Drug.RemoveAt(i);
                }
            }

            if (killBossObjective.hasSyringe && player.GetComponent<SC_PlayerProperties>().HP == 0)
            {
                playableDirector.Play(bossDrug.droppedSyringeScene);
            }

            if (bossDrug.drawers_Drug.Count == 0 && !bossDrug.isFindingLast && bossProp.currentPhase == 4 & bossDrug.lastDrugLocation != null)
            {
                Debug.Log("Play GoFindLast");

                playableDirector.Play(bossDrug.goFindLast);
                bossDrug.lastDrugLocation.tag = "InteractableObject";
                bossDrug.lastDrugLocation.isObjectiveActive = true;
                bossDrug.lastDrugLocation.interactable = true;


                bossDrug.isFindingLast = true;
            }

            if (bossDrug.lastDrugLocation.isSearched && !bossDrug.hasFound)
            {
                //Debug.Log("Play FoundLast");

                //playableDirector.Play(bossDrug.foundLast);
                bossDrug.hasFound = true;
            }

            if (bossDrug.isFindingLast)
            {
                //bossDrug.reminderCountdown -= Time.deltaTime;
                //if (bossDrug.reminderCountdown <= 0)
                //{
                //    bossDrug.reminderCountdown = bossDrug.maxReminderCountdown;
                //    playableDirector.Play(bossDrug.reminderScene);
                //}
            }
        }    
    }
}
