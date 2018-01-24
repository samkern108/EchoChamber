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

	RaycastHit2D wallhit;

	protected override void MoveToTargetUnclamped(float speed) {

		Vector3 targetVector = (targetPosition - transform.position);

		Debug.DrawRay (transform.position, targetVector, Color.blue, .1f);

		wallhit = Physics2D.Linecast (transform.position, targetPosition, 1 << LayerMask.NameToLayer ("Wall"));
		if (wallhit.collider != null) {

			/*float angle = wallhit.distance.Map (0.0f, 2.0f, 89.5f, 0.0f);

			angle *= Mathf.Deg2Rad;

			targetVector.x = targetVector.x * Mathf.Cos (angle) - targetVector.y * Mathf.Sin (angle);
			targetVector.y = targetVector.y * Mathf.Cos (angle) + targetVector.x * Mathf.Sin (angle);

			Debug.Log (targetVector.magnitude);*/

			float distance = 1.0f - (wallhit.distance / targetVector.magnitude);

			Vector2 lerpVector = wallhit.normal;
			lerpVector.x = lerpVector.y;
			lerpVector.y = lerpVector.x;

			targetVector = Vector2.Lerp (targetVector.normalized, lerpVector.normalized, distance);
		}

		_velocity = targetVector.normalized * speed;

		Debug.DrawRay (transform.position, targetVector, Color.red, .1f);

		sinOffset += (Time.deltaTime * sinDamping);
		_velocity.y += (amplitude - (amplitude / 2)) * Mathf.Sin (sinOffset);

		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;

		if(_velocity.x * spriteFlipped > (speed * .9f)) {
			FlipSprite ();
		}
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
