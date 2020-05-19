using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_BriefingScreen : MonoBehaviour
{
    public bool showPressKeyText;
    public GameObject PressKeyText;
    public string nextScneName;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (showPressKeyText)
            {
                loadNextScene();
            }
            else
            {
                showPressKeyText = true;
            }
        }

        PressKeyText.SetActive(showPressKeyText);
    }

    public void loadNextScene()
    {
        SceneManager.LoadScene(nextScneName);
    }
}
