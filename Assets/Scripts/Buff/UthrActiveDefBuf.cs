using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UthrActiveDefBuf : Buff
{
    private int buffEffect = 6;
    private bool buffAdded = false;
    private string description = "乌瑟尔给你吟了个诗，所以你变得更硬汉了";
    private int startTurn = 0;
    private int recentTurn;
    public GameObject unit;

    private string buffName = "UthrDefBuf";

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
                this.unit = ((UnitAttribute)charUnit).gameObject;
                //自己临时防御力加6
                ((UnitAttribute)charUnit).defTemp += buffEffect;
            }

        }
        recentTurn = roundManager.getRound();
        Debug.Log("当前Turn：" + recentTurn + "起始Turn" + startTurn);
    }

    public void UnApply()
    {
        if (unit)
        {
            unit.GetComponent<UnitAttribute>().defTemp -= buffEffect;
            unit = null;
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
    public int RecentTurn { get; set; }
}
