using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy Actor code
/// </summary>
public class EnemyActor : MonoBehaviour {
    // damage effect prefab
    public GameObject damageEffectPrefab;

    // current animator
    Animator animator;

    // current trasnform
    Transform tr;

    // bone and shadow
    public Transform bone1, bone2, shadow;

	void Start () {
        animator = GetComponent<Animator>();
        bone1 = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        bone2 = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        shadow = transform.Find("Shadow");
        tr = transform;
    }

    void OnDeal(int type)
    {
        //Debug.Log("OnDeal : " + type);
    }

	void Update () {
        // update shadow position
        if (animator && bone1 && bone2)
        {
            Vector3 pos = (bone1.position + bone2.position) / 2f;
            pos.y = tr.position.y + 0.01f;
            if (shadow)
                shadow.transform.position = pos;
        }
    }

    // display damage effect
    void OnDamage()
    {
        animator.CrossFade("Damage", 0.2f);
        if (damageEffectPrefab) Instantiate(damageEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
    }

    // damage event from bullet
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.tag == "Bullet")
            {
                OnDamage();
            }
        }
    }
}
