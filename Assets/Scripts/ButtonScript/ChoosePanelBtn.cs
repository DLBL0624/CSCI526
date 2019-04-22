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
    public Button mapEditor;
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
        mapEditor.onClick.AddListener(GoMapEditor);
        back.onClick.AddListener(BackToMain);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GoTutorial()
    {
        tran.SetLevelName(0);
    }
    public void GoLevel1()
    {
        tran.SetLevelName(1);
    }
    public void GoLevel2()
    {
        tran.SetLevelName(2);
    }
    public void GoLevel3()
    {
        tran.SetLevelName(3);
    }
    public void GoLevel4()
    {
        tran.SetLevelName(4);
    }
    public void GoMapEditor()
    {
        tran.SetLevelName(-1);
    }
    public void BackToMain()
    {
        MainP.SetActive(true);
        ChooseP.SetActive(false);
    }
}
