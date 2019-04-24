using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeController : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] characterAttribute;
    public Image attributeImage;
    public Button attributeQuit;
    public GameObject actionpanel;
    void Start()
    {
        attributeImage = this.GetComponent<Image>();
        attributeQuit.onClick.AddListener(QuitAtt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadattImage(int num)
    {
        if(num >= 0 && num < characterAttribute.Length)
        {
            this.gameObject.SetActive(true);
            actionpanel.SetActive(false);
            attributeImage.sprite = characterAttribute[num];
        }
    }
    public void QuitAtt()
    {
        this.gameObject.SetActive(false);
        actionpanel.SetActive(true);
    }
}
