using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class whichLevel : MonoBehaviour
{
    public string LevelName;
    public string levelPath;
    private int levelIndex;
    

    private string[] LevelNameSet =
    {
        "tutorial",
        "level1",
        "level2",
        "level3",
        "level4"
    };

    private string[] levelIntro =
    {
        "Tutorial_Intro",
        "Level1_Intro",
        "Level2_Intro",
        "Level3_Intro",
        "Level4_Intro"
    };

    public GameObject gameManager;
    public GameObject endController;
    public GameObject nextButton;
    public GameObject introNextButton;

    private gameSystem gs;
    private EndControl ec;
    private Button nextButtonClick;
    private Button introNextButtonClick;
    private Button quitButtonClick;



    public bool isLoading = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        searchSceneTarget();
        loadTarget();
        //checkMultiple();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);
        isLoading = true;
    }

    public void SetLevelName(int index)
    {
        levelIndex = index;
        if (levelIndex != -1) LevelName = LevelNameSet[index];
        else LevelName = "@MapEditor";
        if (LevelName != "@MapEditor") levelPath = Application.dataPath + @"/Resources/" + LevelName + ".map";
        else levelPath = "@MapEditor";

        if(levelIndex!=-1)SwitchToIntroScene();
        else SwitchToMainScene();
    }

    public void SwitchToNextLevel()
    {
        levelIndex++;
        if(levelIndex<=LevelNameSet.Length)
        {
            SetLevelName(levelIndex);
        }
    }

    public void SwitchToMainScene()
    {
        
        SceneManager.LoadScene("Main_Scene");
        clearSceneTarget();
        
    }

    public void SwitchToIntroScene()
    {
        
        SceneManager.LoadScene(levelIntro[levelIndex]);
        clearSceneTarget();
        
    }

    public void SwitchToBeginScene()
    {
        SceneManager.LoadScene("begin");
        Destroy(this.gameObject);
    }

    public void SwitchToBackgroundIntroScene()
    {
        SceneManager.LoadScene("Background_Intro");
        clearSceneTarget();
    }

    public void SwitchToVictoryScene()
    {
        SceneManager.LoadScene("Victory");
        clearSceneTarget();
    }

    public void SwitchToFailureScene()
    {
        SceneManager.LoadScene("Failure");
        clearSceneTarget();
    }

    private void searchSceneTarget()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        //quitButton = GameObject.FindGameObjectWithTag("QuitButton");
        nextButton = GameObject.FindGameObjectWithTag("NextButton");
        introNextButton = GameObject.FindGameObjectWithTag("IntroNextButton");
        endController = GameObject.FindGameObjectWithTag("EndController");
    }

    private void loadTarget()
    {
        if (isLoading)
        {
            if (gameManager)
            {
                Debug.Log("GameManager IN!");
                isLoading = false;
                gs = gameManager.GetComponent<gameSystem>();
                if (levelPath != "@MapEditor")
                {
                    gs.saveLoadMenu.Load(levelPath);
                    gs.hexMapEditor.gameObject.SetActive(false);
                }
                else
                {
                    gs.hexMapEditor.gameObject.SetActive(true);
                }

                quitButtonClick = gs.quitButton;
                    //quitButtonClick = quitButton.GetComponent<Button>();
                    quitButtonClick.onClick.AddListener(SwitchToBeginScene);
            }
            else if (endController)
            {
                isLoading = false;
                ec = endController.GetComponent<EndControl>();
                nextButtonClick = ec.nextButton;
                quitButtonClick = ec.quitButton;
                if (ec.end == 1 && levelIndex == LevelNameSet.Length - 1)
                {
                    ec.nextButton.gameObject.SetActive(false);
                }
                else
                {
                    ec.nextButton.gameObject.SetActive(true);
                }
                if(ec.end==1)
                {
                    nextButtonClick.onClick.AddListener(SwitchToNextLevel);
                }
                if(ec.end==0)
                {
                    nextButtonClick.onClick.AddListener(SwitchToMainScene);
                }
                quitButtonClick.onClick.AddListener(SwitchToBeginScene);

            }
            else if (nextButton)
            {
                isLoading = false;
                nextButtonClick = nextButtonClick.GetComponent<Button>();
                nextButtonClick.onClick.AddListener(SwitchToNextLevel);
            }
            else if (introNextButton)
            {
                isLoading = false;
                introNextButtonClick = introNextButton.GetComponent<Button>();
                introNextButtonClick.onClick.AddListener(SwitchToMainScene);
            }
            
        }
    }

    private void clearSceneTarget()
    {
        gameManager = null;
        gs = null;
        endController = null;
        ec = null;
        nextButton = null;
        introNextButton = null;
        nextButtonClick = null;
        introNextButtonClick = null;
        quitButtonClick = null;
    }

    private void checkMultiple()
    {
        if(GameObject.FindGameObjectsWithTag("SwitchLevel").Length>1)
        {
            Destroy(this.gameObject);
        }
    }
}
