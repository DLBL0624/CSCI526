using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlimmerBuff : Buff
{
    private float buffEffect = 0.8f;
    private bool buffAdded = false;
    private string description = "看到吉安娜施法，你兴奋了许多";
    private int startTurn = 0;
    private int recentTurn;
    private UnitAttribute unit;

    private string buffName = "Glimmer Buff";

    public string BuffName
    {
        set
        {

        }
        get
        {
            return this.buffName;
        }
    }

    public void Apply(Component charUnit)
    {
        if (!buffAdded)
        {
            startTurn = roundManager.getRound();
            buffAdded = true;
            //保证输入进来的是一个UnitAttribute
            if (charUnit as UnitAttribute != null)
            {
                //自己法术强度+0.8
                this.unit = (UnitAttribute)charUnit;
                //
                unit.skillDamageDepth += buffEffect;
            }

        }
        recentTurn = roundManager.getRound();
        Debug.Log("当前Turn：" + recentTurn + "起始Turn" + startTurn);
    }

    public void UnApply()
    {
        unit.skillDamageDepth -= buffEffect;
        unit = null;
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
    public bool FinishTurn
    {
        get
        {
            if (recentTurn - startTurn >= 1)
            {
                UnApply();
                return true;
            }

            return false;
        }
        set
        {
            if (value == true)
            {
                UnApply();
            }
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

        }
    }

}
