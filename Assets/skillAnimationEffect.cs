using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillAnimationEffect : MonoBehaviour
{

    public GameObject skillPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void skillAt(HexUnit target)
    {
        Debug.Log("Spell On" + target.UnitAttribute.name);
        Instantiate(skillPrefab,target.transform);
    }
}

