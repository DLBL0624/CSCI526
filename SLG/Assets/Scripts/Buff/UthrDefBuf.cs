using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UthrDefBuf : Buff
{
    private int buffEffect = 4;
    private bool buffAdded = false;
    private string description = "你在乌瑟尔的附近，所以你变得更硬汉了";
    public GameObject uthr;
    public GameObject unit;
    public UthrDefBuf(GameObject uthrFrom)
    {
        this.uthr = uthrFrom;
    }

    public void Apply(Component charUnit)
    {
        if (!buffAdded)
        {
            buffAdded = true;
            //保证输入进来的是一个UnitAttribute
            if (charUnit as UnitAttribute != null)
            {
                this.unit = ((UnitAttribute)charUnit).gameObject;
                //自己临时防御力加4
                ((UnitAttribute)charUnit).defTemp += buffEffect;
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
    public bool FinishTurn
    {
        get
        {
            if(HexMetrics.FindDistanceBetweenCells(uthr.GetComponent<HexUnit>().Location, unit.GetComponent<HexUnit>().Location)>1)
            {
                unit.GetComponent<UnitAttribute>().defTemp -= buffEffect;
                return true;
            }
            return false;
        }
        set
        {

        }
    }
    public int RecentTurn { get; set; }
}
