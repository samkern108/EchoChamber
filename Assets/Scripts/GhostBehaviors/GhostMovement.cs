using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public enum GhostMovementState {
	RunAway, Idle, ChasePlayer
}

public class GhostMovement : MonoBehaviour {

	// movement config
	private float gravity = -30f;
	private float runSpeed = 7f;
	private float groundDamping = 20f; // how fast do we change direction? higher means faster
	private float jumpHeight = 1.5f;//3f;
	private float doubleJumpHeight = 2f;

	private float normalizedHorizontalSpeed = 0;

	// A non-flipped PlayerTemp sprite faces right (1)
	public static int spriteFlipped = 1;

	public float aggression;

	private CharacterController2D _controller;
	public Vector3 _velocity;

	public Vector3 startPosition, targetPosition, newPosition;

	delegate void MoveDelegate(RaycastHit2D hit);
	private MoveDelegate moveDelegate;
	private GhostMovementState state;

	public Bounds floor;
	private int layerMask;

	public void Start() {
		moveDelegate = new MoveDelegate(Idle);
		_controller = GetComponent <CharacterController2D>();
		layerMask = 1 << LayerMask.NameToLayer ("Wall") | 1 << LayerMask.NameToLayer("Player");
	}

	private bool meandering = false;

	private void ClearMovementState() {
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

	private bool attackingPlayer = false;

	private RaycastHit2D hit;
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


	private void MoveToTargetClamped() {
		newPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		newPosition.y = transform.position.y;
		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		transform.position = newPosition;

		_velocity.y += gravity * Time.deltaTime;
		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;
	}

	private void MoveToTargetUnclamped() {
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
		
	private void MoveToTarget() {

		newPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		newPosition.y = transform.position.y;
		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		//Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;

		if (_controller.isGrounded && Mathf.Abs (newPosition.x - floor.center.x) >= (floor.extents.x - .2f)) {
			_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
		}

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;
			
		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
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

	private void FlipPlayer() {
		transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		spriteFlipped *= -1;
	}
}