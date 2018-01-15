using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAIStats {

	public GhostAI self;

	public float Aggressiveness() {
		return totalGhostsInLevel == 0 ? 0 : ((float)ghostsKilled / (float)totalGhostsInLevel);
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

public class GhostAI : MonoBehaviour, IPlayerObserver {

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
		stats.self = this;
		NotificationMaster.playerObservers.Add (this);

		float aggro = stats.Aggressiveness ();

		if (aggro > 0) {
			attack = gameObject.AddComponent <GhostAttack>();
			GetComponent<GhostAttack> ().Initialize (stats);
		}

		movement = gameObject.AddComponent <GhostMovement_Fly>();
		movement.Initialize (stats);

		detectionRadius = Room.bounds.extents.x;

		transform.localScale = new Vector3(stats.size, stats.size, 1);

		size = GetComponent <SpriteRenderer>().bounds.extents;

		transform.position = movement.GetSpawnPosition(size);
		movement.startPosition = transform.position;

		layerMask = 1 << LayerMask.NameToLayer ("Wall") | 1 << LayerMask.NameToLayer("Player");

		GetComponent<SpriteRenderer>().sortingOrder = (int)(1.0f/size.magnitude * 1000);

		AdjustColors(aggro);

		animate = GetComponent <Animate>();
		animate.AnimateToColor (Palette.Invisible, color, .3f);
	}

	void Awake() {
		GetComponent <SpriteRenderer> ().color = Palette.Invisible;
	}

	RaycastHit2D hit;
	void Update () {
		hit = Physics2D.Linecast (transform.position, PlayerController.PlayerPosition, layerMask);
		if (hit.collider.tag == "Wall" || (hit.collider.tag == "Player" && hit.distance > detectionRadius)) {
			if (detectedPlayer) {
				detectedPlayer = false;
				if(attack) attack.StopShooting ();
				animate.AnimateToColor (Palette.EnemyColor, color, .3f);
			}
			return;
		} else {
			if (!detectedPlayer) {
				detectedPlayer = true;
				if(attack) attack.StartShooting ();
				animate.AnimateToColor (color, Palette.EnemyColor, .1f);
			}
		}
	}

	public void AdjustColors(float aggro) {
		if (aggro == 0.0f) {
			color = Color.white;
		} else {
			color = Color.black;
			color.r = .5f + aggro;
			color.g = 1 - aggro;
		}

		Color endColor = new Color ((float)(148.0f/255.0f), (float)(118.0f/255.0f), (float)(91.0f/255.0f));

		color.r -= color.r/4.0f;
		color.g -= color.g/4.0f;

		ParticleSystem ps = GetComponentInChildren <ParticleSystem>();
		var psColor = ps.colorOverLifetime;

		Gradient gradient = new Gradient ();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(endColor, .65f) },
			new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.15f), new GradientAlphaKey(0.0f, 1.0f) }
		);
			
		psColor.color = new ParticleSystem.MinMaxGradient(gradient);

		var main = ps.main;
		main.startSizeMultiplier = size.magnitude + .6f;
		Debug.Log (size.magnitude);
	}

	public void PlayerDied() {
		this.enabled = false;
	}
}