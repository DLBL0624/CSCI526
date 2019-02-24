using UnityEngine;
using System.Collections;

/// <summary>
/// Game Manager Main Code
/// </summary>
public class GameManager : MonoBehaviour {
    // our team animator list
    public Animator[] team;

    // actor list
    public ChoiceActor[] highlights;

    // hud actor list
    public Transform[] hudTeam;

    // choice actor
    Animator choice;

    // hud position
    Vector3 hudStartPos;

	void Start () {
        hudStartPos = hudTeam[0].position;
        SetHud(0);
	}

    // set current hud display
    void SetHud(int who)
    {
        choice = team[who];
        for (int i=0; i<hudTeam.Length; i++) {
            Transform tf = hudTeam[i];
            if (i == who)
            {
                tf.transform.position = hudStartPos;
                highlights[i].selected = true;
            }
            else
            {
                tf.transform.position = hudStartPos + Vector3.up * 10f;
                highlights[i].selected = false;
            }
        }
    }

    // set current actor
    void SetActor(Animator ani)
    {
        int i;
        for (i=0; i<team.Length; i++) {
            Animator t = team[i];
            if (t==ani) break;
        }
        if (i >= team.Length) return;
        SetHud(i);
    }

    // attack event from button
    public void Attack()
    {
        if (!choice) return;
        choice.CrossFade("Attack", 0.2f);
    }

    // attack event from button
    public void Attack2()
    {
        if (!choice) return;
        choice.CrossFade("Attack2", 0.2f);
    }

    // attack event from button
    public void Attack3()
    {
        if (!choice) return;
        choice.CrossFade("Attack3", 0.2f);
    }

	void Update () {
    }

    // goto main menu scene
    public void LoadMenu(){
        Application.LoadLevel("Menu");
    }
}
