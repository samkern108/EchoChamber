using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prime31;

public class PlayerController : MonoBehaviour, IRestartObserver {

	// movement config
	private float gravity = -30f;
	private float runSpeed = 7f;
	private float groundDamping = 20f; // how fast do we change direction? higher means faster
	private float inAirDamping = 5f;
	private float jumpHeight = 3f;
	private float doubleJumpHeight = 2f;

	private float normalizedHorizontalSpeed = 0;

	// A non-flipped PlayerTemp sprite faces right (1)
	public static int spriteFlipped = 1;

	private CharacterController2D _controller;
	private Animate _animate;
	private RaycastHit2D _lastControllerColliderHit;
	public Vector3 _velocity;

	private bool shooting = true, doubleJumping = false;
	public GameObject projectile;
	private Vector2 playerSize, playerExtents;
	private float projectileSpeed = 8.0f;

	void Awake()
	{
		GetComponent <SpriteRenderer>().color = Palette.Invisible;

		NotificationMaster.restartObservers.Add (this);

		_animate = GetComponent<Animate>();
		_controller = GetComponent<CharacterController2D>();

		PlayerTransform = transform;
		playerSize = GetComponent <SpriteRenderer> ().bounds.size;
		playerExtents = GetComponent <SpriteRenderer> ().bounds.extents;
	}

	public void Start() {
		SpawnPlayer ();
	}

	void Update()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis ("Vertical");

		bool jump = Input.GetKeyDown (KeyCode.JoystickButton18) || Input.GetKeyDown (KeyCode.UpArrow);
		bool jumpCancel = Input.GetKeyUp (KeyCode.JoystickButton18) || Input.GetKeyUp (KeyCode.UpArrow);

		bool fire = Input.GetAxisRaw ("Fire") <= 0.0f;

		if (!fire)
			shooting = false;
		else if (fire && !shooting) {
			_animate.AnimateToColorAndBack (Palette.PlayerColor, Color.red, .05f);

			shooting = true;
			NotificationMaster.SendPlayerShootNotification ();
			AudioManager.PlayEnemyShoot ();

			//float direction = spriteFlipped ? -1 : 1;
			Vector2 direction = new Vector2(x, y).normalized;
			if (direction == Vector2.zero)
				direction.x = spriteFlipped;
			GameObject missile = Instantiate (projectile, ProjectileManager.myTransform);
			missile.transform.position = transform.position + Vector3.Scale(playerSize, direction.normalized);
			missile.GetComponent <Missile>().Initialize(direction.normalized, projectileSpeed);
		}

		// xbox
		/*float fireX = Input.GetAxis ("FireX");
		float fireY = Input.GetAxis ("FireY");
		if (Mathf.Abs(fireX) > .5f||Mathf.Abs(fireY) > .5f)
			fire = true;*/

		if(Mathf.Abs(x) > .3)
		{
			normalizedHorizontalSpeed = x;
			if (spriteFlipped * x < 0)
				FlipPlayer ();
		}
		else
			normalizedHorizontalSpeed = 0;

		if (_controller.isGrounded) {
			doubleJumping = false;
			_velocity.y = 0;

			// we can only jump whilst grounded
			if (jump) {
				_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
				AudioManager.PlayPlayerJump ();
			}
		} else {
			if (!doubleJumping && jump) {
				doubleJumping = true;
				AudioManager.PlayPlayerJump ();
				_velocity.y = Mathf.Sqrt (2f * doubleJumpHeight * -gravity);
			}
			else if (jumpCancel && (_velocity.y > 0))
				_velocity.y *= .5f;
		}

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = groundDamping;//_controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		if( _controller.isGrounded && Input.GetKey(KeyCode.DownArrow) )
		{
			_velocity.y *= 3f;
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}
		
	public void FlipPlayer() {
		AudioManager.PlayPlayerTurn ();
		transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		spriteFlipped *= -1;
	}

	private void SpawnPlayer() {
		GetComponent <SpriteRenderer>().color = Palette.Invisible;

		Vector3 newPosition = Vector3.zero;
		RaycastHit2D hit;
		// Uhh this is hacky but whatever
		for(int i = 0; i < 5; i++) {
			hit = Physics2D.Linecast (transform.position + new Vector3(0, playerExtents.y, 0), newPosition - new Vector3(0, playerExtents.y * 2, 0), 1 << LayerMask.NameToLayer("Wall"));
			if (hit.collider) {
				newPosition.y = hit.transform.position.y + hit.transform.GetComponent<SpriteRenderer> ().bounds.extents.y + playerExtents.y;
				transform.position = newPosition;
				break;
			}
		}

		_animate.AnimateToColor (Palette.Invisible, Palette.PlayerColor, .5f);
	}

	public void Restart() {
		_velocity = Vector3.zero;
		SpawnPlayer ();
		gameObject.SetActive (true);
	}

	private static Transform _playerTransform;
	public static Transform PlayerTransform
	{
		set { if(_playerTransform == null) _playerTransform = value; }
	}
	public static Vector3 PlayerPosition {
		get { return _playerTransform.position; }
	}
}
