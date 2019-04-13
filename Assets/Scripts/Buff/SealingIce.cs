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

    public bool NeedBuff = false;

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
        HexUnit TempClosestEny;
        if (charUnit as UnitAttribute != null)
        {
            TempClosestEny = roundManager.unitManager.getClosestEnemy(charUnit.gameObject.GetComponent<HexUnit>());
            //假如最近目标更换 -> 把原来目标的buff消掉，然后给新的目标上buff
            if(TempClosestEny != ClosestEnemy && TempClosestEny != null)
            {
                NeedBuff = true;
            }
            else
            {
                NeedBuff = false;
            }
            if (NeedBuff)
            {
                if (ClosestEnemy != null)
                {
                    ClosestEnemy.UnitAttribute.RemoveBuffable(new JainaDefBuf(((UnitAttribute)charUnit).gameObject));
                }
                ClosestEnemy = TempClosestEny;
                ClosestEnemy.UnitAttribute.AddBuffable(new JainaDefBuf(((UnitAttribute)charUnit).gameObject));
                NeedBuff = false;
            }
            //Debug.Log("closest enemy is" + ClosestEnemy.UnitAttribute.actorName);
        }
        //只 上一次buff，所以 你需要一个bool 值去表示 是否需要重新上buff
        //意义在于 只上一次buff，然后不能同时减了又加
        //ClosestEnemy.UnitAttribute.RemoveBuffable(new JainaDefBuf(((UnitAttribute)charUnit).gameObject) { BuffName = "JainaDefBuf" });
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
