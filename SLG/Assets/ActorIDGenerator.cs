using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActorIDGenerator
{
    private static int actorID = -1;

    public static int getNewID()
    {
        countID();
        return actorID;
    }

    public static int getRecentID()
    {
        return actorID;
    }

    public static void countID()
    {
        actorID++;
    }
}
