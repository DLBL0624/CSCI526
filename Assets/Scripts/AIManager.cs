using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    //roundManager

    public UnitManager unitManager;
    public HexGameUI hexGameUI;
    public HexGrid hexGrid;
    public Text round;
    public Button roundEndButton;

    private bool isAIAction = false;

    public void AIwakeup()
    {
        StartCoroutine(AIstart());
    }

    public IEnumerator AIstart()
    {
        isAIAction = false;
        List<HexUnit> enemy_temp = unitManager.enemyUnits;
        //Debug.Log("Number of Enemies = " + unitManager.enemyUnits.Count);
        for (int i = 0; i< enemy_temp.Count;i++ )
        {
            HexUnit aiChess = enemy_temp[i];
            if (!aiChess) continue;
            //List<int> ids = unitManager.FindPlayer(aiChess);
            //List<HexUnit> DetectChess = unitManager.friendUnits;

            List<HexUnit> DetectChess = AIFindTouchableChess(aiChess);
            //Debug.Log("DetectChess = " +DetectChess.Count);
            yield return AI(aiChess, DetectChess);
            while(isAIAction)
            {
                yield return null;
            }
            DetectChess.Clear();
        }
        
        roundManager.switchTurn();
        yield return new WaitForSeconds(1f);
        round.text = roundManager.getRound().ToString();
        hexGameUI.transform.GetChild(0).gameObject.SetActive(true);
        roundEndButton.gameObject.SetActive(true);
    }

    IEnumerator AI(HexUnit aiChess, List<HexUnit> DetectChess)
    {
        isAIAction = true;
        HexUnit target = null;
        int maxScore = int.MinValue;
        for(int i = 0; i< DetectChess.Count; i++)
        {
            HexUnit userChess = DetectChess[i];
            //计算分数加权的方式，暂不考虑sp过三的事
            int score = Mathf.Min(aiChess.UnitAttribute.Att - userChess.UnitAttribute.Def, (int)userChess.UnitAttribute.hp) - Mathf.Min(userChess.UnitAttribute.Att - aiChess.UnitAttribute.Def, (int)aiChess.UnitAttribute.hp);
            //触发连击的情况
            if (score > maxScore)
            {
                maxScore = score;
                target = userChess;
            }
        }
        
        if (maxScore != int.MinValue)
        {
            Debug.Log("AI Operating!");
            yield return hexGameUI.AIDoSelection(aiChess, target, aiChess.UnitAttribute.Ap);
            
        }
        isAIAction = false;
    }

    List<HexUnit> AIFindTouchableChess(HexUnit aiChess)
    {
        List<HexUnit> friend_temp = unitManager.friendUnits;

        List<HexUnit> cellList = new List<HexUnit>();
        Debug.Log("Number of Friends = " + unitManager.friendUnits.Count);
        for(int i = 0; i<friend_temp.Count; i++)
        { 
            HexUnit friendChess = friend_temp[i];
            if (!friendChess) continue;
            HexCell targetChess = friendChess.Location;
            if(hexGameUI.checkNeighbor(friendChess.Location, aiChess.Location))
            {
                cellList.Add(friendChess);
            }
            else
            {
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = friendChess.Location.GetNeighbor(d);
                    HexEdgeType edgeType = friendChess.Location.GetEdgeType(neighbor);
                    Debug.Log(edgeType);
                    if (neighbor == null)
                    {
                        continue;
                    }
                    if (neighbor.IsUnderwater || (neighbor.Unit && neighbor.Unit != aiChess))
                    {
                        continue;
                    }
                    if (edgeType == HexEdgeType.Cliff)
                    {
                        continue;
                    }
                    if (friendChess.Location.Walled != neighbor.Walled)
                    {
                        continue;
                    }
                    targetChess = neighbor;//找能到的cell
                }
                hexGrid.FindPath(aiChess.Location, targetChess, aiChess.UnitAttribute.Ap);
                Debug.Log(hexGrid.HasPath);
                if (hexGrid.HasPath)
                {
                    cellList.Add(friendChess);
                }
            }
        }
        return cellList;
    }

}