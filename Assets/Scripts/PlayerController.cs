using UnityEngine;
using System.Collections;
using Utils.Modifier;

public class PlayerController : MonoBehaviour {

	public static PlayerController active;

	public float speed,airSpeed, jumpHeight;
	CharacterController cc;
	public Vector3 velocity = Vector3.zero, gravity = Vector3.zero;
	CollisionFlags oFlag;
	float testFloat;
	bool isOnWall, isWallJumping, isGrounded;
	public bool doWallJumps;

	public GameObject test;

	// Use this for initialization
	void Start () {
		active = this;
		cc = GetComponent<CharacterController>();
	}

	void Move() {
		velocity = Vector3.zero;


		CollisionFlags flag = cc.collisionFlags;

		if (isGrounded && !isOnWall && !isWallJumping) {
			gravity = Vector3.zero;
			velocity += transform.forward * Input.GetAxisRaw("Vertical") * speed;
			velocity += transform.right * Input.GetAxisRaw("Horizontal") * speed;


		} else {
			gravity += Physics.gravity * Time.deltaTime;
			velocity += transform.forward * Input.GetAxisRaw("Vertical") * airSpeed;
			velocity += transform.right * Input.GetAxisRaw("Horizontal") * airSpeed;
		}
		if ((isGrounded || !isOnWall) && isWallJumping) {
			isWallJumping = false;
		}

		if ((flag & CollisionFlags.Above) != 0 && !((oFlag & CollisionFlags.Above) != 0) && gravity.y > 0 ) {
			gravity = -gravity / 2;
		}

		if (Input.GetButtonDown("Jump") && (isGrounded || isOnWall) && !isWallJumping) {
			gravity = transform.up * jumpHeight;
			isGrounded = false;
			if (isOnWall) {
				isWallJumping = true;
				Debug.Log("Wall Jump");
			}
		}

		

		
		//velocity.Normalize();


		if (Input.GetButton("Sprint") && isOnWall && !isWallJumping) {
			gravity = Vector3.zero;
		}

		velocity += gravity;

		cc.Move(velocity * Time.deltaTime);

		oFlag = flag;
	}

	// Update is called once per frame
	void Update() {
		Move();
	}
	void FixedUpdate() {
		isOnWall = Physics.CheckCapsule(transform.position + transform.up / 3, transform.position - transform.up / 3, 0.6f, 1 << 8);
		Debug.DrawRay(transform.position, -transform.up * (cc.bounds.extents.y + 0.1f));
		isGrounded = Physics.Raycast(transform.position,-transform.up,cc.bounds.extents.y+0.1f);
	}
}
