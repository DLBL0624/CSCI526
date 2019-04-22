using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartGmBtn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject MainstartPanel;
    public GameObject ChoosePanel;

    void Start()
    {
        //Button btn = this.GetComponent<Button>();
        //btn.onClick.AddListener(OnClick);

    }

    // Update is called once per frame

    //void OnClick()
    //{
    //    SceneManager.LoadScene("Main_Scene");
    //}

    public void startGame()
    {
        MainstartPanel.SetActive(false);
        ChoosePanel.SetActive(true);
        //SceneManager.LoadScene("ChooseStage");
    }

    void Update()
    {
    //    // Check if there is a touch  && Input.GetTouch(0).phase == TouchPhase.Began
    //    if (Input.touchCount > 0)
    //    {
    //        //// Check if finger is over a UI element 
    //        //if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
    //        //{
    //        //    Debug.Log("UI is touched");
    //        //    //so when the user touched the UI(buttons) call your UI methods 
    //        //    SceneManager.LoadScene("Main_Scene");
    //        //}
    //        //else
    //        //{
    //        //    Debug.Log("UI is not touched");
    //        //    //so here call the methods you call when your other in-game objects are touched 
    //        //}
    //        for (int i = 0; i < Input.touchCount; ++i)
    //        {
    //            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
    //            RaycastHit hit;
    //            if (Physics.Raycast(ray, out hit))
    //            {
    //                Debug.Log(hit.transform.name);
    //                if (hit.transform.name == "StartButton")
    //                {
    //                    SceneManager.LoadScene("Main_Scene");
    //                }

    //            }
    //        }

    //    }
    }

}
