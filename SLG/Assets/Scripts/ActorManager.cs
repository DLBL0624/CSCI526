using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Game Manager Main Code
/// </summary>
public class ActorManager : MonoBehaviour
{
    // actor list
    public ChoiceActor[] highlights;

    private ChoiceActor choice;

    public GameObject SelectedMark_pfb;

    private GameObject SelectedMark;

    private HexCell currentCell;

    private HexCell targetCell;

    public GameObject chessAttr;

    public HexGrid hexGrid;

    bool xunluOK = false;
    bool cundangOK = false;
    bool gongjiOK = false;


    void Start()
    {
        for(int i = 0; i<highlights.Length; i++)
        {
            highlights[i].setID(i);
        }
    }

    public void RemoveSelectedMark()
    {
        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].selected = false;
            if (SelectedMark) Destroy(SelectedMark);
        }
    }

    public void generateSelectedMark()
    {
        SelectedMark = Instantiate(SelectedMark_pfb, choice.getTransform().position + new Vector3(0, 10, 0), choice.getTransform().rotation);
    }

    void SetSelected(int id)
    {
        RemoveSelectedMark();
        highlights[id].selected = true;
        
        //generate selectedMark
        choice = highlights[id];
        currentCell = choice.hexCell;
        generateSelectedMark();
        showChessAttribute();
    }

    // attack event from button
    public void Move()
    {
        if (!choice) return;
        else if ((int)choice.bs>1)
        {
            Debug.Log("This chess has already Moved!");
            return;
        }
        else
        {
            Debug.Log("Please select a position!");
            choice.bs = behaviorStatus.ready;
        }
        
    }

    // attack event from button
    public void Attack()
    {
        if (!choice) return;
        if (choice.bs==behaviorStatus.rest)
        {
            Debug.Log("It has already attacked!");
        }
        else
        {
            choice.bs = behaviorStatus.attackready;
        }

    }

    // attack event from button
    public void Cancel()
    {
        if (!choice) return;
        else if ((int)choice.bs < 2)
        {
            Debug.Log("This chess didn't do any thing!");
            return;
        }
        else if (choice.bs == behaviorStatus.rest)
        {
            Debug.Log("This chess has attacked ! It cannot cancel the attack command");
            return;
        }
        else
        {
            choice.hexCell = currentCell;
            choice.reloadPosition();
            choice.bs = behaviorStatus.wakeup;
            RemoveSelectedMark();
            generateSelectedMark();
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() &&choice&&choice.bs == behaviorStatus.ready)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            targetCell = hexGrid.GetCell(hit.point);
            if(choice.bs == behaviorStatus.ready&&choice.hexCell!=targetCell)
            {
                chessMove();
            }
            else if(choice.bs == behaviorStatus.attackready)
            {//for attack

            }
        }
        
    }

    void chessMove()
    {
        if (xunluOK)
        {
            xunluAlgorithm(choice.hexCell, targetCell);
        }
        else
        {
            moveToPositionByNormal(choice.hexCell, targetCell);
        }
        choice.bs = behaviorStatus.moved;
        choice.hexCell = targetCell;
        choice.reloadPosition();
        //reset the Marker;
        RemoveSelectedMark();
        generateSelectedMark();
    }

    void chessAttack()
    {

        choice.bs = behaviorStatus.rest;
        
    }
    //apply xun lu Algorithm to find the path and chess will move follow the path
    private void xunluAlgorithm(HexCell a, HexCell b)
    {

    }

    private void moveToPositionByNormal(HexCell a, HexCell b)
    {
        Vector3 offSet = b.Position - a.Position;
        Vector3 norm = offSet.normalized;
        while (Vector3.Distance(choice.getTransform().position, b.Position)> HexMetrics.outerRadius + 2f)
        {
            choice.getTransform().position += norm * HexMetrics.outerRadius * Time.deltaTime;
        }
    }

    public void showChessAttribute()
    {
        chessAttr.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = choice.gameObject.name;
        chessAttr.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = choice.hp.ToString();
        chessAttr.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = choice.att.ToString();
        chessAttr.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = choice.bs.ToString();
    }

    //// goto main menu scene
    //public void LoadMenu()
    //{
    //    Application.LoadLevel("Menu");
    //}
}

public enum behaviorStatus
{
    wakeup,
    ready,
    moved,
    attackready,
    rest
}
