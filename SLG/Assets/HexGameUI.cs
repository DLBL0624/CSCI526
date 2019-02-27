using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{

    public HexGrid grid;

    HexCell currentCell;

    List<HexCell> rangeCells = new List<HexCell>();

    HexUnit targetUnit;

    HexUnit selectedUnit;

    public GameObject SelectedMark_pfb;

    private GameObject SelectedMark;

    bool showAttackRange = false;

    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        grid.ShowUI(!toggle);
        grid.ClearPath();
    }

    bool UpdateCurrentCell()
    {
        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    void DoSelection()
    {
        grid.ClearPath();
        UpdateCurrentCell();
        if (currentCell)
        {
            selectedUnit = currentCell.Unit;
        }
    }
    void DoTargetSelection()
    {
        UpdateCurrentCell();
        if (currentCell&&currentCell.Unit&&rangeCells.Contains(currentCell)&&selectedUnit.checkTeam(currentCell))
        {
            targetUnit = currentCell.Unit; 
        }
        
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(showAttackRange == false)
                {
                    DoSelection();
                }
                else
                {
                    DoTargetSelection();
                }
            }
            else if (selectedUnit)//已选中目标
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if(showAttackRange == false)
                    {
                        DoMove();//走位
                    }
                    else
                    {
                        DoAttack();
                    }
                    
                }
                else
                {
                    if(showAttackRange == false)
                    {
                        DoPathfinding();//找路径
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && selectedUnit.isValidDestination(currentCell))
            {
                grid.FindPath(selectedUnit.Location, currentCell, 24);
        }
            else
            {
                grid.ClearPath();
            }
        }
    }

    void DoMove()
    {
        if (grid.HasPath)
        {
            selectedUnit.Travel(grid.GetPath());
            grid.ClearPath();
        }
    }

    public void OnAttack()
    {
        if(selectedUnit)
        {
            if(showAttackRange==false)
            {
                ShowAttackCell(true);
            }
            else
            {
                ShowAttackCell(false);
            }
            //选择攻击目标
            showAttackRange = !showAttackRange;
        }
        
    }

    void DoAttack()
    {
        if(targetUnit)
        {
            ShowAttackCell(false);
            selectedUnit.Fight(targetUnit);
            showAttackRange = false;
            Debug.Log("showAttackRange" + showAttackRange);
            Debug.Log(rangeCells.Count);
            targetUnit = null;
        }
    }

    void ShowAttackCell(bool enable)
    {
        //假设距离为一
        if(enable)
        {
            grid.ClearPath();
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = selectedUnit.Location.GetNeighbor(d);
                rangeCells.Add(neighbor);
                neighbor.EnableHighlight(Color.yellow);
            }
        }
        else
        {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = selectedUnit.Location.GetNeighbor(d);
                neighbor.DisableHighlight();
                
            }
            rangeCells.Clear();
        }
        
    }

    //public void RemoveSelectedMark()
    //{
    //    for (int i = 0; i < friends.Length; i++)
    //    {
    //        friends[i].selected = false;
    //        if (SelectedMark) Destroy(SelectedMark);
    //    }
    //}

    //public void generateSelectedMark()
    //{
    //    SelectedMark = Instantiate(SelectedMark_pfb, selectedUnit.transform.position + new Vector3(0, 10, 0), selectedUnit.transform.rotation);
    //}

}