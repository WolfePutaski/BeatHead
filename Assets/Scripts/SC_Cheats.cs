using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SC_Cheats : MonoBehaviour
{
    public static bool isPause;

    public bool isFullScreen;
    public bool isMainMenu;
    public bool enableCheats;
    public bool isStageFinish;
    public string nextLevelName;
    public GameObject pauseMenu;
    public GameObject hideInMainMenuGroup;
    public int finishedLevel2;
    public GameObject StartLevel2Button;
    public bool showControls;
    public GameObject showControlsGroup;

    public AudioMixer mixer;

    [System.Serializable]
    public class SliderSetting
    {
        public Slider sfxSlider;
        public Slider musicSlider;
    }

    [System.Serializable]

    public class Cheats
    {
        public bool godModON;
    }

    public List<Resolution> resolutions;

    public SliderSetting slider;

    public Cheats cheats;

    // Start is called before the first frame update
    void Start()
    {
        slider.sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        slider.musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        finishedLevel2 = PlayerPrefs.GetInt("FinishedLevel2");
        isStageFinish = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (StartLevel2Button != null)
        {
            if (finishedLevel2 == 1)
            {
                StartLevel2Button.SetActive(true);
            }
        }
        if (enableCheats)
        {


            if (!isPause)
            {
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    RestartLevel();
                }
                if (Input.GetKeyDown(KeyCode.KeypadPlus)) //
                {
                    Time.timeScale = 20.0f;
                }
                if (Input.GetKeyUp(KeyCode.KeypadPlus))
                {
                    Time.timeScale = 1.0f;
                }

                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    if (cheats.godModON)
                    {
                        cheats.godModON = false;
                    }
                    else
                    {
                        cheats.godModON = true;

                    }
                }
                if (cheats.godModON)
                {
                    if (FindObjectOfType<SC_PlayerProperties>() != null)
                    {
                        FindObjectOfType<SC_PlayerProperties>().damage = 999;
                        FindObjectOfType<SC_PlayerProperties>().HP = FindObjectOfType<SC_PlayerProperties>().maxHP;

                    }
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (enableCheats)
            {
                enableCheats = false;
            }
            else
            {
                enableCheats = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                Resume();
            }
            else
            {
                isPause = true;
            }
        }

        if (isPause)
        {
            Time.timeScale = 0;
            //Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);

        }

            hideInMainMenuGroup.SetActive(!isMainMenu);

        if (isStageFinish)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(nextLevelName);
            }
        }
    }

    public void GoToLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void Resume()
    {
        Time.timeScale = 1;
        isPause = false;
        showControlsGroup.SetActive(false);

    }

    public void OpenSetting()
    {
        isPause = true;
    }



    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetMusicVolume(float sliderValue)
    {
        mixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mixer.SetFloat("SFX", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);

    }

    public void SetFullScreen(bool booleen)
    {
        Screen.fullScreen = booleen;

    }

    void FinishedGame()
    {
        finishedLevel2 = 1;
        PlayerPrefs.SetInt("FinishedLevel2", finishedLevel2);
    }

    public void SetResolution(int i)
    {
        Resolution resolution = resolutions[i];
        Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
    }

    public void ShowControl()
    {
        showControlsGroup.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void HideControl()
    {
        pauseMenu.SetActive(true);
        showControlsGroup.SetActive(false);
    }
}
