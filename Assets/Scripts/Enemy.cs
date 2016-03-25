using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float health;

	public GameObject bloodDecal;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(health <= 0) {
			Die();
		}
	}

	public void Die() {
		Debug.Log("Death!");

		Instantiate(bloodDecal, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}

	public void Hit(float damage) {
		//Debug.Log("Hit! damage: " + damage);

		health -= damage;
	}
}
