using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAIStats {

	public float Aggressiveness() {
		Debug.Log ((ghostsKilled + 1) * (1 / (totalGhostsInLevel + 1)));
		return (ghostsKilled + 1) * (1 / (totalGhostsInLevel + 1));
	}
		
	public int shotsFired;
	public int ghostsKilled;
	public int totalGhostsInLevel;

	public float timeOpen;

	public float size;

	public float totalGhostAggressiveness;
	public float killedGhostAggressiveness;

	// speed can be based on time spent in the level, or perhaps health?
	// dexterity based on shots dodged?
	// mobility can be based on total ground covered? or total time spent moving?
	// movement should be a factor of average speed, number of jumps, percentage of the map covered, starting/ending position, etc.
}

public class GhostAI : MonoBehaviour {

	public GhostAIStats stats;

	private GhostAttack attack;
	private GhostMovement movement;

	private int layerMask;

	private bool detectedPlayer = false;
	private float detectionRadius = 4.0f;

	private Animate animate;
	private Vector3 size;

	private Color color;

	public void Initialize(GhostAIStats stats) {
		this.stats = stats;

		if (stats.shotsFired > 0) {
			attack = gameObject.AddComponent <GhostAttack>();
	//		GetComponent<GhostAttack> ().Initialize (stats);
		}

		movement = gameObject.AddComponent <GhostMovement>();

		detectionRadius = Room.bounds.extents.x;

		transform.localScale = new Vector3(stats.size, stats.size, 1);

		size = GetComponent <SpriteRenderer>().bounds.extents;

		transform.position = GetSpawnPosition();
		movement.startPosition = transform.position;

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

	void Update () {
		RaycastHit2D hit = Physics2D.Linecast (transform.position, PlayerController.PlayerPosition, layerMask);
		if (hit.collider.tag == "Wall" || (hit.collider.tag == "Player" && hit.distance > detectionRadius)) {
			if (detectedPlayer) {
				detectedPlayer = false;
				movement.SetMovementState (GhostMovementState.Idle);
				if(attack) attack.StopShooting ();
				animate.AnimateToColor (Palette.EnemyColor, color, .3f);
			}
			return;
		} else {
			if (!detectedPlayer) {
				detectedPlayer = true;
				movement.SetMovementState (GhostMovementState.Attack);
				if(attack) attack.StartShooting ();
				animate.AnimateToColor (color, Palette.EnemyColor, .1f);
			}
		}
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
				movement.floor = hit.collider.bounds;
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