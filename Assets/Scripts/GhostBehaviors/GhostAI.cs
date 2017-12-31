using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAI : MonoBehaviour {

	public GhostAIStats stats;

	private GhostAttack attack;

	public GameObject projectile;

	private LayerMask layerMask;
	private Bounds floor;

	private bool detectedPlayer = false;
	private float detectionRadius = 4.0f;

	private Vector3 startPosition, persistentTargetPosition;

	private Animate animate;
	private Vector3 size;

	private Color color;

	public void Initialize(GhostAIStats stats) {
		this.stats = stats;

		if (stats.shotsFired > 0) {
			attack = gameObject.AddComponent <GhostAttack>();
	//		GetComponent<GhostAttack> ().Initialize (stats);
		}

		gameObject.AddComponent <GhostMovement>();

		detectionRadius = Room.bounds.extents.x;

		size = GetComponent <SpriteRenderer>().bounds.extents;

		startPosition = GetSpawnPosition();
		transform.position = startPosition;

		layerMask = 1 << LayerMask.NameToLayer ("Wall") | 1 << LayerMask.NameToLayer("Player");

		if (stats.Aggressiveness () == 0.0f) {
			color = Color.white;
		} else {
			color = Color.black;
			color.r = stats.Aggressiveness ();
			color.g = stats.Aggressiveness ();
		}

		animate = GetComponent <Animate>();
		animate.AnimateToColor (Palette.Invisible, color, .3f);
	}

	void Awake() {
		GetComponent <SpriteRenderer> ().color = Palette.Invisible;
	}

	/** Tries to avoid positioning the point too near the player (and perhaps too near other enemies?). */
	private Vector3 GetSpawnPosition() {
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
			if (hit.collider) {
				point.x = hit.collider.transform.position.x + hit.collider.GetComponent<SpriteRenderer> ().bounds.extents.x + size.x;
			}

			hit = Physics2D.Raycast (point + new Vector3(size.x * 2, 0, 0), Vector2.right, .5f, 1 << LayerMask.NameToLayer ("Wall"));
			if (hit.collider) {
				point.x = hit.collider.transform.position.x - hit.collider.GetComponent<SpriteRenderer> ().bounds.extents.x - size.x;
			}

			distance = Vector2.Distance(PlayerController.PlayerPosition, point);
		} while (distance < minDistance);

		return point;
	}

	void Update () {
		RaycastHit2D hit = Physics2D.Linecast (transform.position, PlayerController.PlayerPosition, layerMask);
		if (hit.collider.tag == "Wall" || (hit.collider.tag == "Player" && hit.distance > detectionRadius)) {
			MoveToStart ();
			if (detectedPlayer) {
				detectedPlayer = false;
				if(attack) attack.StopShooting ();
				animate.AnimateToColor (Palette.EnemyColor, color, .3f);
			}
			return;
		} else {
			MoveToPlayer ();
			if (!detectedPlayer) {
				detectedPlayer = true;
				if(attack) attack.StartShooting ();
				animate.AnimateToColor (color, Palette.EnemyColor, .1f);
			}
		}
	}

	public float smoothDampTime = 0.2f;
	private void MoveToPlayer() {
		Vector3 targetPosition = PlayerController.PlayerPosition;
		targetPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		targetPosition.y = transform.position.y;
		Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
		//Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;
	}

	private void MoveToStart() {
		Vector3 targetPosition = startPosition;
		targetPosition.x = Mathf.Clamp (targetPosition.x, floor.center.x - floor.extents.x, floor.center.x + floor.extents.x);
		targetPosition.y = transform.position.y;
		Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, .5f * Time.deltaTime);
		//Vector3 newPosition = Vector3.SmoothDamp( transform.position, targetPosition, ref _smoothDampVelocity, smoothDampTime );
		// TODO(samkern): Which looks better, smoothdamp or lerp?
		transform.position = newPosition;
	}
}