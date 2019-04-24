using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexGameUI : MonoBehaviour
{

    public HexGrid grid;

    public AIManager aIManager;

    HexCell currentCell;

    List<HexCell> rangeCells = new List<HexCell>();

    List<HexCell> centerCells = new List<HexCell>();

    HexUnit targetUnit;

    HexUnit selectedUnit;

    bool isMalganisDead = false;

    bool isArthasDead = false;

    public CharacterStatus statusWindow;

    public CharacterStatus targetWindow;

    public CheckVideoStop checkVideoStop;

    bool showAttackRange = false;

    bool showSpellRange = false;

    public Text spellText;


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
        if (selectedUnit) selectedUnit.isSelected = false;
        grid.ClearPath();
        UpdateCurrentCell();
        if (currentCell)
        {
            selectedUnit = currentCell.Unit;

            if (selectedUnit)
            {
                statusWindow.showUnitStatus(selectedUnit);
                selectedUnit.isSelected = true;
                if (selectedUnit.UnitAttribute.activeSkill != null)
                {
                    if (selectedUnit.UnitAttribute.activeSkill.Spellable)
                    {
                        spellText.color = new Color(171f / 255f, 174f / 255f, 57f / 255f);
                        spellText.text = "Spell";
                    }
                    else
                    {
                        spellText.color = Color.gray;
                        spellText.text = "CoolDown: " + (selectedUnit.UnitAttribute.activeSkill.CoolDown - (selectedUnit.UnitAttribute.activeSkill.RecentTurn - selectedUnit.UnitAttribute.activeSkill.StartTurn)).ToString();
                    }
                }
                else
                {
                    spellText.color = new Color(171f / 255f, 174f / 255f, 57f / 255f);
                    spellText.text = "Spell";
                }
            }
            else
            {
                statusWindow.showUnitStatus(null);
                spellText.color = new Color(171f / 255f, 174f / 255f, 57f / 255f);
                spellText.text = "Spell";
            }
        }
        targetWindow.showUnitStatus(null);
    }

    void DoTargetSelection()
    {
        UpdateCurrentCell();
        if (currentCell && currentCell.Unit && rangeCells.Contains(currentCell))
        {
            targetUnit = currentCell.Unit;
            if (targetUnit) targetWindow.showUnitStatus(targetUnit);
            else targetWindow.showUnitStatus(null);
        }
        
    }

    public IEnumerator AIDoSelection(HexUnit enemyUnit, HexUnit friendUnit, int speed)
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
        if (targetUnit)
        {
            //攻击范围
            ShowRangeCell(true, 1);
        }
        while(selectedUnit.isQunar)
        {
            yield return null;
        }
        yield return StartCoroutine(DoAttack());//攻击
        statusWindow.showUnitStatus(null);
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
            //Debug.Log(edgeType);
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
                //未在施法或攻击状态
                if(showAttackRange == false && showSpellRange == false)
                {
                    //选择目标
                    if(!aIManager.isAIworking)
                    {
                        DoSelection();
                    }
                }
                else
                {
                    if (!aIManager.isAIworking)
                    {
                        DoTargetSelection();
                    }
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
                        if(targetUnit
                            && selectedUnit.UnitAttribute.bs != behaviorStatus.rest 
                            && selectedUnit.checkTeam(targetUnit.Location))
                        {
                            StartCoroutine(DoAttack());
                        }
                    }
                    else if(showSpellRange == true)
                    {
                        //当前角色未进行过攻击、施法
                        if (selectedUnit.UnitAttribute.bs != behaviorStatus.rest)
                        {
                            Skill selectedUnitSkill = selectedUnit.UnitAttribute.activeSkill;
                            //当前角色有主动技能
                            if(selectedUnitSkill!=null)
                            {
                                //如果当前角色主动技能只能对自己释放
                                if(selectedUnitSkill.TargetItself)
                                {
                                    //当前角色不在冷却中
                                    if(selectedUnitSkill.Spellable)
                                    {
                                        targetUnit = selectedUnit;
                                        StartCoroutine(DoSpell());
                                    }
                                }
                                //如果当前角色主动技能对其他人释放
                                else
                                {
                                    //当前角色不在冷却中
                                    
                                    if (selectedUnitSkill.Spellable)
                                    {
                                        Debug.Log("//当前角色不在冷却中");
                                        //已选中目标
                                        if (targetUnit)
                                        {
                                            Debug.Log("//有目标");
                                            if (selectedUnitSkill.NeedBehavior)
                                            {
                                                
                                                Debug.Log("目标" + targetUnit.UnitAttribute.name + "状态 - >" + targetUnit.UnitAttribute.bs + "所需状态 - >" + selectedUnitSkill.TargetBehaviour);
                                                if (targetUnit.UnitAttribute.bs == selectedUnitSkill.TargetBehaviour)
                                                {
                                                    if (targetUnit.UnitAttribute.team == selectedUnitSkill.TargetTeam)
                                                    {
                                                        StartCoroutine(DoSpell());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (targetUnit.UnitAttribute.team == selectedUnitSkill.TargetTeam)
                                                {
                                                    StartCoroutine(DoSpell());
                                                }
                                            
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
            ShowRangeCell(false, 0);//隐藏施法范围
            if (showAttackRange==false)
            {
                ShowRangeCell(true,1);//显示攻击范围
            }
            else
            {
                ShowRangeCell(false, 1);//隐藏攻击范围
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
                if (selectedUnit.UnitAttribute.team == 0 && selectedUnit.UnitAttribute.bs != behaviorStatus.rest && activeSkill.Spellable)
                {
                    showAttackRange = false;
                    ShowRangeCell(false,1);//隐藏攻击范围
                    if (showSpellRange == false)
                    {
                        ShowRangeCell(true,0);//显示施法范围
                    }
                    else
                    {
                        ShowRangeCell(false,0);//隐藏施法范围
                    }
                    showSpellRange = !showSpellRange;
                }
            }
        }
        
    }

    IEnumerator DoAttack()
    {
        if(targetUnit)
        {
            selectedUnit.UnitAttribute.bs = behaviorStatus.rest;
            ShowRangeCell(false,1);//隐藏攻击范围
            selectedUnit.Fight(targetUnit);
            //targetUnit.Wound(selectedUnit);
            
            while(targetUnit&&(selectedUnit.isQunar||targetUnit.isQunar))
            {
                Debug.Log("halting!");
                yield return null;
            }
            if (targetUnit&&targetUnit.UnitAttribute.hp > 0
                && HexMetrics.FindDistanceBetweenCells(selectedUnit.Location, targetUnit.Location) <= targetUnit.UnitAttribute.maxAttRange
                && HexMetrics.FindDistanceBetweenCells(selectedUnit.Location, targetUnit.Location) >= targetUnit.UnitAttribute.minAttRange
                )
            {
                targetUnit.Fight(selectedUnit);
                //如果我方比对方速度快15以上 追加攻击
                while (targetUnit && (selectedUnit.isQunar || targetUnit.isQunar))
                {
                    Debug.Log("halting!");
                    yield return null;
                }
                if (selectedUnit.UnitAttribute.Sp >= targetUnit.UnitAttribute.Sp + 15)
                {
                    selectedUnit.Fight(targetUnit);
                }
                //如果对方比我方速度快15以上 对方追加攻击
                else if (targetUnit.UnitAttribute.Sp >= selectedUnit.UnitAttribute.Sp + 15)
                {
                    targetUnit.Fight(selectedUnit);
                }
                if (selectedUnit) statusWindow.showUnitStatus(selectedUnit);
                if (targetUnit) targetWindow.showUnitStatus(targetUnit);
            }
            else if (targetUnit && targetUnit.UnitAttribute.hp > 0
                && (HexMetrics.FindDistanceBetweenCells(selectedUnit.Location, targetUnit.Location) > targetUnit.UnitAttribute.maxAttRange
                    || HexMetrics.FindDistanceBetweenCells(selectedUnit.Location, targetUnit.Location) < targetUnit.UnitAttribute.minAttRange)
                )
            {
                while (targetUnit && (selectedUnit.isQunar || targetUnit.isQunar))
                {
                    Debug.Log("halting!");
                    yield return null;
                }
                if (selectedUnit.UnitAttribute.Sp >= targetUnit.UnitAttribute.Sp + 15)
                {
                    selectedUnit.Fight(targetUnit);
                }
                if (selectedUnit) statusWindow.showUnitStatus(selectedUnit);
                if (targetUnit) targetWindow.showUnitStatus(targetUnit);
            }
            //checkDie(selectedUnit);
            //checkDie(targetUnit);
            showAttackRange = false;
            if(selectedUnit)statusWindow.showUnitStatus(selectedUnit);
            if (targetUnit) targetWindow.showUnitStatus(targetUnit);
            targetUnit = null;
            
        }
        spellText.color = new Color(171f / 255f, 174f / 255f, 57f / 255f);
        spellText.text = "Spell";
    }

    IEnumerator DoSpell()
    {
        selectedUnit.UnitAttribute.bs = behaviorStatus.rest;
        ShowRangeCell(false,0);//隐藏施法范围
        selectedUnit.Spell(targetUnit);
        showSpellRange = false;
        if (selectedUnit) statusWindow.showUnitStatus(selectedUnit);
        if (targetUnit) targetWindow.showUnitStatus(targetUnit);
        targetUnit = null;

        spellText.color = Color.gray;
        spellText.text = "CoolDown: " + (selectedUnit.UnitAttribute.activeSkill.CoolDown - (selectedUnit.UnitAttribute.activeSkill.RecentTurn - selectedUnit.UnitAttribute.activeSkill.StartTurn)).ToString();
        yield return new WaitForSeconds(1f);
        if (selectedUnit) statusWindow.showUnitStatus(selectedUnit);
        if (targetUnit) targetWindow.showUnitStatus(targetUnit);
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
                //if (hu.UnitAttribute.actorName == "Arthas")
                //{
                //    isArthasDead = true;
                //}
                //if (hu.UnitAttribute.actorName == "Malganis")
                //{
                //    isMalganisDead = true;
                //}
                //grid.unitManager.removeUnit(hu);
                hu.Die(1);//有动画的die
            }
            //用于检测克罗米被动 (第一次死生命回到1)
            else if (hu.UnitAttribute.hpMin > 0)
            {
                hu.UnitAttribute.hp = hu.UnitAttribute.hpMin;
                hu.UnitAttribute.hpMin = 0;
            }
        }
    }


    void ShowRangeCell(bool enable, int model)
    {
        // model -> 0 => 施法 
        // model -> 1 => 攻击
        if (enable)
        {
            grid.ClearPath();
            //迭代遍历 -> 用于寻找最大maxRange
            if (model == 0)
            {
                showMaxRangeIterator(selectedUnit.Location, model, selectedUnit.UnitAttribute.maxSplRange);
                centerCells.Clear();
                showMinRangeIterator(selectedUnit.UnitAttribute.minSplRange);
            }
            else if (model == 1)
            {
                showMaxRangeIterator(selectedUnit.Location, model, selectedUnit.UnitAttribute.maxAttRange);
                centerCells.Clear();
                showMinRangeIterator(selectedUnit.UnitAttribute.minAttRange);
            }
        }
        else
        {
            foreach (HexCell cel in rangeCells)
            {
                cel.DisableHighlight();
            }
            rangeCells.Clear();
        }
    }

    public void showMaxRangeIterator(HexCell center, int model, int maxRange)
    {
        //Debug.Log("MaxRangeIterator -> ");
        if(centerCells.Contains(center))
        {
            return;
        }
        centerCells.Add(center);
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            
            HexCell neighbor = center.GetNeighbor(d);
            //检测格子是否有效？
            if (neighbor&&HexMetrics.FindDistanceBetweenCells(selectedUnit.Location,neighbor)<=maxRange)
            {
                if(!rangeCells.Contains(neighbor))
                {
                    rangeCells.Add(neighbor);
                }
            }
            else
            {
                continue;
            }
            //上色
            switch (model)
            {
                case 0:
                    neighbor.EnableHighlight(Color.cyan);
                    break;
                case 1:
                    neighbor.EnableHighlight(Color.yellow);
                    break;
                default:
                    break;
            }
            if(HexMetrics.FindDistanceBetweenCells(selectedUnit.Location, neighbor) < maxRange)
            {
                showMaxRangeIterator(neighbor, model, maxRange);
            }
        }
    }

    public void showMinRangeIterator(int minRange)
    {
        for (int i = rangeCells.Count - 1; i >=0; i--)
        {
            HexCell cel = rangeCells[i];
            if (HexMetrics.FindDistanceBetweenCells(selectedUnit.Location, cel)<minRange)
            {
                cel.DisableHighlight();
                rangeCells.Remove(cel);
            }
        }
    }

    public void resetSelect()
    {
        if(this.selectedUnit)this.selectedUnit.isSelected = false;
        this.selectedUnit = null;
        statusWindow.showUnitStatus(null);
        spellText.color = new Color(171f / 255f, 174f / 255f, 57f / 255f);
        spellText.text = "Spell";
    }
}