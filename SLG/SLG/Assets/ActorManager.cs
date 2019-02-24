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
    public ChoiceActor[] highlights;

    private ChoiceActor choice;

    public GameObject SelectedMark_pfb;

    private GameObject SelectedMark;

    private HexCell currentCell;//英文说不明白了。。。储存棋子的原始位置，用于cancel使棋子回到上一个位置

    private HexCell targetCell;

    public GameObject chessAttr;

    public HexGrid hexGrid;
    public Dictionary<string, string> path=new Dictionary<string, string>();

    bool xunluOK = false;
    bool cundangOK = false;
    bool gongjiOK = false;


    void Start()
    {
        for(int i = 0; i<highlights.Length; i++)
        {
            highlights[i].setID(i);
        }
    }

    void SetSelected(int id)
    {
        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].selected = false;
            if(SelectedMark) Destroy(SelectedMark);
        }
        highlights[id].selected = true;
        
        //generate selectedMark
        choice = highlights[id];
        currentCell = choice.hexCell;
        SelectedMark = Instantiate(SelectedMark_pfb, choice.getTransform().position + new Vector3(0, 10, 0), choice.getTransform().rotation);
        showChessAttribute();
    }

    // attack event from button
    public void Move()
    {
        if (!choice) return;
        else if ((int)choice.bs > 1)
        {
            //Debug.Log("This chess has already Moved!");
            return;
        }
        else
        {
            //Debug.Log("Please select a position!");
            List<HexCell> res;
            choice.bs = behaviorStatus.ready;
            res = listCellAlgorithm(choice.hexCell);
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
        if (!choice) return;
        if (choice.bs==behaviorStatus.rest)
        {
            Debug.Log("It has already attacked!");
        }
        else
        {
            choice.bs = behaviorStatus.attackready;
        }

    }

    // attack event from button
    public void Cancel()
    {
        if (!choice) return;
        else if ((int)choice.bs < 2)
        {
            Debug.Log("This chess didn't do any thing!");
            return;
        }
        else if (choice.bs == behaviorStatus.rest)
        {
            Debug.Log("This chess has attacked ! It cannot cancel the attack command");
            return;
        }
        else
        {
            choice.hexCell = currentCell;
            choice.reloadPosition();
            choice.bs = behaviorStatus.wakeup;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && choice.bs == behaviorStatus.ready)
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
            if(choice.bs == behaviorStatus.ready)
            {
                chessMove();
            }
            else if(choice.bs == behaviorStatus.attackready)
            {//for attack

            }
        }
        
    }

    void chessMove()
    {
        if (xunluOK)
        {
            xunluAlgorithm(choice.hexCell, targetCell);
        }
        else
        {
            moveToPositionByNormal(choice.hexCell, targetCell);
        }
        choice.bs = behaviorStatus.moved;
        choice.hexCell = targetCell;
        choice.reloadPosition();
    }

    void chessAttack()
    {

        choice.bs = behaviorStatus.rest;
        
    }
    //apply xun lu Algorithm to find the path and chess will move follow the path
    private List<string> xunluAlgorithm(HexCell a, HexCell b)
    {
        string cur = b.coordinates.X.ToString() + "###" + b.coordinates.Z.ToString();
        List<string> result = new List<string>();
        string final = a.coordinates.X.ToString() + "###" + a.coordinates.Z.ToString();
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
                path.Add(key, hash_cord);
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
                        path.Add(cur_key, u);
                        key_cell.Add(cur_key, neigbor);
                        distance_table[cur_key] = distance_table[u] + neigbor.cost;
                    }

                    if (found.Contains(cur_key) && distance_table[u] + neigbor.cost < distance_table[cur_key])
                    {
                        distance_table[cur_key] = distance_table[u] + neigbor.cost;
                        path[cur_key]=u;
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

    private void moveToPositionByNormal(HexCell a, HexCell b)
    {
        Vector3 offSet = b.Position - a.Position;
        Vector3 norm = offSet.normalized;
        while (Vector3.Distance(choice.getTransform().position, b.Position)> HexMetrics.outerRadius + 2f)
        {
            choice.getTransform().position += norm * HexMetrics.outerRadius * Time.deltaTime;
        }
    }

    public void showChessAttribute()
    {
        chessAttr.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = choice.gameObject.name;
        chessAttr.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = choice.hp.ToString();
        chessAttr.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = choice.att.ToString();
        chessAttr.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = choice.bs.ToString();
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
