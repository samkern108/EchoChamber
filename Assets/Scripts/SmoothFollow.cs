using UnityEngine;
using System.Collections;
using Prime31;


public class SmoothFollow : MonoBehaviour
{
	public Transform target;
	public float smoothDampTime = 0.2f;
	[HideInInspector]
	public new Transform transform;
	[HideInInspector]
	public Vector3 cameraOffset;
	public bool useFixedUpdate = false;

	private CharacterController2D _playerController;
	private PlayerController _playerMovement;
	private Vector3 _smoothDampVelocity;

	private Vector2 bounds_x;
	private Vector2 bounds_y;
	private float wallWidth = .6f;

	void Awake()
	{
		cameraOffset.z = -10;
		transform = gameObject.transform;
		_playerController = target.GetComponent<CharacterController2D>();
		_playerMovement = target.GetComponent <PlayerController>();

		Bounds b = Room.bounds;
		bounds_x = new Vector2 (b.center.x - (b.extents.x + wallWidth), b.center.x + (b.extents.x + wallWidth));
		bounds_y = new Vector2 (b.center.y - (b.extents.y + wallWidth), b.center.y + (b.extents.y + wallWidth));
	}


	void LateUpdate()
	{
		if( !useFixedUpdate )
			updateCameraPosition();
	}


	void FixedUpdate()
	{
		if( useFixedUpdate )
			updateCameraPosition();
	}


	// Maybe new camera controller that takes the player's position within the rectangle into account.
	void updateCameraPosition()
	{
		cameraOffset.x = 0f;
		cameraOffset.y = 0f;
		Vector3 targetPosition = Vector3.zero;
		targetPosition.x = /*target.position.x;*/Mathf.Clamp (target.position.x, bounds_x.x, bounds_x.y);
		targetPosition.y = /*target.position.y;*/Mathf.Clamp (target.position.y, bounds_y.x, bounds_y.y);
		Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition + cameraOffset, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;
			//Vector3.Lerp(transform.position, target.position + cameraOffset, 2.0f * Time.deltaTime);

	}

}
