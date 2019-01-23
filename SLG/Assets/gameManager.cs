using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    //switch
    public bool isSelected = false;
    public bool isInstantiate = false;
    public bool isPosChanged = false;


    public GameObject SelectedTarget_pfb;
    private GameObject SelectedTarget;

    public GameObject recentChess;
    public GameObject move_Button;
    public GameObject cancel_Button;
    public GameObject attack_Button;

    public Transform chessTransform;
    public Vector3 chessPosition;
    public Vector3 chessRotation;



    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (isSelected)
        {
            if (!isInstantiate)
            {
                SelectedTarget = Instantiate(SelectedTarget_pfb, recentChess.transform.position + new Vector3(0, 1, 0), recentChess.transform.rotation);
                isInstantiate = true;
            }
        }
        if(SelectedTarget&&isPosChanged)
        {
            SelectedTarget.transform.position = recentChess.transform.position + new Vector3(0, 1, 0);
            isPosChanged = false;
        }

    }

    public void Battle(GameObject chess_a, GameObject chess_b)
    {
        Chess chessA = chess_a.GetComponent<Chess>();
        Chess chessB = chess_b.GetComponent<Chess>();

        chessB.HP -= chessA.Att;
    }
}
