using UnityEngine;
using System.Collections;
using Utils.Modifier;

public class PlayerController : MonoBehaviour {

	public float speed,airSpeed, jumpHeight;
	CharacterController cc;
	public Vector3 velocity = Vector3.zero, gravity = Vector3.zero;
	CollisionFlags oFlag;
	float testFloat;


	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
	}

	void Move() {
		velocity = Vector3.zero;

		if (cc.isGrounded) {
			gravity = Physics.gravity * Time.deltaTime;
			velocity += transform.forward * Input.GetAxisRaw("Vertical") * speed;
			velocity += transform.right * Input.GetAxisRaw("Horizontal") * speed;

			if (Input.GetButtonDown("Jump")) {
				gravity = transform.up * jumpHeight;
			}
		} else {

			velocity += transform.forward * Input.GetAxisRaw("Vertical") * airSpeed;
			velocity += transform.right * Input.GetAxisRaw("Horizontal") * airSpeed;
		}
		gravity += Physics.gravity * Time.deltaTime;
		//velocity.Normalize();
		velocity += gravity;
		CollisionFlags flag = cc.Move(velocity * Time.deltaTime);

		if ((flag & CollisionFlags.Above) != 0 && !((oFlag & CollisionFlags.Above) != 0)) {
			gravity = -gravity / 2;
		}
		oFlag = flag;
	}

	// Update is called once per frame
	void Update() {
		Move();
	}
}
