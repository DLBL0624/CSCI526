using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatus : MonoBehaviour
{
    public Sprite[] characterImage;


    public void showUnitStatus(HexUnit selectedUnit)
    {
        Slider hpStatus = transform.GetChild(0).GetComponent<Slider>();
        Text chessName = transform.GetChild(2).GetComponent<Text>();
        Image avatar = transform.GetChild(3).GetChild(0).GetComponent<Image>();

        hpStatus.maxValue = selectedUnit.UnitAttribute.hpMax;
        hpStatus.value = selectedUnit.UnitAttribute.hp;
        chessName.text = selectedUnit.UnitAttribute.actorName;

        avatar.sprite = characterImage[selectedUnit.unitType];


    }
}
