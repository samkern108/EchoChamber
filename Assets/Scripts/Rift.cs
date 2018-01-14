using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class Rift : MonoBehaviour, IRestartObserver, IGhostDeathObserver, IPlayerShootObserver {

	GhostAIStats ghostStats;
	ParticleSystem ps;
	BoxCollider2D boxCollider;

	// TODO(samkern): Should rifts get bigger over time? Spawn little enemies? How should we close them?
	// TODO(samkern): Idea: maybe if shit's really hitting the fan, a checkpoint has a random chance of spawning a friendly ghost? Or like, if you catch it really early it becomes friendly?

	private bool activated = false;

	private float timeOpened;

	private AnimationCurve psSizeCurve;

	public void Start() {
		timeOpened = Time.time;

		ghostStats = new GhostAIStats ();

		ps = GetComponent <ParticleSystem>();
		boxCollider = GetComponent <BoxCollider2D>();

		NotificationMaster.restartObservers.Add (this);
		NotificationMaster.ghostDeathObservers.Add (this);
		NotificationMaster.playerShootObservers.Add (this);

		psSizeCurve = new AnimationCurve();
		psSizeCurve.AddKey(0.0f, 0.0f);
		psSizeCurve.AddKey(1.0f, 1.0f);

		purpleColor = new Color (.4f, 0.0f, 1.0f);
		gradient = new Gradient ();
		alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) };

		Vector3 point;
		float distance, minDistance = 2.0f;
		RaycastHit2D hit;
		do {
			point = Room.GetRandomPointInRoom ();
			hit = Physics2D.Raycast (point, Vector3.back, 5.0f, 1 << LayerMask.NameToLayer ("Wall"));
			if(hit.collider)
				continue;
			distance = Vector2.Distance (PlayerController.PlayerPosition, point);
		} while (distance < minDistance);

		transform.position = point;

		StartCoroutine ("C_AnimateSize");
	}
		
	private float psRadius = 0.2f;
	private float endSize = 0.2f;

	private float blueValue = 1.0f;
	private float greenValue = 1.0f;
	private float redValue = 0.0f;

	private float speed = .05f;

	private Color purpleColor;
	private Color endColor;

	private Gradient gradient;
	private GradientAlphaKey[] alphaKeys;

	// sin animation? :D
	// https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
	private IEnumerator C_AnimateSize () {
		Vector3 newSize;
		float delay;
		while(true) {
			var shape = ps.shape;

			psRadius += .01f * speed;
			endSize += .1f * speed;

			shape.radius = psRadius;

			var size = ps.sizeOverLifetime;
			boxCollider.size = new Vector2 (psRadius, psRadius);

			var psColor = ps.colorOverLifetime;

			redValue += .05f * speed;
			greenValue -= .05f * speed;
			blueValue -= .02f * speed;

			endColor = new Color(redValue, greenValue, blueValue);
			gradient.SetKeys(
				new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(endColor, .8f) },
				alphaKeys
			);

			psColor.color = new ParticleSystem.MinMaxGradient(gradient);
			size.size = new ParticleSystem.MinMaxCurve(endSize, psSizeCurve);

			delay = .1f;
			yield return new WaitForSeconds(delay);
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (!activated) {
			ghostStats.timeOpen = Time.time - timeOpened;

			// For reference, the player is .14 scale
			// TODO(samkern): Figure out an appropriate scaling measure between rift & ghost
			ghostStats.size = psRadius;

			// TODO(samkern): Should we record ALL ghosts killed while the rift is active, or just some? Should they decay over time?
			// ghosts alive during this rift's time in the level = ghosts currently alive plus ghosts murdered.
			ghostStats.totalGhostsInLevel = (GhostManager.instance.children.Count + ghostStats.ghostsKilled);
			ghostStats.totalGhostAggressiveness = GhostManager.instance.TotalGhostAggressiveness ();

			GhostManager.instance.SpawnGhost (ghostStats);

			NotificationMaster.SendCheckpointReachedNotification (Time.time - timeOpened);
			AudioManager.PlayDotPickup ();
			Destroy (this.gameObject);
		}
	}

	public void Restart() {
		Destroy (this.gameObject);
	}

	public void GhostDied(GhostAIStats stats) {
		ghostStats.ghostsKilled++;
		ghostStats.killedGhostAggressiveness += stats.Aggressiveness ();
	}

	public void PlayerShoot() {
		ghostStats.shotsFired++;
	}
}
