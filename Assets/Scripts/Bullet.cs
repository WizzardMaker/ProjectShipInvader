using UnityEngine;
using System.Collections;
using System;

public class Bullet : MonoBehaviour {

	Rigidbody rig;

	public GameObject explosion;

	public float startVelocity, damage;
	public float timeUntilDeath;
	float timeLeft;

	// Use this for initialization
	public void Start() {
		rig = GetComponent<Rigidbody>();

		timeLeft = timeUntilDeath + Time.time;

		rig.AddForce(transform.forward * startVelocity + PlayerController.active.GetComponent<CharacterController>().velocity.normalized, ForceMode.VelocityChange);
	}

	void Update() {
		if (Time.time > timeLeft)
			Destroy(gameObject);
	}

	// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
	public void OnCollisionEnter(Collision collision) {
		//Debug.Log("Hit! Layer: " + collision.collider.gameObject.tag);

		if(collision.gameObject.tag == "Enemy") {
			collision.gameObject.GetComponent<Enemy>().Hit(damage);
		}

		Instantiate(explosion, transform.position - transform.forward /2, Quaternion.identity);

		Destroy(gameObject);
	}
}
