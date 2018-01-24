using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public enum GhostMovementState {
	RunAway, Idle, ChasePlayer
}

public class GhostMovement : MonoBehaviour {

	// movement config
	protected float gravity = -30f;
	protected float groundDamping = 20f; // how fast do we change direction? higher means faster
	protected float jumpHeight = 1.5f;//3f;
	protected float doubleJumpHeight = 2f;

	protected float walkSpeed = 2f;
	protected float chaseSpeed = 5f;

	protected float normalizedHorizontalSpeed = 0;

	// A non-flipped PlayerTemp sprite faces right (1)
	protected static int spriteFlipped = -1;

	private float aggression;

	protected CharacterController2D _controller;
	public Vector3 _velocity;

	public Vector3 startPosition, targetPosition, newPosition;

	protected delegate void MoveDelegate(RaycastHit2D hit);
	protected MoveDelegate moveDelegate;
	protected GhostMovementState state;

	public Bounds floor;
	protected int layerMask;

	public void Initialize(GhostAIStats stats) {
		aggression = stats.Aggressiveness ();
		chaseSpeed *= ((aggression + .5f) / 2.0f);
	}

	public void Start() {
		moveDelegate = new MoveDelegate(Idle);
		_controller = GetComponent <CharacterController2D>();
		layerMask = 1 << LayerMask.NameToLayer ("Wall") | 1 << LayerMask.NameToLayer("Player");
	}

	protected bool meandering = false;

	protected void ClearMovementState() {
		meandering = false;
	}
		
	public void SetMovementState(GhostMovementState state) {
		this.state = state;
		ClearMovementState ();

		switch(state) {
		case GhostMovementState.RunAway:
			moveDelegate = new MoveDelegate(RunAway);
			break;
		case GhostMovementState.ChasePlayer:
			moveDelegate = new MoveDelegate(ChasePlayer);
			break;
		case GhostMovementState.Idle:
			moveDelegate = new MoveDelegate(Idle);
			break;
		}
	}

	public float smoothDampTime = 0.2f;

	protected bool attackingPlayer = false;

	protected RaycastHit2D hit;
	public void Update() {
		hit = Physics2D.Linecast (transform.position, PlayerController.PlayerPosition, layerMask);
		moveDelegate (hit);
	}

	public void ChasePlayer(RaycastHit2D hit) {
		/*if (hit.collider.tag == "Wall") {
			// If aggression isn't high enough, don't keep hunting.
			// Even if aggression is high, stop hunting after some time?
			if (aggression < .4f) {
				moveDelegate = new MoveDelegate (Idle);
				return;
			}
		}*/
		targetPosition = PlayerController.PlayerPosition;
		MoveToTargetUnclamped (chaseSpeed);
	}

	public void Idle(RaycastHit2D hit) {
		if (hit.collider.tag == "Player") {
			moveDelegate = new MoveDelegate (ChasePlayer);
			return;
		}

		bool closeToTarget = Vector2.Distance (transform.position, targetPosition) < .1f;

		float speed;
		if (meandering) {
			speed = .15f;
			if (closeToTarget)
				targetPosition.x += Random.Range (-.25f, .2f);
		} else {
			speed = .3f;
			targetPosition = startPosition;

			if (closeToTarget)
				meandering = true;
		}
		MoveToTargetClamped (speed);
	}
		
	public void RunAway(RaycastHit2D hit) {
		switch (hit.collider.tag) {
		case "Wall":
			// Should they try to get back to where they were?
			moveDelegate = new MoveDelegate(Idle);
			return;	
		}
			
		targetPosition = new Vector3(transform.position.x - (PlayerController.PlayerPosition - transform.position).normalized.x, transform.position.y, 0);
		MoveToTargetClamped (walkSpeed);
	}


	protected virtual void MoveToTargetClamped(float speed) {
		/*newPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		newPosition.y = transform.position.y;
		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		transform.position = newPosition;*/

		_velocity.x = (targetPosition - transform.position).normalized.x * speed;
		_velocity.y += gravity * Time.deltaTime;
		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;

		if((Mathf.Sign(_velocity.x) > 0) && spriteFlipped == 1) {
			FlipSprite ();
		}
		else if ((Mathf.Sign(_velocity.x) < 0) && spriteFlipped == -1) {
			FlipSprite ();
		}
	}

	protected virtual void MoveToTargetUnclamped(float speed) {
		/*if (_controller.isGrounded) {
			newPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		} else {
			newPosition.x = targetPosition.x;
		}
		newPosition.y = transform.position.y;
		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		transform.position = newPosition;*/

		_velocity.x = (targetPosition - transform.position).normalized.x * speed;

		/*if (_controller.isGrounded && Mathf.Abs (newPosition.x - floor.center.x) >= (floor.extents.x - .2f)) {
			_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
		}*/

		_velocity.y += gravity * Time.deltaTime;
		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;

		if((Mathf.Sign(_velocity.x) > 0) && spriteFlipped == 1) {
			FlipSprite ();
		}
		else if ((Mathf.Sign(_velocity.x) < 0) && spriteFlipped == -1) {
			FlipSprite ();
		}
	}

	// SMoooooooth Damp
	// http://devblog.aliasinggames.com/smoothdamp/

	protected void FlipSprite() {
		transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		spriteFlipped *= -1;
	}

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	public virtual Vector3 GetSpawnPosition(Vector3 size) {
		Vector3 point;
		float minDistance = 3.0f, distance;
		RaycastHit2D hit;

		do {
			point = Room.GetRandomPointInRoom ();
			hit = Physics2D.Raycast (point, Vector2.down, 20.0f, 1 << LayerMask.NameToLayer ("Wall"));
			if(hit.collider) {
				point.y = hit.transform.position.y + hit.transform.GetComponent<SpriteRenderer>().bounds.extents.y + size.y;
				floor = hit.collider.bounds;
			}
			// Just a safeguard in case the raycast bugs out.
			else {
				distance = minDistance * 2;
				continue;
			}

			hit = Physics2D.Raycast (point - new Vector3(size.x * 2, 0, 0), Vector2.left, .5f, 1 << LayerMask.NameToLayer ("Wall"));
			if (hit.collider)
				point.x = hit.collider.transform.position.x + hit.collider.GetComponent<SpriteRenderer> ().bounds.extents.x + size.x;

			hit = Physics2D.Raycast (point + new Vector3(size.x * 2, 0, 0), Vector2.right, .5f, 1 << LayerMask.NameToLayer ("Wall"));
			if (hit.collider)
				point.x = hit.collider.transform.position.x - hit.collider.GetComponent<SpriteRenderer> ().bounds.extents.x - size.x;

			distance = Vector2.Distance(PlayerController.PlayerPosition, point);
		} while (distance < minDistance);

		return point;
	}
}