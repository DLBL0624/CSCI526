using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FalQuitBtn : MonoBehaviour
{
    void Start()
    {
        //Button btn = this.GetComponent<Button>();
        //btn.onClick.AddListener(OnClick);
    }

    // Update is called once per frame

    public void falQuit()
    {
        SceneManager.LoadScene("begin");
    }
}
