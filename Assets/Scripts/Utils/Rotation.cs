using UnityEngine;
using System.Collections.Generic;

namespace Utils {

	public static class Rotation {
		public static Quaternion Euler(Vector3 angles) {
			return Quaternion.Euler(angles);
		}
		public static Quaternion Euler(float x, float y, float z) {
			return Quaternion.Euler(x, y, z);
		}

		/// <summary>
		/// Returns an Isometric View Angle
		/// </summary>
		/// <returns>A Quaternion with Euler(45,45,0)</returns>
		public static Quaternion IsoAngle() {
			return Quaternion.Euler(45, 45, 0);
		}
	}


}
