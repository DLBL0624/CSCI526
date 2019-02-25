﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
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
        roundManager.actorManager = this;
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
        chessAttr.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = choiceActor.actorName;
        chessAttr.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = choiceActor.hp.ToString();
        chessAttr.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = choiceActor.att.ToString();
        chessAttr.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = choiceActor.bs.ToString();
    }

    public void resetBehaviorStatus()
    {
        foreach(ChoiceActor ca in friends)
        {
            ca.bs = behaviorStatus.wakeup;
        }
        foreach (EnemyActor en in enemies)
        {
            en.bs = behaviorStatus.wakeup;
        }
    }

    public void CreateSaveGameObject()
    {
        Save save = new Save();
        int En = 0;
        int Fn = 0;
        foreach(ChoiceActor ca in friends)
        {
            save.TargetPositions.Add(ca.getTransform().position);
            save.TargetTypes.Add(0);
            save.TargetAtt.Add(ca.att);
            save.TargetDef.Add(ca.def);
            save.TargetHP.Add(ca.hp);
            save.TargetHPMAX.Add(ca.hpMax);
            Fn++;
        }

        foreach (EnemyActor ca in enemies)
        {
            save.TargetPositions.Add(ca.position);
            save.TargetTypes.Add(1);
            save.TargetAtt.Add(ca.att);
            save.TargetDef.Add(ca.def);
            save.TargetHP.Add(ca.hp);
            save.TargetHPMAX.Add(ca.hpMax);
            En++;
        }
    }

    public void SaveGame()
    {
        //创建了对象并保存所有数据
        Save save = CreateSaveGameObject();//创建了对象并保存所有数据

        //二进制序列化数据并创建对象文档
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        //显示保存成功
        Debug.Log("Game Saved");
    }

}

public enum behaviorStatus
{
    wakeup,
    ready,
    moved,
    attackready,
    rest
}

public class Save
{
    public List<Vector3> TargetPositions = new List<Vector3>;
    public List<int> TargetTypes = new List<int>();
    public List<int> TargetAtt = new List<int>();
    public List<int> TargetDef = new List<int>();
    public List<int> TargetHP = new List<int>();
    public List<int> TargetHPMAX = new List<int>();
}

