using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class roundManager
{



    private static int round = 0;
    private static roundTurn turn = roundTurn.PlayerTurn;

    public static ActorManager actorManager;

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
            actorManager.aIManager.AIstart();
        }
        else
        {
            actorManager.resetBehaviorStatus();
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
