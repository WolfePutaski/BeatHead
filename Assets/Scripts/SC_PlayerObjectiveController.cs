using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]

public class SC_PlayerObjectiveController : MonoBehaviour
{
    public string newObjectiveScriptName;
    string currnetObjectiveScriptName;
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
            showObjective = (Input.GetKey(KeyCode.Tab));
        }



        objectiveText.SetActive(showObjective);

        if (Input.GetKeyDown(KeyCode.O))
        {
            GetNewObjective();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            RemoveScript();
        }



    }

    public void GetNewObjective()
    {
        if (!hasObjective)
        {
            System.Type MyScriptType = System.Type.GetType("SC_Objective_" + newObjectiveScriptName + ",Assembly-CSharp");
            gameObject.AddComponent(MyScriptType);
            hasObjective = true;
            currnetObjectiveScriptName = newObjectiveScriptName;
            StartCoroutine(ShowNewObjective());

        }
        else
        {
            RemoveScript();
            GetNewObjective();
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

        objectiveText.GetComponent<TextMeshProUGUI>().text = newText;
        StartCoroutine(ShowNewObjective());

    }

    public void ObjectiveClear()
    {
        StartCoroutine(ShowNewObjective());
        objectiveText.GetComponent<TextMeshProUGUI>().text = string.Format("Objective is Clear!");
        RemoveScript();
    }

    public IEnumerator ShowNewObjective()
    {
        isShowingNewObjective = true;
        showObjective = true;
        yield return new WaitForSeconds(3);
        showObjective = false;

        isShowingNewObjective = false;
    }
}
