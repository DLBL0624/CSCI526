using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromieDanBuf : Buff
{
    private int buffEffect = 4;
    private bool buffAdded = false;
    private string description = "看到克罗米跳舞，你兴奋了许多";
    private int startTurn = 0;
    private int recentTurn;
    private UnitAttribute unit;

    private string buffName = "Chromie Dance Buff";

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
                //自己临时攻击力加4
                this.unit = (UnitAttribute)charUnit;
                unit.attTemp += buffEffect;
            }

        }
        recentTurn = roundManager.getRound();
    }

    public void UnApply()
    {
        unit.attTemp -= buffEffect;
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
