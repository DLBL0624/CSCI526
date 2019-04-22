using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whichLevel : MonoBehaviour
{
    public GameObject transform;
    public string LevelName;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(transform);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetLevelname(string name)
    {
        LevelName = name;
    }
}
