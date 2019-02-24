using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActor : MonoBehaviour
{
    private int id;//chessID

    //chess attribute
    public int ap = 10;//actionPower

    public int hp = 50;//HealthPower

    public int hpMax = 50;//HealthPowerMax

    public int att = 20;//attackDamage

    public int def = 0;//defense

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
        this.bs = behaviorStatus.wakeup;
    }

    // choice actor event
    void OnMouseDown()
    {
        actorManager.SendMessage("SetSelectedTarget", id, SendMessageOptions.DontRequireReceiver);
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
        tr.position.Set(tr.position.x, tr.position.y + tr.localScale.y * 1, tr.position.z);
    }

    public void setID(int id)
    {
        this.id = id;
    }

    public void checkAlive()
    {
        if (this.hp <= 0)
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
