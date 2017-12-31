using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public enum GhostMovementState {
	RunAway, Idle, Hunt, Chase, Attack
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

	private CharacterController2D _controller;
	public Vector3 _velocity;

	public Vector3 startPosition, targetPosition, newPosition;

	delegate void MoveDelegate();
	private MoveDelegate moveDelegate;

	public Bounds floor;

	public void Start() {
		moveDelegate = new MoveDelegate(Idle);
		_controller = GetComponent <CharacterController2D>();
	}

	public void SetMovementState(GhostMovementState state) {
		switch(state) {
		case GhostMovementState.RunAway:
			moveDelegate = new MoveDelegate(RunAway);
			break;
		case GhostMovementState.Attack:
			moveDelegate = new MoveDelegate(Attack);
			break;
		case GhostMovementState.Chase:
			moveDelegate = new MoveDelegate(ChasePlayer);
			break;
		case GhostMovementState.Hunt:
			moveDelegate = new MoveDelegate(HuntPlayer);
			break;
		case GhostMovementState.Idle:
			moveDelegate = new MoveDelegate(Idle);
			break;
		}
	}

	public float smoothDampTime = 0.2f;

	public void Update() {
		moveDelegate ();
		if (Vector2.Distance (targetPosition, transform.position) > .01f) {
			MoveToTarget ();
		}
	}

	public void Attack() {
		targetPosition = PlayerController.PlayerPosition;
		runSpeed = 1.0f;
	}

	public void Idle() {
		targetPosition = startPosition;
		runSpeed = .5f;
	}
		
	private void MoveToTarget() {

		newPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		newPosition.y = transform.position.y;
		newPosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);

		//Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;

		if (Mathf.Abs (newPosition.x - floor.center.x) >= (floor.extents.x - .5f)) {
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

	public void HuntPlayer() {
		Vector3 targetPosition = PlayerController.PlayerPosition;
		targetPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		targetPosition.y = transform.position.y;
		Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
		//Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;
	}

	public void RunAway() {
	}

	public void ChasePlayer() {
	}

	private void FlipPlayer() {
		transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		spriteFlipped *= -1;
	}
}
	
public class GhostAttack : MonoBehaviour {

	private GameObject projectile;

	private bool shooting = false;
	private float detectionRampUp = .7f;
	private float shootCooldown = 1.0f;
	private float projectileSpeed = 7.0f;
	private Animate animate;

	public void Start() {
		animate = GetComponent<Animate> ();
		projectile = ResourceLoader.LoadProjectile("EnemyMissile");
	}

	public void StartShooting() {
		if (!shooting)
			StartCoroutine ("Co_Shoot");
	}

	public void StopShooting() {
		shooting = false;
	}

	private IEnumerator Co_Shoot()
	{
		shooting = true;
		yield return new WaitForSeconds (detectionRampUp);
		while (shooting) {
			Shoot ();
			yield return new WaitForSeconds (shootCooldown);
		}
	}

	private void Shoot() {
		AudioManager.PlayEnemyShoot ();
		animate.AnimateToColorAndBack (Palette.EnemyColor, Color.red, .2f);
		Vector3 direction = (PlayerController.PlayerPosition - transform.position).normalized;
		GameObject missile = Instantiate (projectile, ProjectileManager.myTransform);
		missile.transform.position = transform.position;
		missile.GetComponent <Missile>().Initialize(direction, projectileSpeed);
	}
}