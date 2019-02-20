using UnityEngine;
using System.Collections;

/// <summary>
/// Hero display item & event
/// </summary>
public class HeroItem : MonoBehaviour {
    string heroName;

	void Start () {
        heroName = GetComponentInChildren<Animator>().name;
	}

    void OnClick()
    {
        Debug.Log("HERO: " + heroName);
    }
	
	void Update () {
	
	}
}
