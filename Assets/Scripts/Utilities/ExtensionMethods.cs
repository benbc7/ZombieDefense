using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {

	public static Vector2 XZ (this Vector3 v3) {
		return new Vector2 (v3.x, v3.z);
	}

	public static Vector3 RoundToInt (this Vector3 v3) {
		return new Vector3 (Mathf.RoundToInt (v3.x), Mathf.RoundToInt (v3.y), Mathf.RoundToInt (v3.z));
	}
}