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
    }

    public void HitImage()
    {
        //attControl.SetActive(true);
        ac.LoadattImage(typego);
    }
}
