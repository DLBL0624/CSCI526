using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glimmer : Skill
{
    private string skillName = "Glimmer";
    private string description = "增加友放目标额外 80% 的奥义伤害";
    private int startTurn = 0;
    private int recentTurn = 0;
    private int coolDown = 4;
    private bool spellable = true;
    private int targetTeam = 0;//目标 -> 友军
    private behaviorStatus targetBehaviour = behaviorStatus.rest;//目标状态
    private UnitAttribute unit;
    private UnitAttribute target;
    private bool targetItself = false;
    private bool needBehaviour = false;
    private bool buffEffect;

    public void Apply(Component charUnit)
    {
        if (charUnit as UnitAttribute != null)
        {

            recentTurn = roundManager.getRound();
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
                //再来一次
                this.target = ((HexUnit)targetUnit).UnitAttribute;
                target.AddBuffable(new GlimmerBuff());
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
