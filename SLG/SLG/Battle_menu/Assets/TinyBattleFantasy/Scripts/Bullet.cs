using UnityEngine;
using System.Collections;

/// <summary>
/// Bullet
/// </summary>
public class Bullet : MonoBehaviour {

	void Start () {
	
	}

    // check collision
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
	
    // move bullet
	void Update () {
        transform.position += Vector3.right * Time.deltaTime * 10f;
	}
}
