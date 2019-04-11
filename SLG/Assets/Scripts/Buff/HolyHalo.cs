using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyHalo : Buff
{
    private int buffEffect = 4;
    private bool buffAdded = false;
    private string description = "与己方单位相邻时，自己和相邻单位的攻击+4";
    private int countBuffer = 0;

    public void Apply(Component charUnit)
    {
        //保证输入进来的是一个UnitAttribute
        if (charUnit as UnitAttribute != null)
        {
            if (!buffAdded)
            {
                buffAdded = true;
                //自己临时防御力加4
                ((UnitAttribute)charUnit).defTemp += buffEffect;
            }
            
            HexUnit effectUnit = charUnit.gameObject.GetComponent<HexUnit>();
            //检查邻近格子是否有己方单位

            //检查周围格子单位数量是否发生改变
            if (checkUnitsAround(effectUnit, ((UnitAttribute)charUnit), countBuffer))
            {
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = effectUnit.Location.GetNeighbor(d);
                    if (neighbor.Unit)
                    {
                        if (neighbor.Unit.UnitAttribute.team == ((UnitAttribute)charUnit).team)
                        {
                            //己方单位获得加防御buf
                            neighbor.Unit.UnitAttribute.AddBuffable(new UthrDefBuf(((UnitAttribute)charUnit).gameObject));
                        }
                    }
                }
            }
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
    public bool FinishTurn {
        get
        {
            return false;
        }
        set
        {

        }
    }
    public int RecentTurn { get; set; }

    private bool checkUnitsAround(HexUnit effectUnit, UnitAttribute charUnit, int nearbyUnit)
    {
        int countter = 0;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = effectUnit.Location.GetNeighbor(d);
            if (neighbor.Unit)
            {
                if (neighbor.Unit.UnitAttribute.team == charUnit.team)
                {
                    //己方单位获得加防御buf
                    countter++;
                }
            }
        }
        if(countter!=nearbyUnit)
        {
            this.countBuffer = countter;
            return true;
        }
        return false;
    }
}
