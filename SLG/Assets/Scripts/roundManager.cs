using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class roundManager
{

    private static int round = 0;
    private static roundTurn turn = roundTurn.PlayerTurn;

    public static UnitManager unitManager;

    public static int getRound()
    {
        return round;
    }

    public static void setRound(int r)
    {
        round = r;
    }

    public static roundTurn getTurn()
    {
        return turn;
    }

    public static void switchTurn()
    {
        if ((int)turn == 0)
        {
            turn++;
            unitManager.aiManager.AIstart();
        }
        else
        {
            unitManager.grid.resetList();
            turn--;
            round++;
        }
    }
}

public enum roundTurn
{
    PlayerTurn,
    EnemyTurn
}
