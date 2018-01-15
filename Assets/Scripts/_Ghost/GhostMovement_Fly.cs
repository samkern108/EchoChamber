using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement_Fly : GhostMovement {

	protected override void MoveToTargetClamped(float speed) {
		MoveToTargetUnclamped (speed);
	}

	private float sinOffset = 0.0f;
	private float amplitude = 1f;
	private float sinDamping = 5f;

	protected override void MoveToTargetUnclamped(float speed) {

		_velocity = (targetPosition - transform.position).normalized * speed;

		sinOffset += (Time.deltaTime * sinDamping);
		_velocity.y += (amplitude - (amplitude / 2)) * Mathf.Sin (sinOffset);

		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;
	}

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	public override Vector3 GetSpawnPosition(Vector3 size) {
		Vector3 point;
		float minDistance = 3.0f, distance;
		do {
			point = Room.GetRandomPointInRoom ();
			distance = Vector2.Distance(PlayerController.PlayerPosition, point);
		} while (distance < minDistance);

		return point;
	}
}
