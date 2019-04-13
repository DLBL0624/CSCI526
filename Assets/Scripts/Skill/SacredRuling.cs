using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacredRuling : Skill
{
    private string skillName = "Sacred Ruling";
    private string description = "（由自己发动攻击时，于战斗前，对特定范围->大范围内的敌人给予（自己的攻击－敌人的防守）的伤害）";
    private int startTurn = 0;
    private int recentTurn = 0;
    private int coolDown = 4;
    private bool spellable = true;
    private int targetTeam = 1;//目标 -> 敌人
    private behaviorStatus targetBehaviour = behaviorStatus.rest;//目标状态
    private UnitAttribute unit;
    private UnitAttribute target;
    private bool targetItself = false;
    private bool needBehaviour;

    public void Apply(Component charUnit)
    {
        if (charUnit as UnitAttribute != null)
        {
            
            recentTurn = roundManager.getRound();
            if(recentTurn - startTurn >= coolDown)
            {
                spellable = true;
            }
            this.unit = (UnitAttribute)charUnit;
        }
    }
    public void UnApply()
    {

    }

    public void Spell(Component targetUnit)
    {
        if(spellable)
        {
            startTurn = roundManager.getRound();


            if (targetUnit as HexUnit != null)
            {
                //大范围受伤
                this.target = ((HexUnit)targetUnit).UnitAttribute;
                target.hp -= unit.Att - target.Def;
                HexCell targetCell = ((HexUnit)targetUnit).Location;
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexUnit targetNeightbor = targetCell.GetNeighbor(d).Unit;
                    if (targetNeightbor && targetNeightbor.UnitAttribute.team == targetTeam)
                    {
                        this.target = targetNeightbor.UnitAttribute;
                        target.hp -= unit.Att - target.Def;
                        target.hp = target.hp <= 0 ? 1 : target.hp;
                    }

                }
            }

            spellable = false;
        }
        
    }

    //Skill Description
    public string SkillName {
        get
        {
            return this.skillName;
        }
        set
        {

        }
    }
    public string Description {
        get
        {
            return this.description;
        }
        set
        {

        }
    }

    //CoolDown Check
    public int StartTurn
    {
        get
        {
            return this.startTurn;
        }
        set
        {
        }
    }
    public int RecentTurn {
        get {
            return this.recentTurn;
        }
        set {
            this.recentTurn = value;
        }
    }
    public int CoolDown {
        get
        {
            return this.coolDown;
        }
        set
        {
            this.coolDown = value;
        }
    }

    public bool Spellable
    {
        get
        {
            return this.spellable;
        }
        set
        {
            
        }
    }

    //Valid Condition
    public int TargetTeam {
        get
        {
            return this.targetTeam;
        }
        set
        {

        }
    }
    public behaviorStatus TargetBehaviour {
        get
        {
            return this.targetBehaviour;
        }
        set
        {
            
        }
    }
    public bool TargetItself
    {
        get
        {
            return this.targetItself;
        }
        set
        {

        }
    }

    public bool NeedBehavior
    {
        get
        {
            return this.needBehaviour;
        }
        set
        {

        }
    }

}
