using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChoosePanelBtn : MonoBehaviour
{
    public whichLevel tran;
    public Button tutorial;
    public Button level1;
    public Button level2;
    public Button level3;
    public Button level4;
    public Button back;

    public GameObject MainP;
    public GameObject ChooseP;

    // Start is called before the first frame update
    void Start()
    {
        tutorial.onClick.AddListener(GoTutorial);
        level1.onClick.AddListener(GoLevel1);
        level2.onClick.AddListener(GoLevel2);
        level3.onClick.AddListener(GoLevel3);
        level4.onClick.AddListener(GoLevel4);
        back.onClick.AddListener(BackToMain);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GoTutorial()
    {
        tran.SetLevelname("tutorial");
        SceneManager.LoadScene("Main_Scene");
    }
    public void GoLevel1()
    {
        tran.SetLevelname("level1");
        SceneManager.LoadScene("Main_Scene");
    }
    public void GoLevel2()
    {
        tran.SetLevelname("level2");
        SceneManager.LoadScene("Main_Scene");
    }
    public void GoLevel3()
    {
        tran.SetLevelname("level3");
        SceneManager.LoadScene("Main_Scene");
    }
    public void GoLevel4()
    {
        tran.SetLevelname("level4");
        SceneManager.LoadScene("Main_Scene");
    }
    public void BackToMain()
    {
        MainP.SetActive(true);
        ChooseP.SetActive(false);
    }
}
