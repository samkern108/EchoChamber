using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	public static Bounds bounds;

	void Start () {
		bounds = GetComponent<SpriteRenderer> ().bounds;
	}

	// I know these are busted right now.

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	public static Vector3 GetRandomPointOnFloorAvoidingPoints(Vector2[] pointsToAvoid) {
		// * 6/7 to prevent it from spawning on the absolute limits of the box.
		float x = bounds.extents.x * 6 / 7;
		float y = bounds.extents.y * 6 / 7;
		Vector3 center = bounds.center;

		Vector3 point = Vector3.zero;
		point.x = Random.Range (center.x - x, center.x + x);
		point.y = Random.Range (center.y - y, center.y + y);

		RaycastHit2D hit = Physics2D.Raycast (point, Vector2.down, 20.0f, 1 << LayerMask.NameToLayer("Wall"));
		point.y = hit.collider.transform.position.y + hit.collider.GetComponent<SpriteRenderer>().bounds.extents.y;

		float distance = Vector2.Distance(PlayerController.PlayerPosition, point);
		float minDistance = 2.0f;

		if (distance < minDistance)
			return GetRandomPointInRoomAvoidingPoints(pointsToAvoid);

		return point;
	}

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	public static Vector3 GetRandomPointInRoomAvoidingPoints(Vector2[] pointsToAvoid) {
		float x = bounds.extents.x;
		float y = bounds.extents.y;
		Vector3 center = bounds.center;

		Vector3 point = Vector3.zero;
		point.x = Random.Range (center.x - x, center.x + x);
		point.y = Random.Range (center.y - y, center.y + y);

		float distance = Vector2.Distance(PlayerController.PlayerPosition, point);
		float minDistance = 2.0f;
		//Mathf.Repeat ();

		if (distance < minDistance)
			return GetRandomPointInRoomAvoidingPoints (pointsToAvoid);

		return point;
	}
}
