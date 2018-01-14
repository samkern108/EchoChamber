using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement_Fly : GhostMovement {

	protected override void MoveToTargetClamped() {
		MoveToTargetUnclamped ();
	}

	private float sinOffset = 0.0f;
	private float amplitude = .3f;
	private float sinDamping = .5f;

	protected override void MoveToTargetUnclamped() {
		sinOffset += (Time.deltaTime * sinDamping);

		float height = (amplitude - (amplitude / 2)) * Mathf.Sin (sinOffset);
		newPosition = PlayerController.PlayerPosition;
		newPosition.y += height;

		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		transform.position = newPosition;

		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;
	}

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	public override Vector3 GetSpawnPosition(Vector3 size) {
		Vector3 point;
		float minDistance = 3.0f, distance;
		RaycastHit2D hit;

		do {
			point = Room.GetRandomPointInRoom ();
			distance = Vector2.Distance(PlayerController.PlayerPosition, point);
		} while (distance < minDistance);

		return point;
	}
}
