using System;
using System.Collections.Generic;
using UnityEngine;

//Buff池，用于存放所有技能的数据
public static class BuffPool
{
    public static List<Buff> passiveSkills = new List<Buff>();

    public static bool loadComplete = false;

    //public static void LoadPassiveSkills()
    //{
    //    Debug.Log("load complete!");
    //    passiveSkills.Add(new ShelterOfLight());//1
    //    loadComplete = true;
    //}

    public static Buff getPassiveSkills(int index)
    {
        //被动池用于分发被动技能
        switch(index)
        {
            case 0://阿尔萨斯
                return new ShelterOfLight();
            case 1://克罗米
                return new FutureGuardian();
            case 2://乌瑟尔
                return new HolyHalo();
            default:
                return new NoSkills();
        }
        //Debug.Log("the unit has skill:" + index);
        //return passiveSkills[index];
    }

    public static Skill getActiveSkills(int index)
    {
        //被动池用于分发被动技能
        switch (index)
        {
            case 0://阿尔萨斯 - > 圣裁
                return new SacredRuling();
            default:
                return new SacredRuling();
        }
        //Debug.Log("the unit has skill:" + index);
        //return passiveSkills[index];
    }

}
