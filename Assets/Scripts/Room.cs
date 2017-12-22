using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	public static Bounds bounds;
	private static int layermask;
	public static float roomSidesBuffer = 6.0f/7.0f;

	void Start () {
		bounds = GetComponent<SpriteRenderer> ().bounds;
		layermask = 1 << LayerMask.NameToLayer ("Wall");
	}

	// I know these are busted right now.

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	public static Vector3 GetRandomPointInRoom() {
		// roomSidesBuffer prevents it from spawning on the absolute limits of the box.
		// roomSidesBuffer prevents it from spawning on the absolute limits of the box.
		float x = bounds.extents.x * roomSidesBuffer;
		float y = bounds.extents.y * roomSidesBuffer;
		Vector3 center = Room.bounds.center;

		Vector3 point = Vector3.zero;
		point.x = Random.Range (center.x - x, center.x + x);
		point.y = Random.Range (center.y - y, center.y + y);

		return point;
	}
}
