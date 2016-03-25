using UnityEngine;
using System.Collections;

public class WeaponList : MonoBehaviour {

	public Weapon[] weapons;

	public Weapon active;
	int lastWeapon;
	public int curWeapon = -1;

	// Use this for initialization
	void Start () {
		foreach (Weapon w in weapons)
			w.isActive = false;

		SetWeapon(curWeapon);
	}
	
	public void SetWeapon(int i) {
		if(curWeapon >= 0 && curWeapon < weapons.Length)
			weapons[lastWeapon].isActive = false;

		if (!(i >= 0 && i < weapons.Length))
			return;

		lastWeapon = curWeapon;
		curWeapon = i;

		active = weapons[curWeapon];
		weapons[curWeapon].isActive = true;

	}

	// Update is called once per frame
	void Update () {
		
		curWeapon += Input.mouseScrollDelta.y == 0 ? 0: Mathf.RoundToInt(Mathf.Sign(Input.mouseScrollDelta.y));
		curWeapon = Mathf.Clamp(curWeapon, 0, weapons.Length);

		SetWeapon(curWeapon);

		if (Input.GetButton("Fire1"))
			active.Fire();
    }
}
