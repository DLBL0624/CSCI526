using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JainaDefBuf : Buff
{
    private int buffEffect = 4;
    private bool buffAdded = false;
    private string description = "被吉安娜弱化的小兵";
    public GameObject jaina;
    public GameObject unit;
    public JainaDefBuf(GameObject jainaFrom)
    {
        this.jaina = jainaFrom;
    }

    private string buffName = "JainaDefBuf";

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
        if (!buffAdded)
        {
            buffAdded = true;
            //保证输入进来的是一个UnitAttribute
            if (charUnit as UnitAttribute != null)
            {
                this.unit = ((UnitAttribute)charUnit).gameObject;
                //自己临时防御力加4
                ((UnitAttribute)charUnit).defTemp -= buffEffect;
                ((UnitAttribute)charUnit).apTemp -= buffEffect;
                ((UnitAttribute)charUnit).attTemp -= buffEffect;
                ((UnitAttribute)charUnit).spTemp -= buffEffect;

            }

        }
    }

    public void UnApply()
    {
        if (unit)
        {
            unit.GetComponent<UnitAttribute>().defTemp += buffEffect;
            unit.GetComponent<UnitAttribute>().apTemp += buffEffect;
            unit.GetComponent<UnitAttribute>().attTemp += buffEffect;
            unit.GetComponent<UnitAttribute>().spTemp += buffEffect;
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
            UnApply();
            return true;
        }
        set
        {
            if (value == true)
            {
                UnApply();
            }
        }
    }
    public int RecentTurn { get; set; }
}
