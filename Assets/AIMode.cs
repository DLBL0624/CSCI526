using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMode : MonoBehaviour
{
    public int mode; // 0 -> none, 1 -> defense, 2-> attack
    public HexUnit hexUnit;
    public UnitAttribute unitAttribute;
    public GameObject attackTarget;
    // Start is called before the first frame update
    void Start()
    {
        hexUnit = this.GetComponent<HexUnit>();
        unitAttribute = this.GetComponent<UnitAttribute>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
