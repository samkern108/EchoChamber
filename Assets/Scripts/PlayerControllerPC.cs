using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prime31;

public class PlayerControllerPC : MonoBehaviour, IRestartObserver {

	// Tells us what our collison state was in the last frame
	public CharacterController2D.CharacterCollisionState2D flags;

	public bool isGrounded;
	public bool isJumping;

	// movement config
	private float gravity = -40f;
	private float runSpeed = 8f;
	private float groundDamping = 20f; // how fast do we change direction? higher means faster
	private float inAirDamping = 5f;
	private float jumpHeight = 3f;

	private float normalizedHorizontalSpeed = 0;

	// A non-flipped PlayerTemp sprite faces right
	public static bool spriteFlipped = false;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	public Vector3 _velocity;

	public GameObject projectile;
	private float playerWidth;
	private float projectileSpeed = 8.0f;

	private static int index = 0;
	public static float[] ghostActions = new float[500];


	void Awake()
	{
		NotificationMaster.restartObservers.Add (this);
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		_controller.onTriggerEnterEvent += onTriggerEnterEvent;

		PlayerTransform = transform;
		playerWidth = GetComponent <SpriteRenderer> ().bounds.extents.x * 2.0f;
	}

	public void OnEnable() {
		_animator.Play("Appear");
	}

	#region Event Listeners

	private bool activated = false;
	void onTriggerEnterEvent( Collider2D col )
	{
		if (col.tag == "Exit") {
			activated = true;
			float[] ghostPositionsChopped = new float[index + 1];
			Array.Copy (ghostActions, ghostPositionsChopped, index);
			ghostActions = new float[500];
			index = 0;
			GhostManager.instance.StartCapturingNewGhost (ghostPositionsChopped);
			Invoke ("Deactivate", .5f);
		}
	}

	#endregion

	// Just so the player can't touch exits twice
	public void Deactivate() {
		activated = false;
	}


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		if( Input.GetKey( KeyCode.RightArrow ) )
		{
			normalizedHorizontalSpeed = 1;
			if (spriteFlipped)
				FlipPlayer ();
		}
		else if( Input.GetKey( KeyCode.LeftArrow ) )
		{
			normalizedHorizontalSpeed = -1;
			if (!spriteFlipped)
				FlipPlayer ();
		}
		else
		{
			normalizedHorizontalSpeed = 0;
		}

		if (_controller.isGrounded) {
			
			_velocity.y = 0;

			// we can only jump whilst grounded
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
				_animator.Play( "Jump" );
				AudioManager.PlayPlayerJump ();
			}
		}
		else {
			if (Input.GetKeyUp (KeyCode.UpArrow) && (_velocity.y > 0)) {
				_velocity.y *= .5f;
			}
		}

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		if( _controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			_velocity.y *= 3f;
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		bool fire = Input.GetKeyDown (KeyCode.Space);

		if (fire) {
			AudioManager.PlayEnemyShoot ();
			_animator.Play("Shoot");

			float direction = spriteFlipped ? -1 : 1;
			GameObject missile = Instantiate (projectile);
			missile.transform.position = transform.position + new Vector3((playerWidth * direction), 0, 0);
			missile.GetComponent <Missile>().Initialize(Vector3.right * direction, projectileSpeed);

			shoot = true;
		}
	}

	private bool shoot = false;

	public void FixedUpdate() {
		ghostActions [index++] = transform.position.x;
		ghostActions [index++] = transform.position.y;
		ghostActions [index++] = spriteFlipped ? 1 : -1;
		ghostActions [index++] = shoot ? 1 : -1;
		shoot = false;

		if (index >= ghostActions.Length - 4) {
			float[] temp = new float[ghostActions.Length * 2];
			ghostActions.CopyTo(temp, 0);
			ghostActions = temp;
		}
	}
		
	public void FlipPlayer() {
		AudioManager.PlayPlayerTurn ();
		transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

		// Fixes the sprite not flipping due to animator issues. If we can flip the sprite in the animator, that will be a lot easier.
		spriteFlipped = !spriteFlipped;
		//GetComponent <SpriteRenderer>().flipX = spriteFlipped;
	}

	private static Transform _playerTransform;
	public static Transform PlayerTransform
	{
		set { if(_playerTransform == null) _playerTransform = value; }
	}
	public static Vector3 PlayerPosition {
		get { return _playerTransform.position; }
	}

	public void Restart() {
		transform.position = Vector3.zero;
		gameObject.SetActive (true);
		ghostActions = new float[500];
	}
}
