using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SealingIce : Buff
{
    private int recentTurn = 0;
    private int buffEffect = 4;
    private string description = "回合开始时，全地图范围内离自己最近的敌人全数值-4（直到敌人下次行动结束)";
    public HexUnit ClosestEnemy;

    private string buffName = "SealingIce";

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

    public string Description
    {
        set
        {

        }
        get
        {
            return this.description;
        }
    }

    public void Apply(Component charUnit)
    {
        if(charUnit as UnitAttribute != null)
        {
            ClosestEnemy = roundManager.unitManager.getClosestEnemy(charUnit.gameObject.GetComponent<HexUnit>());
        }
        if (ClosestEnemy != null)
        {
            ClosestEnemy.UnitAttribute.RemoveBuffable(new JainaDefBuf(((UnitAttribute)charUnit).gameObject) { BuffName = "JainaDefBuf" });
            ClosestEnemy.UnitAttribute.AddBuffable(new JainaDefBuf(((UnitAttribute)charUnit).gameObject));
        }
    }

    public void UnApply()
    {
        ClosestEnemy = null;
    }

    public bool FinishTurn
    //用于判断是否buff效果结束
    //回合结束后，被动消失
    {
        set
        {
            if(value == true)
            {
                UnApply();
            }
        }
        get
        {
            UnApply();
            return true;
        }
    }

    public int RecentTurn
    {
        //用于获取游戏当前回合数
        set
        {
            this.recentTurn = value;
        }
        get
        {
            return this.recentTurn;
        }
    }
}
