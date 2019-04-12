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

    bool showSpellRange = false;

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
            //若已有unit
            if(selectedUnit)
            {
                //相同unit 双击取消
                if(selectedUnit==currentCell.Unit)
                {
                    selectedUnit = null;
                    statusWindow.showUnitStatus(null);
                    
                    return;
                }
            }
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
    //用于攻击操作
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(showAttackRange == false&&showSpellRange == false)
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
                    if(showAttackRange == false && showSpellRange ==false)
                    {
                        if(selectedUnit.UnitAttribute.bs != behaviorStatus.moved)
                        {
                            DoMove();//走位
                        }
                    }
                    else if(showAttackRange == true)
                    {
                        if(selectedUnit.UnitAttribute.bs != behaviorStatus.rest)
                        {
                            DoAttack();
                        }
                    }
                    else if(showSpellRange == true)
                    {
                        if (selectedUnit.UnitAttribute.bs != behaviorStatus.rest)
                        {
                            Skill selectedUnitSkill = selectedUnit.UnitAttribute.activeSkill;
                            if(selectedUnitSkill!=null)
                            {
                                if(selectedUnitSkill.TargetItself)
                                {
                                    if(selectedUnitSkill.Spellable)
                                    {
                                        targetUnit = selectedUnit;
                                        DoSpell();
                                    }
                                }
                                else
                                {
                                    if(selectedUnitSkill.Spellable)
                                    {
                                        if (targetUnit)
                                        {
                                            if (targetUnit.UnitAttribute.team == selectedUnitSkill.TargetTeam)
                                            {
                                                DoSpell();
                                            }
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
                else
                {
                    if(showAttackRange == false&&showSpellRange == false&&selectedUnit.UnitAttribute.bs == behaviorStatus.wakeup)
                    {
                        DoPathfinding(selectedUnit.UnitAttribute.Ap);//找路径
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

    //选择攻击目标
    public void OnAttack()
    {
        if(selectedUnit&&selectedUnit.UnitAttribute.team==0&&selectedUnit.UnitAttribute.bs!=behaviorStatus.rest)
        {
            showSpellRange = false;
            ShowSpellCell(false);
            if (showAttackRange==false)
            {
                ShowAttackCell(true);
            }
            else
            {
                ShowAttackCell(false);
            }
            showAttackRange = !showAttackRange;
        }
        
    }

    //选择施法目标
    public void OnSpell()
    {
        if(selectedUnit)
        {
            if (selectedUnit.UnitAttribute.actiSkillNumber != -1)
            {
                Skill activeSkill = selectedUnit.UnitAttribute.activeSkill;
                //目标选择条件
                if (selectedUnit.UnitAttribute.team == 0 && selectedUnit.UnitAttribute.bs != behaviorStatus.rest)
                {
                    showAttackRange = false;
                    ShowAttackCell(false);
                    if (showSpellRange == false)
                    {
                        ShowSpellCell(true);
                    }
                    else
                    {
                        ShowSpellCell(false);
                    }
                    showSpellRange = !showSpellRange;
                }
            }
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

    void DoSpell()
    {
        ShowSpellCell(false);
        selectedUnit.Spell(targetUnit);
        checkDie(selectedUnit);
        checkDie(targetUnit);
        showSpellRange = false;
        if (selectedUnit) statusWindow.showUnitStatus(selectedUnit);
        targetUnit = null;
        selectedUnit.UnitAttribute.bs = behaviorStatus.rest;
    }

    void checkDie(HexUnit hu)
    {
        if(hu == null)
        {
            //if hu 不存在
            return;
        }
        if (hu.UnitAttribute.hp <= 0)
        {
            if (hu.UnitAttribute.hpMin == 0)
            {
                //胜利判断条件 --- 后续需要更改
                //TODO -> change the victory condition
                // this is just an example for level tutorial
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
            //用于检测克罗米被动 (第一次死生命回到1)
            else if (hu.UnitAttribute.hpMin > 0)
            {
                hu.UnitAttribute.hp = hu.UnitAttribute.hpMin;
                hu.UnitAttribute.hpMin = 0;
            }
            
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


    void ShowSpellCell(bool enable)
    {
        //假设距离为一
        if (enable)
        {
            grid.ClearPath();
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = selectedUnit.Location.GetNeighbor(d);
                rangeCells.Add(neighbor);
                neighbor.EnableHighlight(Color.cyan);
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