using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Weapon {

	public GameObject weapon;
	public Vector3 localBulletSpawn;
	public Bullet bullet;
	public float firerate;
	public string weaponName;

	[SerializeField]
	bool _isActive;
	public bool isActive {
		get {
			return _isActive;
		}
		set {
			_isActive = value;
			OnActiveChange();
		}
	}

	bool canFire;
	float timelFirerate = 0, timeFirerate = -1;

	// Use this for initialization
	public Weapon() {



	}

	public void Fire() {
		timelFirerate = Time.time;
		if (timelFirerate > timeFirerate) {
			GameObject temp = ((Bullet)GameObject.Instantiate(bullet, localBulletSpawn, Quaternion.identity)).gameObject;

			temp.transform.SetParent(weapon.transform, false);
			temp.transform.SetParent(null, true);

			temp.transform.rotation = weapon.transform.rotation;

			temp.GetComponent<Bullet>().Start();

			timeFirerate = Time.time + firerate;
		}
	}
	
	// Update is called once per frame
	void OnActiveChange () {
		weapon.SetActive(isActive);
		if (!isActive)
			return;



	}
}
