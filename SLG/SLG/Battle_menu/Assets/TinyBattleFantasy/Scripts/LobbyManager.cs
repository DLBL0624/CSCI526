using UnityEngine;
using System.Collections;

/// <summary>
/// Lobby Manager
/// </summary>
public class LobbyManager : MonoBehaviour {
    // item prefab list
    public GameObject[] itemPrefabs;

    // tab menu button control list
    MenuControl[] menuControls;

    // tab menu body list
    WindowControl[] winControls;

    // item list
    public UIGrid[] gridControls;

    // camera
    GameObject viewCamera;

	void Start () {
        viewCamera = GameObject.Find("View Camera") as GameObject;
        menuControls = GameObject.Find("Window").transform.GetComponentsInChildren<MenuControl>();
        Transform tf = GameObject.Find("Panels").transform;
        winControls = tf.GetComponentsInChildren<WindowControl>();
        gridControls = tf.GetComponentsInChildren<UIGrid>();

        UIGrid grid = gridControls[1];
        int i = 0;
        // Listing Items
        foreach (string spriteName in itemSprites)
        {
            if (i % 23 == 1)
            {
                GameObject go = NGUITools.AddChild(grid.gameObject, itemPrefabs[1]);
                UISprite sprite = go.transform.Find("Item").GetComponent<UISprite>();
                sprite.spriteName = spriteName;
            }
            i++;
        }
        grid.Reposition();

        grid = gridControls[2];
        // Listing cards
        for (i = 0; i < 10; i++)
        {
            GameObject go = NGUITools.AddChild(grid.gameObject, itemPrefabs[2]);
            UISprite sprite = go.transform.Find("Item").GetComponent<UISprite>();
            sprite.spriteName = "f" + (i+1).ToString("000");
        }
        grid.Reposition();

        grid = gridControls[3];
        // Listing Items
        foreach (string spriteName in itemSprites)
        {
            GameObject go = NGUITools.AddChild(grid.gameObject, itemPrefabs[3]);
            UISprite sprite = go.transform.Find("Item").GetComponent<UISprite>();
            sprite.spriteName = spriteName;
        }
        grid.Reposition();

        SetMenu(0);
	}

    // Set tab menu.
    public void SetMenu(int no)
    {
        for(int i=0;i<menuControls.Length;i++)
        {
            MenuControl mc = menuControls[i];
            if (i == no) mc.OnMenu();
            else mc.OffMenu();
            WindowControl wc = winControls[i];
            if (i == no) NGUITools.SetActive(wc.gameObject, true);
            else NGUITools.SetActive(wc.gameObject, false);
        }
        if (no == 0) NGUITools.SetActive(viewCamera, true);
        else NGUITools.SetActive(viewCamera, false);
    }

    // link tab menu button event
    public void SetMenu1()
    {
        SetMenu(0);
    }

    // link tab menu button event
    public void SetMenu2()
    {
        SetMenu(1);
    }

    // link tab menu button event
    public void SetMenu3()
    {
        SetMenu(2);
    }

    // link tab menu button event
    public void SetMenu4()
    {
        SetMenu(3);
    }

    // Goto battle scene.
    public void LoadBattle()
    {
        Application.LoadLevel("Battle");
    }
	
	void Update () {
	
	}

    // Sample item image list
    string[] itemSprites = new string[]{
  "bird1"
, "bottle1"
, "gold1"
, "poison1"
, "starfish1"
, "sword1"
, "bird1"
, "bottle1"
, "gold1"
, "poison1"
, "starfish1"
, "sword1"
, "bird1"
, "bottle1"
, "gold1"
, "poison1"
, "starfish1"
, "sword1"
, "bird1"
, "bottle1"
, "gold1"
, "poison1"
, "starfish1"
, "sword1"
    };
}
