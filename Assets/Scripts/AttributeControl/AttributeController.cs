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
    public GameObject QUIT;

    public UnitAttribute getattribute;

    public Text hpnum;
    public Text apnum;
    public Text attnum;
    public Text defnum;
    public Text spdnum;
    public Text maxhpnum;

    void Start()
    {
        attributeImage = this.GetComponent<Image>();
        attributeQuit.onClick.AddListener(QuitAtt);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void LoadattImage(int num, HexUnit Unit)
    {
        if(num >= 0 && num < characterAttribute.Length)
        {
            this.gameObject.SetActive(true);
            actionpanel.SetActive(false);
            QUIT.SetActive(false);
            attributeImage.sprite = characterAttribute[num];
            getattribute = Unit.UnitAttribute;
            //Debug.Log("hp is "+ getattribute.hp);
            hpnum.text = getattribute.hp + "";
            maxhpnum.text = getattribute.hpMax + "";
            apnum.text = getattribute.ap + "";
            attnum.text = getattribute.Att + "";
            defnum.text = getattribute.Def + "";
            spdnum.text = getattribute.Sp + "";

        }
    }
    public void QuitAtt()
    {
        this.gameObject.SetActive(false);
        actionpanel.SetActive(true);
        QUIT.SetActive(true);
    }
}
