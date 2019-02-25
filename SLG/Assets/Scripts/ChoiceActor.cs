using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceActor : MonoBehaviour
{
    private int id;//chessID

    public string actorName;//Actor Name

    //chess attribute
    public int ap = 10;//ActionPower

    public int hp = 100;//HealthPower

    public int hpMax = 100;//HealthPowerMax

    public int att = 20;//AttackDamage

    public int def = 0;//Defense

    public behaviorStatus bs;

    // main game manager
    public ActorManager actorManager;

    // select by user?
    public bool selected;


    // is the actor death?
    private bool isDead = false;

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

    }

    // choice actor event
    void OnMouseDown()
    {
        actorManager.SendMessage("SetSelected", id, SendMessageOptions.DontRequireReceiver);
    }

    public Transform getTransform()
    {
        return tr;
    }

    public void reloadPosition()
    {
        tr.position = hexCell.Position;
        tr.position.Set(tr.position.x, tr.position.y + tr.localScale.y * 1, tr.position.z);
    }

    public int ID
    {
        get
        {
            return this.id;
        }
        set
        {
            this.id = value;

        }
    }

    public void checkAlive()
    {
        if(this.hp<=0)
        {
            isDead = true;
            noAlive();
        }
    }

    public void noAlive()
    {
        Destroy(this.gameObject);
    }
}
