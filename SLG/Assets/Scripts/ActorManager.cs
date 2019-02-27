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
    public Dictionary<HexCell, HexCell> path=new Dictionary<HexCell, HexCell>();

    bool xunluOK = false;
    bool cundangOK = false;

    public AIManager aIManager;

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
            enemies[i].ID=i;
            enemies[i].actorManager = this;
        }

        //load friendsz
        List<ChoiceActor> list2 = new List<ChoiceActor>();
        foreach (Transform tf in friendSet)
        {
            list2.Add(tf.GetComponent<ChoiceActor>());
        }
        friends = list2.ToArray();
        //set enemies' ID
        for (int i = 0; i < friends.Length; i++)
        {
            friends[i].ID=i;
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
            //Debug.Log("Please select a position!");
            List<HexCell> res;
            choiceActor.bs = behaviorStatus.ready;
            res = listCellAlgorithm(choiceActor.hexCell);
            foreach (HexCell cell in res)
            {
                string cur_key = cell.coordinates.X.ToString() + "###" + cell.coordinates.Z.ToString();
                Debug.Log(cur_key);
            }
            //foreach (HexDirection dir in HexDirection.GetValues(typeof(HexDirection))) {
            //    HexCell cell = choice.hexCell.GetNeighbor(dir);
            //    Debug.Log(cell);
            //}
            //HexCell cell = choice.hexCell.GetNeighbor(NE);
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

    public void chessAttack( EnemyActor eny, ChoiceActor fri)
    {
        OnDeal(eny, fri);
        eny.bs = behaviorStatus.rest;
    }

    public void moveToTarget( EnemyActor eny, ChoiceActor fri)
    {
        EnemyMoveToPositionByNormal(eny, fri.hexCell);
        eny.bs = behaviorStatus.moved;
    }

    //apply xun lu Algorithm to find the path and chess will move follow the path
    private List<HexCell> xunluAlgorithm(HexCell a, HexCell b)
    {
        HexCell cur = a;
        List<HexCell> result = new List<HexCell>();
        HexCell final = b;
        while (cur != final) {
            result.Add(cur);
            cur = path[cur];
        }
        result.Add(cur);
        return result;
    }


    private List<HexCell> listCellAlgorithm(HexCell v)
    {
        List<HexCell> result = new List<HexCell>();
        HashSet<string> found = new HashSet<string>();
        HashSet<string> set = new HashSet<string>();
        Dictionary<string, int> distance_table = new Dictionary<string, int>();
        Dictionary<string, HexCell> key_cell = new Dictionary<string, HexCell>();
        path = new Dictionary<HexCell, HexCell>();
        int strength = 50;
        bool change = true;
        int mindis;
        string u;

        string hash_cord = v.coordinates.X.ToString() + "###" + v.coordinates.Z.ToString();
        key_cell.Add(hash_cord, v);


        foreach (HexDirection dir in HexDirection.GetValues(typeof(HexDirection)))
        {
            //get the neighbor cell
            HexCell neigbor = v.GetNeighbor(dir);
            if (neigbor == null) continue;
            //initialize the cost of one cell
            neigbor.cost = 10;
            //set the initialize cost to get to the neighbor to be 0
            string key = neigbor.coordinates.X.ToString() + "###" + neigbor.coordinates.Z.ToString();
            distance_table.Add(key, 0);
            key_cell.Add(key, neigbor);
            if (neigbor.cost + distance_table[key] <= strength)
            {
                found.Add(key);
                result.Add(neigbor);
                path.Add(neigbor, v);
                distance_table[key] += neigbor.cost;
            }   
        }

        while (change)
        {
            u = "####";
            change = false;
            mindis = strength;

            foreach (HexCell cell in result)
            {
                string cur_key = cell.coordinates.X.ToString() + "###" + cell.coordinates.Z.ToString();
                if (found.Contains(cur_key) && !set.Contains(cur_key) && cur_key != hash_cord && distance_table[cur_key] <= mindis)
                {
                    u = cur_key;
                    mindis = distance_table[cur_key];
                }
            }



            if (u == "####") break;
            set.Add(u);
            HexCell cell_u = key_cell[u];

            foreach (HexDirection dir in HexDirection.GetValues(typeof(HexDirection)))
            {
                HexCell neigbor = cell_u.GetNeighbor(dir);
                if (neigbor == null) continue;
                //initialize cost for the newly found cell
                neigbor.cost = 10;
                string cur_key = neigbor.coordinates.X.ToString() + "###" + neigbor.coordinates.Z.ToString();
                if (cur_key != hash_cord)
                {
                    if (!found.Contains(cur_key) && distance_table[u] + neigbor.cost <= strength)
                    {
                        found.Add(cur_key);
                        result.Add(neigbor);
                        path.Add(neigbor, cell_u);
                        key_cell.Add(cur_key, neigbor);
                        distance_table[cur_key] = distance_table[u] + neigbor.cost;
                    }

                    if (found.Contains(cur_key) && distance_table[u] + neigbor.cost < distance_table[cur_key])
                    {
                        distance_table[cur_key] = distance_table[u] + neigbor.cost;
                        path[neigbor]=cell_u;
                    }
                }
            }

            foreach (HexCell cell in result)
            {
                string cur_key = cell.coordinates.X.ToString() + "###" + cell.coordinates.Z.ToString();
                if (found.Contains(cur_key) && !set.Contains(cur_key))
                {
                    change = true;
                    break;
                }
            }
        }

        return result;
    }

    public List<int> FindPlayer(EnemyActor enemy) {
        List<int> res = new List<int>();
        List<HexCell> reachable = new List<HexCell>();
        reachable = listCellAlgorithm(enemy.hexCell);

        foreach(HexCell cell in reachable) { 
            foreach(ChoiceActor player in friends) { 
                if(player.hexCell.coordinates.X== cell.coordinates.X && player.hexCell.coordinates.Z == cell.coordinates.Z)
                {
                    res.Add(player.ID);
                    continue;
                }
            }
        }

        return res;
    }



    public List<HexCell> EnemyToPlayer(int e_id, int p_id) {
        HexCell cur = Enemies[e_id].hexCell;
        HexCell final = Friends[p_id].hexCell;
        List<HexCell> result = new List<HexCell>();
        while (cur != final)
        {
            result.Add(cur);
            cur = path[cur];
        }
        return result;
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

    private void EnemyMoveToPositionByNormal(EnemyActor eny, HexCell b)
    {
        Vector3 offSet = b.Position - eny.hexCell.Position;
        Vector3 norm = offSet.normalized;
        while (Vector3.Distance(eny.getTransform().position, b.Position) > HexMetrics.outerRadius + 2f)
        {
            eny.getTransform().position += norm * HexMetrics.outerRadius * Time.deltaTime;
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

}