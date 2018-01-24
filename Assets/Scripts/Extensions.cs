using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

	public static float Map (this float value, float from1, float to1, float from2, float to2) {
		value = Mathf.Clamp (value, Mathf.Min(from1, to1), Mathf.Max(from1, to1));
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}
