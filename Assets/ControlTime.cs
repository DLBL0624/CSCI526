using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTime : MonoBehaviour
{
    public float startTime=0;
    public float recentTime = 0;
    public float timeInterval = 3;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.fixedTime;
        //timeInterval = 0;
    }

    // Update is called once per frame
    void Update()
    {
        recentTime = Time.fixedTime;
        if (timeInterval < recentTime - startTime) {
            Destroy(this.gameObject);
        }
    }
}
