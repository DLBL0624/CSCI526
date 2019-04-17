using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burning : Buff
{
    private int buffEffect = 0;
    private bool buffAdded = false;
    private string description = "每回合对周围所有敌人造成100点伤害并且全属性-20的debuff";
    private int countBuffer = 0;
    private int startTurn = 0;
    private int recentTurn = 0;

    private string buffName = "Burning";

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
        //保证输入进来的是一个UnitAttribute
        if (charUnit as UnitAttribute != null)
        {
            HexUnit effectUnit = charUnit.gameObject.GetComponent<HexUnit>();
            if (!buffAdded)
            {
                buffAdded = true;
                //检查邻近格子是否有己方单位
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = effectUnit.Location.GetNeighbor(d);
                    if (neighbor.Unit)
                    {
                        if (neighbor.Unit.UnitAttribute.team != ((UnitAttribute)charUnit).team)
                        {
                            neighbor.Unit.UnitAttribute.DoDamage(100);
                            neighbor.Unit.UnitAttribute.AddBuffable(new MalganisBuff());
                        }
                    }
                }
            }
            this.recentTurn = roundManager.getRound();
            if (recentTurn != startTurn)
            {
                startTurn = recentTurn;
                buffAdded = false;
            }
        }

    }

    public void UnApply()
    {

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
            return false;
        }
        set
        {

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
