using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

    private HexCell currentCell;//英文说不明白了。。。储存棋子的原始位置，用于cancel使棋子回到上一个位置

    private HexCell targetCell;

    

    public HexGrid hexGrid;

    bool xunluOK = false;
    bool cundangOK = false;
    bool gongjiOK = false;


    void Start()
    {
        
    }

    void SetSelected(int id)
    {
        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].selected = false;
            if(SelectedMark) Destroy(SelectedMark);
        }
        highlights[id].selected = true;
        
        //generate selectedMark
        choice = highlights[id];
        currentCell = choice.hexCell;
        SelectedMark = Instantiate(SelectedMark_pfb, choice.getTransform().position + new Vector3(0, 10, 0), choice.getTransform().rotation);
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
        else if ((int)choice.bs == 3)
        {
            Debug.Log("This chess has attacked ! It cannot cancel the attack command");
            return;
        }
        else
        {
            choice.hexCell = currentCell;
            choice.reloadPosition();
            choice.bs = behaviorStatus.wakeup;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && choice.bs == behaviorStatus.ready)
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
            if(choice.bs == behaviorStatus.ready)
            {
                chessMove();
            }
            else if(choice.bs == behaviorStatus.moved)
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
    rest
}
