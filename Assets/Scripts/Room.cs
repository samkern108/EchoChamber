using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	public static Bounds bounds;

	void Start () {
		bounds = GetComponent<SpriteRenderer> ().bounds;
	}

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	public static Vector3 GetRandomPointInRoom() {
		float x = bounds.extents.x;
		float y = bounds.extents.y;
		Vector3 center = bounds.center;

		Vector3 point = Vector3.zero;
		point.x = Random.Range (center.x - x, center.x + x);
		point.y = Random.Range (center.y - y, center.y + y);

		float distance = Vector2.Distance(PlayerController.PlayerPosition, point);
		float minDistance = 2.0f;
		//Mathf.Clamp ();

		if (distance < minDistance)
			return GetRandomPointInRoom ();

		/*float diffx = px - x;
		float diffy = py - y;
		if(Mathf.Abs(px - x) < Mathf.Abs(px - y))*/

		return point;
	}

	/*public static Vector3 WrapWithinRoom(Vector3 newPosition) {
		if (newPosition.x < bounds.min.x)
			newPosition.x = bounds.max.x;
		else if (newPosition.x > bounds.max.x)
			newPosition.x = bounds.min.x;

		if (newPosition.y < bounds.min.y)
			newPosition.y = bounds.max.y;
		else if (newPosition.y > bounds.max.y)
			newPosition.y = bounds.min.y;

		return newPosition;
	}*/
}
