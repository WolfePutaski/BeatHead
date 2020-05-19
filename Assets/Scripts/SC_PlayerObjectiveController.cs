using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]

public class SC_PlayerObjectiveController : MonoBehaviour
{
    public string newObjectiveScriptName;
    public string currnetObjectiveScriptName;
    public string currentObjDescription;
    public bool hasObjective;


    public GameObject objectiveText;
    public bool isShowingNewObjective;
    public bool showObjective;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShowingNewObjective)
        {

            showObjective = (Input.GetKey(KeyCode.Tab)) && !SC_Cheats.isPause;
        }



        objectiveText.SetActive(showObjective);

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    GetNewObjective();
        //}
        if (Input.GetKeyDown(KeyCode.I))
        {
            RemoveScript();
        }



    }

    public void GetNewObjective()
    {
        if (!hasObjective)
        {

            hasObjective = true;
            currnetObjectiveScriptName = newObjectiveScriptName;
        }

    }

    public void RemoveScript()
    {
        if(hasObjective)
        {
            System.Type MyScriptType = System.Type.GetType("SC_Objective_" + currnetObjectiveScriptName + ",Assembly-CSharp");
            Destroy(GetComponent(MyScriptType));
            hasObjective = false;
            currnetObjectiveScriptName = null;

        }

    }

    public void UpdateNewObjectiveText(string newText)
    {
        currentObjDescription = newText;
        objectiveText.GetComponent<TextMeshProUGUI>().text = newText;
        StartCoroutine(UpdateNewObjective());

    }

    public void ObjectiveClear()
    {
        objectiveText.GetComponent<TextMeshProUGUI>().text = string.Format("Objective is Clear!");
        currentObjDescription = objectiveText.GetComponent<TextMeshProUGUI>().text;

        RemoveScript();
        StartCoroutine(UpdateNewObjective());
    }

    public IEnumerator UpdateNewObjective()
    {

        isShowingNewObjective = true;
        showObjective = true;
        yield return new WaitForSeconds(3);

        if (FindObjectOfType<SC_LevelController>() && !hasObjective)
        {
            if (FindObjectOfType<SC_LevelController>().objectiveNumber + 1 < FindObjectOfType<SC_LevelController>().objectiveList.Count)
            {
                FindObjectOfType<SC_LevelController>().GoToNextObjective();
                System.Type MyScriptType = System.Type.GetType("SC_Objective_" + newObjectiveScriptName + ",Assembly-CSharp");
                gameObject.AddComponent(MyScriptType);
                GetNewObjective();
                yield return new WaitForSeconds(3);
            }


        }
        showObjective = false;

        isShowingNewObjective = false;
    }

    public void GetNewObjective_V2(string objectiveName)
    {        
        RemoveScript();
        newObjectiveScriptName = objectiveName;
        System.Type MyScriptType = System.Type.GetType("SC_Objective_" + newObjectiveScriptName + ",Assembly-CSharp");
        gameObject.AddComponent(MyScriptType);
        GetNewObjective();
        StartCoroutine(UpdateNewObjective());

    }
}
