using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunting : Skill
{
    private string skillName = "Hunting";
    private string description = "（对目标及其背后方向所有敌人造成攻击-对方防御的伤害）";
    private int startTurn = 0;
    private int recentTurn = 0;
    private int coolDown = 2;
    private bool spellable = true;
    private int targetTeam = 1;//目标 -> 敌人
    private behaviorStatus targetBehaviour = behaviorStatus.rest;//目标状态
    private UnitAttribute unit;
    private UnitAttribute target;
    private bool targetItself = false;
    private bool needBehaviour = false;


    public void Apply(Component charUnit)
    {
        if (charUnit as UnitAttribute != null)
        {

            recentTurn = roundManager.getRound();
            Debug.Log("当前Turn：" + recentTurn + "起始Turn" + startTurn);
            if (recentTurn - startTurn >= coolDown)
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
        if (spellable)
        {
            startTurn = roundManager.getRound();


            if (targetUnit as HexUnit != null)
            {
                //大范围受伤
                this.target = ((HexUnit)targetUnit).UnitAttribute;
                target.SkillDoDamage(unit.Att, unit.skillDamageDepth);
                //背后敌人造成伤害
            }

            spellable = false;
        }

    }

    //Skill Description
    public string SkillName
    {
        get
        {
            return this.skillName;
        }
        set
        {

        }
    }
    public string Description
    {
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
    public int RecentTurn
    {
        get
        {
            return this.recentTurn;
        }
        set
        {
            this.recentTurn = value;
        }
    }
    public int CoolDown
    {
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
    public int TargetTeam
    {
        get
        {
            return this.targetTeam;
        }
        set
        {

        }
    }
    public behaviorStatus TargetBehaviour
    {
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
