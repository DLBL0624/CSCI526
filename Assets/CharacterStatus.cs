using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatus : MonoBehaviour
{
    public Sprite[] characterImage;

    //public GameObject attControl;
    public int typego;
    public AttributeController ac;
    public HexUnit tranUnit;

    public Text bloodvisible;
    public Text Dpap;
    public Text Dpatt;
    public Text Dpdef;
    public Text Dpspd;

    public void showUnitStatus(HexUnit selectedUnit)
    {
        Slider hpStatus = transform.GetChild(0).GetComponent<Slider>();
        Text chessName = transform.GetChild(2).GetComponent<Text>();
        Image avatar = transform.GetChild(3).GetChild(0).GetComponent<Image>();

        Button avaterBtn = transform.GetChild(3).GetChild(0).GetComponent<Button>();
        if (!selectedUnit)
        {
            this.gameObject.SetActive(false);
            hpStatus.maxValue = 0;
            hpStatus.value = 0;
            chessName.text = "";
            avatar.sprite = null;
            return;
        }
        this.gameObject.SetActive(true);
        hpStatus.maxValue = selectedUnit.UnitAttribute.hpMax;
        hpStatus.value = selectedUnit.UnitAttribute.hp;
        chessName.text = selectedUnit.UnitAttribute.actorName;

        if (selectedUnit.unitType < characterImage.Length)
        {
            typego = selectedUnit.unitType;
            avatar.sprite = characterImage[selectedUnit.unitType];
        }
        avaterBtn.onClick.AddListener(HitImage);
        tranUnit = selectedUnit;

        bloodvisible.text = selectedUnit.UnitAttribute.hp + " / " + selectedUnit.UnitAttribute.hpMax;

        Dpap.text = selectedUnit.UnitAttribute.Ap + "";
        Dpatt.text = selectedUnit.UnitAttribute.Att + "";
        Dpdef.text = selectedUnit.UnitAttribute.Def + "";
        Dpspd.text = selectedUnit.UnitAttribute.Sp + "";
    }

    public void HitImage()
    {
        //attControl.SetActive(true);
        ac.LoadattImage(typego, tranUnit);

    }
}
