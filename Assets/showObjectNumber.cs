using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showObjectNumber : MonoBehaviour
{
    public Slider slider;
    private Text toolbarNumber;

    private void Start()
    {
        toolbarNumber = this.GetComponent<Text>();
        
    }

    // Update is called once per frame
    void Update()
    {
        toolbarNumber.text = slider.value.ToString();
    }
}
