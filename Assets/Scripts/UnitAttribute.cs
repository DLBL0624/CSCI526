using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAttribute : MonoBehaviour
{
    private int id;//chessID

    public string actorName;//Actor Name


    public int hp = 100;//HealthPower

    public int hpMin = 0;//用于检测不死效果 -> if 1

    public int hpMax = 100;//HealthPowerMax


    public int ap = 24;//ActionPower

    public int apTemp = 0;//附加行动力

    public int Ap
    {
        get
        {
            return ap + apTemp;
        }
    }

    public int att = 20;//AttackDamage

    public int attTemp = 0;//附加攻击力

    public int Att
    {
        get
        {
            return att + attTemp;
        }
    }

    public int def = 0;//Defense

    public int defTemp = 0;//附加攻击力

    public int Def
    {
        get
        {
            return def + defTemp;
        }
    }

    public int sp = 0;//speed

    public int spTemp = 0;//附加速度

    public int Sp
    {
        get
        {
            return sp + spTemp;
        }
    }


    public behaviorStatus bs;//棋子状态

    public int team;//棋子队伍 0->己方  1->敌方  2->中立

    public List<Buff> Buffables = new List<Buff>(); //所有buff

    public int passSkillNumber;//被动技能编号

    public int actiSkillNumber;//主动技能编号

    public Buff passSkill;//被动技能

    public Skill activeSkill;//主动技能

    public int range = 1;//攻击范围 -> 默认为1，暂不可用



    public void AddBuffable(Buff ibuff)
    {
        Buffables.Add(ibuff);
        Debug.Log("After Add : RestBuff = " + Buffables.Count);
    }

    public void RemoveBuffable(Buff ibuff)
    {
        var result = from r in Buffables
                     group r by (r.GetType() == ibuff.GetType()) into g
                     where g.Count() > 0
                     select g;

        foreach(var a in result)
        {
            Debug.Log("EffectBuffs Count = " + a.Count<Buff>());
            foreach (Buff buff in a)
            {
                buff.UnApply();
                Debug.Log("RemoveBuff: " + Buffables.Remove(buff));
            }

        }
        Debug.Log("After Remove : RestBuff = " + Buffables.Count);
    }

    public bool checkBuffable(Buff ibuff)
    {
        return Buffables.Contains(ibuff);
    }

    public void Update()
    {
        //buff结算

        for (int i = Buffables.Count - 1; i >= 0; i--)
        {
            Buffables[i].Apply(this);

            if (Buffables[i].FinishTurn)
            {
                Buffables.Remove(Buffables[i]);
            }
        }
        //如果buff池已加载成功且当前角色拥有被动技能
        if (passSkill!=null)
        {
            //被动技能结算
            passSkill.Apply(this);
        }
        else if (passSkill==null)
        {
            //添加被动技能
            passSkill = BuffPool.getPassiveSkills(this.passSkillNumber);
        }
        if (activeSkill != null)
        {
            //主动技能结算
            activeSkill.Apply(this);
        }
        else if (activeSkill == null)
        {
            //添加主动技能
            activeSkill = BuffPool.getActiveSkills(this.actiSkillNumber);
        }
    }
}