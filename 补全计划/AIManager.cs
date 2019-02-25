using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIManager : MonoBehaviour
{
    //roundManager



    public ActorManager actorManager;


    public void AIstart()
    {
        //Debug.Log("....");
        foreach (EnemyActor aiChess in actorManager.Enemies)
        {
            //Debug.Log("....");
            List<int> ids = actorManager.FindPlayer(aiChess);
            AI(aiChess, ids);
        }
        roundManager.switchTurn();
    }

    void AI(EnemyActor aiChess, List<int> ids)
    {
        ChoiceActor target = null;
        int maxScore = int.MinValue;

        //if (ids.Count == 0) {
        //    Debug.Log("#####");
        //}

        foreach (int id in ids)
        {
            ChoiceActor userChess = actorManager.Friends[id];
            int score = Mathf.Min(aiChess.att, userChess.hp) - Mathf.Min(userChess.att, aiChess.hp);
            if (score > maxScore)
            {
                maxScore = score;
                target = userChess;
            }
            Debug.Log(id);
        }
        if (maxScore != int.MinValue)
        {
            actorManager.moveToTarget(aiChess, target);
            actorManager.chessAttack(aiChess, target);
        }
    }
}