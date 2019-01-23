using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : MonoBehaviour
{
    //Game Manager
    public GameObject GM;
    public gameManager cpt;


    //Chess Position and Rotation
    public Vector3 lastPosition;
    public Quaternion lastRotation;

    //Switches
    public bool isInstantiated = false;
    public bool isControllable = false;
    public bool isMoved = false;
    public bool allowMoved = false;
    public bool isAct = false;

    private bool isMovedOver = true;

    //Chess Move Speed
    public float speed = 1;

    //Ray point -> Use mouse get the position in the game  // will be alternated to touch in mobile game
    private Vector3 rayPoint;

    //Material -> Status Colour  1-> Self-Color    2-> Get Hurt
    public Material[] status;
    private int materialID = 0;

    //Chess Attribute
    public float HP = 100;
    public float Att = 105;

    //Chess Move Interval Schedule
    [Header("Time Variables")]
    public float proTime = 0.0f;
    public float NextTime = 0.0f;



    // Start is called before the first frame update
    void Start()
    {
        cpt = GM.GetComponent<gameManager>();
        lastPosition = this.transform.position;
        lastRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (cpt.recentChess != this.gameObject)
        {
            isControllable = false;
            cpt.isPosChanged = true;
        }

        //Mouse Event -> Left Click Down
        if (Input.GetMouseButtonDown(0))
        {
            //Tell the GM to instantiate a selectedMark
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();
            Physics.Raycast(ray, out hitInfo);
            if (!isControllable && hitInfo.collider.name == this.name)
            {
                cpt.isSelected = true;
                cpt.recentChess = gameObject;
                isControllable = true;
            }
            else if (isControllable)
            {
                if (!isMoved)
                {
                    Debug.Log("Press Move Button to Move");
                    if (hitInfo.collider.name == cpt.move_Button.name)
                    {
                        Debug.Log("Please Select a Position!");
                        allowMoved = true;
                        
                    }


                    if (allowMoved && hitInfo.collider.tag == "Map")
                    {
                        rayPoint = hitInfo.point;
                        rayPoint.y += 1f;
                        rayPoint.x = (float)((int)(rayPoint.x / 1));
                        rayPoint.z = (float)((int)(rayPoint.z / 1));
                        isMovedOver = false;
                        isMoved = true;
                        allowMoved = false;
                    }
                }

                if (!isAct)
                {

                }

                //
                if (hitInfo.collider.name == cpt.cancel_Button.name)
                {
                    Debug.Log("Cancel Button!");
                    this.transform.position = lastPosition;
                    this.transform.rotation = lastRotation;
                    isMoved = false;
                    rayPoint = this.transform.position;
                    isMovedOver = true;
                    cpt.isPosChanged = true;
                }
            }
        }
        if (!isMovedOver)
        {
            moveToPositionByNormal(rayPoint);
            
        }


        //


    }
    //Make the Chess Slowly Move to the Position (not follow any road)
    private void moveToPositionByNormal(Vector3 targetPosition)
    {
        Vector3 offSet = targetPosition - this.transform.position;
        Vector3 norm = offSet.normalized;
        norm.y = 0;
        this.transform.position += norm * speed * Time.deltaTime;
        if (Vector3.Distance(targetPosition, this.transform.position) < 0.1f)
        {
            isMovedOver = true;
            this.transform.position = targetPosition;
            cpt.isPosChanged = true;
        }

    }

    private void moveToPositionByDirection(Vector3 targetPosition)
    {

        Vector3 offSet = targetPosition - this.transform.position;
        float offSet_x = offSet.x;
        float offSet_z = offSet.z;

        proTime = Time.fixedTime;
        if (Mathf.Abs(offSet_x) >= Mathf.Abs(offSet_z))
        {
            if (proTime - NextTime > 1)
            {
                print("FixedTime Here" + (proTime - NextTime));
                this.transform.position += new Vector3((offSet_x / Mathf.Abs(offSet_x)), 0, 0);
                NextTime = proTime;

            }
        }
        else
        {
            if (proTime - NextTime > 1)
            {
                print("FixedTime Here" + (proTime - NextTime));
                this.transform.position += new Vector3(0, 0, (offSet_z / Mathf.Abs(offSet_z)));
                NextTime = proTime;

            }
        }
        
        if (Vector3.Distance(targetPosition, this.transform.position) == 0)
        {
            isMovedOver = true;
            this.transform.position = targetPosition;
            cpt.isPosChanged = true;
        }

    }
    private void getHurt()
    {
        Invoke("changeColor", 1);
        Invoke("changeColor", 1);
        Invoke("changeColor", 1);
        Invoke("changeColor", 1);
    }

    private void changeColor()
    {
        this.transform.GetComponent<MeshRenderer>().material = status[materialID++ / 2];
    }

}
