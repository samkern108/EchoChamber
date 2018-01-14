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
	protected float runSpeed = 7f;
	protected float groundDamping = 20f; // how fast do we change direction? higher means faster
	protected float jumpHeight = 1.5f;//3f;
	protected float doubleJumpHeight = 2f;

	protected float normalizedHorizontalSpeed = 0;

	// A non-flipped PlayerTemp sprite faces right (1)
	protected static int spriteFlipped = 1;

	public float aggression;

	protected CharacterController2D _controller;
	public Vector3 _velocity;

	public Vector3 startPosition, targetPosition, newPosition;

	protected delegate void MoveDelegate(RaycastHit2D hit);
	protected MoveDelegate moveDelegate;
	protected GhostMovementState state;

	public Bounds floor;
	protected int layerMask;

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
		switch (hit.collider.tag) {
		case "Wall":
			// If aggression is of a certain level, keep hunting?
			break;
		case "Player":
			break;
		}
		targetPosition = PlayerController.PlayerPosition;
		runSpeed = 1.0f;
		MoveToTargetUnclamped ();
	}

	public void Idle(RaycastHit2D hit) {
		switch (hit.collider.tag) {
		case "Player":
			if (aggression > 0.2f)
				moveDelegate = new MoveDelegate (ChasePlayer);
			else if(aggression == 0.0f)
				moveDelegate = new MoveDelegate (RunAway);
			return;	
		}

		bool closeToTarget = Vector2.Distance (transform.position, targetPosition) < .1f;

		if (meandering) {
			runSpeed = .15f;
			if (closeToTarget)
				targetPosition.x += Random.Range (-.25f, .2f);
		} else {
			runSpeed = .3f;
			targetPosition = startPosition;

			if (closeToTarget)
				meandering = true;
		}
		MoveToTargetClamped ();
	}
		
	public void RunAway(RaycastHit2D hit) {
		switch (hit.collider.tag) {
		case "Wall":
			// Should they try to get back to where they were?
			moveDelegate = new MoveDelegate(Idle);
			return;	
		}

		//TODO(samkern): Make it move away from the player. =__= not just left. I'm tired.
		targetPosition = new Vector3(transform.position.x - 2.0f, transform.position.y, 0);
		runSpeed = .4f;
		MoveToTargetClamped ();
	}


	protected virtual void MoveToTargetClamped() {
		newPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		newPosition.y = transform.position.y;
		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		transform.position = newPosition;

		_velocity.y += gravity * Time.deltaTime;
		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;
	}

	protected virtual void MoveToTargetUnclamped() {
		if (_controller.isGrounded) {
			newPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		} else {
			newPosition.x = targetPosition.x;
		}
		newPosition.y = transform.position.y;
		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		transform.position = newPosition;

		if (_controller.isGrounded && Mathf.Abs (newPosition.x - floor.center.x) >= (floor.extents.x - .2f)) {
			_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
		}

		_velocity.y += gravity * Time.deltaTime;
		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;
	}

	// SMoooooooth Damp
	// http://devblog.aliasinggames.com/smoothdamp/
		
	protected void MoveToTarget() {

		newPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		newPosition.y = transform.position.y;
		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		//Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;

		if (_controller.isGrounded && Mathf.Abs (newPosition.x - floor.center.x) >= (floor.extents.x - .2f)) {
			_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
		}

		_velocity.y += gravity * Time.deltaTime;
			
		_controller.move( _velocity * Time.deltaTime );

		_velocity = _controller.velocity;
		/*
		if(Mathf.Abs(x) > .3)
		{
			normalizedHorizontalSpeed = x;
			if (spriteFlipped * x < 0)
				FlipPlayer ();
		}
		else
			normalizedHorizontalSpeed = 0;

		if (_controller.isGrounded) {
			_velocity.y = 0;

			if (jump) {
				_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
				AudioManager.PlayPlayerJump ();
			}
		}

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = groundDamping;//_controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );
*/
	}

	protected void FlipPlayer() {
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