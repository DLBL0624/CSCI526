using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttribute : MonoBehaviour
{
    private int id;//chessID

    public string actorName;//Actor Name

    public int ap = 10;//ActionPower

    public int hp = 100;//HealthPower

    public int hpMax = 100;//HealthPowerMax

    public int att = 20;//AttackDamage

    public int def = 0;//Defense

    public behaviorStatus bs;

    public int team;

}

public enum team
{
    friend,
    enemy,
    neutral
}