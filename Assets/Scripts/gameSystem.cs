using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameSystem : MonoBehaviour
{
    public Text round;
    public HexGameUI hexGameUI;
    public HexMapEditor hexMapEditor;
    public SaveLoadMenu saveLoadMenu;
    public CheckVideoStop checkVideoStop;
    public Button quitButton, roundEndButton;

    // Start is called before the first frame update
    void Start()
    {
        round.text = roundManager.getRound().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //save/load

    //round
    public void switchTurn()
    {
        roundManager.switchTurn();
        round.text = roundManager.getRound().ToString();
        hexGameUI.transform.GetChild(0).gameObject.SetActive(false);
        roundEndButton.gameObject.SetActive(false);
    }
}
