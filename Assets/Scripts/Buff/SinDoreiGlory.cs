using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinDoreiGlory : Buff
{
    private int recentTurn = 0;

    private string description = "敌方速度低于自己，伤害增加速度差";

    private string buffName = "SinDoreiGlory";

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
            if(((UnitAttribute)charUnit).bs == behaviorStatus.attackready)
            {

            }
        }
    }

    public void UnApply()
    {

    }

    public bool FinishTurn
    //用于判断是否buff效果结束
    //因为是被动技能效果，所以始终不会结束（始终return false）
    {
        set
        {

        }
        get
        {
            return false;
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
