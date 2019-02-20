using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceActor : MonoBehaviour
{
    private int id;//chessID

    //chess attribute
    public int ap;//actionPower

    public int hp;//HealthPower

    public int att;//attackDamage

    public int def = 0;//define

    public behaviorStatus bs;

    // main game manager
    public ActorManager actorManager;

    // choice of team
    public bool selected;

    // enemy list
    Transform[] enemies;

    // current transform
    Transform tr;

    public HexCell hexCell;

    void Start()
    {
        tr = transform;
        hexCell = actorManager.hexGrid.GetCell(tr.position);
        reloadPosition();

        //get ID
        //this.id = ActorIDGenerator.getNewID();

        //original status
        this.bs = behaviorStatus.wakeup;

        //load the enemies
        //List<Transform> list = new List<Transform>();
        //foreach (Transform tf in GameObject.Find("Enemies").transform)
        //    list.Add(tf);
        //enemies = list.ToArray();
        
    }

    // choice actor event
    void OnMouseDown()
    {
        actorManager.SendMessage("SetSelected", id, SendMessageOptions.DontRequireReceiver);
    }

    // attack event from mecanim
    void OnDeal(int type)
    {
        

    }

    // update shadow position
    void Update()
    {
        
    }

    public Transform getTransform()
    {
        return tr;
    }

    public void reloadPosition()
    {
        tr.position = hexCell.Position;
    }

    public void setID(int id)
    {
        this.id = id;
    }
}
