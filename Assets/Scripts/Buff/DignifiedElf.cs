using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DignifiedElf : Buff
{
    private int recentTurn = 0;

    private string description = "与不死族战斗中，战斗中所受伤害为0.8倍";

    private string buffName = "DignifiedElf";

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
