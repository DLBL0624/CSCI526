using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Game Manager Main Code
/// </summary>
public class ActorManager : MonoBehaviour
{
    // actor list

    public Transform friendSet; //use for get friends Actors;

    public Transform enemySet;  //use for get enemies Actors;

    private ChoiceActor[] friends;

    private EnemyActor[] enemies;

    public ChoiceActor[] Friends
    {
        get
        {
            return friends;
        }
    }

    public EnemyActor[] Enemies
    {
        get
        {
            return enemies;
        }
    }

    private ChoiceActor choiceActor;

    private EnemyActor enemyActor;

    public GameObject SelectedMark_pfb;

    private GameObject SelectedMark;

    private HexCell currentCell;

    private HexCell targetCell;

    public GameObject chessAttr;

    public HexGrid hexGrid;

    bool xunluOK = false;
    bool cundangOK = false;

    private void Awake()
    {
        //load enemies 
        List<EnemyActor> list1 = new List<EnemyActor>();
        foreach (Transform tf in enemySet)
        {
            list1.Add(tf.GetComponent<EnemyActor>());
        }
        enemies = list1.ToArray();
        //set enemies' ID
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].setID(i);
            enemies[i].actorManager = this;
        }

        //load friends
        List<ChoiceActor> list2 = new List<ChoiceActor>();
        foreach (Transform tf in friendSet)
        {
            list2.Add(tf.GetComponent<ChoiceActor>());
        }
        friends = list2.ToArray();
        //set enemies' ID
        for (int i = 0; i < friends.Length; i++)
        {
            friends[i].setID(i);
            friends[i].actorManager = this;
        }
    }

    void Start()
    {

    }

    public void RemoveSelectedMark()
    {
        for (int i = 0; i < friends.Length; i++)
        {
            friends[i].selected = false;
            if (SelectedMark) Destroy(SelectedMark);
        }
    }

    public void generateSelectedMark()
    {
        SelectedMark = Instantiate(SelectedMark_pfb, choiceActor.getTransform().position + new Vector3(0, 10, 0), choiceActor.getTransform().rotation);
    }

    void SetSelected(int id)
    {
        RemoveSelectedMark();
        friends[id].selected = true;
        
        //generate selectedMark
        choiceActor = friends[id];
        currentCell = choiceActor.hexCell;
        generateSelectedMark();
        showChessAttribute();
    }

    void SetSelectedTarget(int id)
    {
        if(!choiceActor||choiceActor.bs != behaviorStatus.attackready)
        {
            return;
        }
        this.enemyActor = enemies[id];

        //attack
        if(hexGrid.FindDistanceBetweenCells(choiceActor.hexCell, enemyActor.hexCell)== 1) chessAttack(choiceActor, enemyActor);
        
        this.enemyActor = null;
    }

    // attack event from button
    public void Move()
    {
        if (!choiceActor) return;
        else if ((int)choiceActor.bs>1)
        {
            Debug.Log("This chess has already Moved!");
            return;
        }
        else
        {
            Debug.Log("Please select a position!");
            choiceActor.bs = behaviorStatus.ready;
        }
        
    }

    // attack event from button
    public void Attack()
    {
        if (!choiceActor) return;
        if (choiceActor.bs==behaviorStatus.rest)
        {
            Debug.Log("It has already attacked!");
        }
        else
        {
            choiceActor.bs = behaviorStatus.attackready;
        }

    }

    // attack event from button
    public void Cancel()
    {
        if (!choiceActor) return;
        else if ((int)choiceActor.bs < 2)
        {
            Debug.Log("This chess didn't do any thing!");
            return;
        }
        else if (choiceActor.bs == behaviorStatus.rest)
        {
            Debug.Log("This chess has attacked ! It cannot cancel the attack command");
            return;
        }
        else
        {
            choiceActor.hexCell = currentCell;
            choiceActor.reloadPosition();
            choiceActor.bs = behaviorStatus.wakeup;
            RemoveSelectedMark();
            generateSelectedMark();
        }
    }

    //Attack
    void OnDeal(ChoiceActor fri, EnemyActor eny)
    {
        fri.hp -= eny.att;
        eny.hp -= fri.att;
        eny.checkAlive();
        fri.checkAlive();
    }

    void OnDeal(EnemyActor eny, ChoiceActor fri)
    {
        eny.hp -= fri.att;
        fri.hp -= eny.att;
        eny.checkAlive();
        fri.checkAlive();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() &&choiceActor&&choiceActor.bs == behaviorStatus.ready)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            targetCell = hexGrid.GetCell(hit.point);
            if(choiceActor.bs == behaviorStatus.ready&&choiceActor.hexCell!=targetCell)
            {
                chessMove();
            }
            //else if(choiceActor.bs == behaviorStatus.attackready)
            //{//for attack
                
            //}
        }
    }

    void chessMove()
    {
        if (xunluOK)
        {
            xunluAlgorithm(choiceActor.hexCell, targetCell);
        }
        else
        {
            moveToPositionByNormal(choiceActor.hexCell, targetCell);
        }
        choiceActor.bs = behaviorStatus.moved;
        choiceActor.hexCell = targetCell;
        choiceActor.reloadPosition();
        //reset the Marker;
        RemoveSelectedMark();
        generateSelectedMark();
    }

    void chessAttack(ChoiceActor fri, EnemyActor eny)
    {
        OnDeal(fri, eny);
        fri.bs = behaviorStatus.rest;
    }

    void chessAttack( EnemyActor eny, ChoiceActor fri)
    {
        OnDeal(eny, fri);
        eny.bs = behaviorStatus.rest;
    }
    //apply xun lu Algorithm to find the path and chess will move follow the path
    private void xunluAlgorithm(HexCell a, HexCell b)
    {

    }

    private void moveToPositionByNormal(HexCell a, HexCell b)
    {
        Vector3 offSet = b.Position - a.Position;
        Vector3 norm = offSet.normalized;
        while (Vector3.Distance(choiceActor.getTransform().position, b.Position)> HexMetrics.outerRadius + 2f)
        {
            choiceActor.getTransform().position += norm * HexMetrics.outerRadius * Time.deltaTime;
        }
    }

    public void showChessAttribute()
    {
        chessAttr.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = choiceActor.gameObject.name;
        chessAttr.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = choiceActor.hp.ToString();
        chessAttr.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = choiceActor.att.ToString();
        chessAttr.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = choiceActor.bs.ToString();
    }

    //// goto main menu scene
    //public void LoadMenu()
    //{
    //    Application.LoadLevel("Menu");
    //}

}

public enum behaviorStatus
{
    wakeup,
    ready,
    moved,
    attackready,
    rest
}
