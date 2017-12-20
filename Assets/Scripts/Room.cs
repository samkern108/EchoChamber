using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	public static Bounds bounds;
	private static int layermask;
	private static float roomSidesBuffer = 6.0f/7.0f;

	void Start () {
		bounds = GetComponent<SpriteRenderer> ().bounds;
		layermask = 1 << LayerMask.NameToLayer ("Wall");
	}

	// I know these are busted right now.

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	public static Vector3 GetRandomPointOnFloorAvoidingPoints(Vector2[] pointsToAvoid, Vector2 extents) {
		// * 6/7 to prevent it from spawning on th e absolute limits of the box.
		float x = bounds.extents.x * roomSidesBuffer;
		float y = bounds.extents.y * roomSidesBuffer;
		Vector3 center = bounds.center;

		Vector3 point = Vector3.zero;
		point.x = Random.Range (center.x - x, center.x + x);
		point.y = Random.Range (center.y - y, center.y + y);

		RaycastHit2D hit = Physics2D.Raycast (point, Vector2.down, 20.0f, layermask);

		/*Debug.Log ("1:  " + point.y);

		Debug.Log ("2:  " + hit.transform.position.y);

		Debug.Log ("3:  " + hit.transform.position.y + hit.transform.GetComponent<SpriteRenderer>().bounds.extents.y);

		Debug.Log ("4:  " + hit.transform.position.y + hit.transform.GetComponent<SpriteRenderer>().bounds.extents.y + extents.y);*/

		point.y = hit.transform.position.y + hit.transform.GetComponent<SpriteRenderer>().bounds.extents.y + extents.y;

		/*hit = Physics2D.Raycast (point - new Vector3(0, extents.y, 0), Vector2.left, .5f, layermask);
		if (hit.collider) {
			point.x = hit.collider.transform.position.x + hit.collider.GetComponent<SpriteRenderer> ().bounds.extents.x + extents.x;
		}

		hit = Physics2D.Raycast (point - new Vector3(0, extents.y, 0), Vector2.right, .5f, layermask);
		if (hit.collider) {
			point.x = hit.collider.transform.position.x - hit.collider.GetComponent<SpriteRenderer> ().bounds.extents.x - extents.x;
		}*/

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
