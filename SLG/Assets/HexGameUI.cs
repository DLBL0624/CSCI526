using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexGameUI : MonoBehaviour
{

    public HexGrid grid;

    HexCell currentCell;

    List<HexCell> rangeCells = new List<HexCell>();

    HexUnit targetUnit;

    HexUnit selectedUnit;

    bool isMalganisDead = false;

    bool isArthasDead = false;

    //HexMapCamera hexMapCamera;

    //public GameObject SelectedMark_pfb;

    //private GameObject SelectedMark;

    public CharacterStatus statusWindow;

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
            if(selectedUnit)statusWindow.showUnitStatus(selectedUnit);
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

    public void AIDoSelection(HexUnit enemyUnit, HexUnit friendUnit, int speed)
    {
        selectedUnit = enemyUnit;
        checkPos(friendUnit);
        targetUnit = friendUnit;
        if (currentCell && selectedUnit.isValidDestination(currentCell))
        {
            grid.FindPath(selectedUnit.Location, currentCell, speed, true);
        }
        else
        {
            grid.ClearPath();
        }

        if(!checkNeighbor(selectedUnit.Location, friendUnit.Location))DoMove();//移动
        if(targetUnit) ShowAttackCell(true);
        DoAttack();//攻击
    }

    public bool checkNeighbor(HexCell c1, HexCell c2)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = c1.GetNeighbor(d);
            if(neighbor == c2)
            {
                return true;
            }
        }
        return false;
    }

    public void checkPos(HexUnit friendUnit)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Debug.Log("Checking " + "friendUnit " + friendUnit.Location.coordinates);
            HexCell neighbor = friendUnit.Location.GetNeighbor(d);
            HexEdgeType edgeType = friendUnit.Location.GetEdgeType(neighbor);
            Debug.Log(edgeType);
            if (neighbor == null)
            {
                continue;
            }
            if (neighbor.IsUnderwater || neighbor.Unit)
            {
                continue;
            }
            if (edgeType == HexEdgeType.Cliff)
            {
                continue;
            }
            if (friendUnit.Location.Walled != neighbor.Walled)
            {
                continue;
            }
            currentCell = neighbor;//找能到的cell
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
            else if (selectedUnit&&selectedUnit.UnitAttribute.team==0)//已选中目标
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if(showAttackRange == false)
                    {
                        if(selectedUnit.UnitAttribute.bs != behaviorStatus.moved)
                        {
                            DoMove();//走位
                        }
                    }
                    else
                    {
                        if(selectedUnit.UnitAttribute.bs != behaviorStatus.rest)
                        {
                            DoAttack();
                        }
                    }
                }
                else
                {
                    if(showAttackRange == false&&selectedUnit.UnitAttribute.bs == behaviorStatus.wakeup)
                    {
                        DoPathfinding(selectedUnit.UnitAttribute.ap);//找路径
                    }
                    else
                    {

                    }
                }
            }
        }
        if(isMalganisDead)
        {
            //victory
            SceneManager.LoadScene("Victory");
        }
        if(isArthasDead)
        {
            //fail
            SceneManager.LoadScene("Failure");
        }
    }

    void DoPathfinding(int speed)
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && selectedUnit.isValidDestination(currentCell))
            {
                grid.FindPath(selectedUnit.Location, currentCell, speed);
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
            selectedUnit.UnitAttribute.bs = behaviorStatus.moved;
        }
    }

    public void OnAttack()
    {
        if(selectedUnit&&selectedUnit.UnitAttribute.team==0&&selectedUnit.UnitAttribute.bs!=behaviorStatus.rest)
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
            checkDie(selectedUnit);
            checkDie(targetUnit);
            showAttackRange = false;
            if(selectedUnit)statusWindow.showUnitStatus(selectedUnit);
            targetUnit = null;
            
            selectedUnit.UnitAttribute.bs = behaviorStatus.rest;
        }
    }

    void checkDie(HexUnit hu)
    {
        if (hu.UnitAttribute.hp <= 0)
        {
            if (hu.UnitAttribute.actorName == "Arthas")
            {
                isArthasDead = true;
            }
            if (hu.UnitAttribute.actorName == "Malganis")
            {
                isMalganisDead = true;
            }
            grid.unitManager.removeUnit(hu);
            hu.Die();
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

}