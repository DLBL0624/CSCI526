using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterOfLight : Buff
{
    //圣光之庇护效果：每4/3/2回合回复10点hp
    //回复生命值
    public int recoverHP = 10;
    public int startTurn = 0;
    private int turnVal = 3;
    private int recentTurn = 0;
    private string description = "Shelter Of Light: Recover 10 Hp for every 4/3/2 turns";
    private string buffName = "ShelterOfLight";
    
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

    //激活时的效果
    public void Apply(Component charUnit)
    {
        //获取当前round值
        this.recentTurn = roundManager.getRound();
        //Debug.Log("StartTurn: " + startTurn + ",RecentTurn: " + recentTurn);
        if (charUnit as UnitAttribute != null)
        {
            if(recentTurn - startTurn == turnVal)
            {
                Debug.Log(((UnitAttribute)charUnit).actorName + " recover! his hp is from " + ((UnitAttribute)charUnit).hp + " to " + (((UnitAttribute)charUnit).hp + recoverHP));
                //更新开始回合
                startTurn = recentTurn;
                //如果当前回合-开始回合==间隔回合
                if(turnVal > 2)//缩短间隔时间
                {
                    turnVal--;
                }
                
                //回复hp
                ((UnitAttribute)charUnit).hp = 
                    ((UnitAttribute)charUnit).hp + recoverHP > ((UnitAttribute)charUnit).hpMax ? 
                    ((UnitAttribute)charUnit).hpMax : 
                    ((UnitAttribute)charUnit).hp + recoverHP;
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
