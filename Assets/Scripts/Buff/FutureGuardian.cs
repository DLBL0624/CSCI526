using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FutureGuardian : Buff
{
    private int recentTurn = 0;

    private string description = "Future Guardian: 当自己的ＨＰ高于２，且遭受敌人致命攻击时，将伤害降低至自身ＨＰ会剩下１，每回合只能生效一次";

    private string buffName = "FutureGuardian";

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
    //其实啥也不用写，直接把克罗米hpMin设置成1就行了
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
