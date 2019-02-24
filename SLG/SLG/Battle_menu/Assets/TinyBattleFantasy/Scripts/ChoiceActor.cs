using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChoiceActor : MonoBehaviour {
    // main game manager
    public GameManager gameManager;

    // bullet object prefab
    public GameObject[] bulletPrefab;

    // choice of team
    public bool selected;

    // enemy list
    Transform[] enemies;

    // current animator
    Animator animator;

    // current transform
    Transform tr;

    // bone and shadow
    public Transform bone1, bone2, shadow;

	void Start () {
        animator = GetComponent<Animator>();
        bone1 = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        bone2 = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        shadow = transform.Find("Shadow");
        List<Transform> list = new List<Transform>();
        foreach (Transform tf in GameObject.Find("Enemies").transform)
            list.Add(tf);
        enemies = list.ToArray();
        tr = transform;
	}

    // choice actor event
    void OnMouseDown()
    {
        gameManager.SendMessage("SetActor", animator, SendMessageOptions.DontRequireReceiver);
    }

    // attack event from mecanim
    void OnDeal(int type)
    {
        GameObject bPrefab = bulletPrefab[type % 10 - 1];
        Vector3 pos = tr.position + Vector3.right * 1.2f + Vector3.up * 0.5f;
        if (type % 10 == 3)
        {
            Instantiate(bPrefab, tr.position + Vector3.right * 1.2f + Vector3.up * 0.5f, tr.rotation);
            foreach (Transform t in enemies)
                t.SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);

        }
        Instantiate(bPrefab, tr.position + Vector3.right * 1.2f + Vector3.up * 0.5f, tr.rotation);

    }
	
    // update shadow position
	void Update () {
        if (animator && bone1 && bone2)
        {
            Vector3 pos = (bone1.position + bone2.position) / 2f;
            pos.y = tr.position.y;
            if (shadow)
                shadow.transform.position = pos;
        }
    }
}
