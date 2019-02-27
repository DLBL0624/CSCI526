using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<HexUnit> friendUnits = new List<HexUnit>();
    public List<HexUnit> enemyUnits = new List<HexUnit>();
    public List<HexUnit> neutralUnits = new List<HexUnit>();

    

    public AIManager aiManager;

    public HexGrid grid;

    public void Start()
    {
        roundManager.unitManager = this;
    }

    public void loadUnits(List<HexUnit> units)
    {
        for(int i = 0; i<units.Count; i++)
        {
            loadUnit(units[i]);
        }
    }

    public void loadUnit(HexUnit unit)
    {
        if (unit.UnitAttribute.team == 0)
        {
            friendUnits.Add(unit);
        }
        if (unit.UnitAttribute.team == 1)
        {
            enemyUnits.Add(unit);
        }
        if (unit.UnitAttribute.team == 2)
        {
            neutralUnits.Add(unit);
        }
    }

    public void removeUnit(HexUnit unit)
    {
        if (unit.UnitAttribute.team == 0)
        {
            friendUnits.Remove(unit);
        }
        if (unit.UnitAttribute.team == 1)
        {
            enemyUnits.Remove(unit);
        }
        if (unit.UnitAttribute.team == 2)
        {
            neutralUnits.Remove(unit);
        }
    }

    public void clearUnits()
    {
        friendUnits.Clear();
        enemyUnits.Clear();
        neutralUnits.Clear();
    }
}
