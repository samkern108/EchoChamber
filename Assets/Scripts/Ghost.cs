using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

	public GameObject projectile;
	private float playerWidth;

	private bool active = false;

	private float[] positions;
	private int positionIndex = 0;

	private float shootTelemarkTime = .5f;

	private int spriteFlipped = 1;

	private Animator animator;
	private SpriteRenderer spriteRenderer;

	public void Initialize (float[] positions) {
		this.positions = positions;

		spriteRenderer = GetComponent <SpriteRenderer> ();
		animator = GetComponent <Animator>();

		playerWidth = spriteRenderer.bounds.size.x;

		Color color = Palette.GhostColor;
		color.a = .9f;
		spriteRenderer.color = color;
	}

	public void EnactRoutine () {
		Color color = spriteRenderer.color;
		color.a -= .05f;
		spriteRenderer.color = color;

		if (color.a <= 0) {
			Destroy (this.gameObject);
		}

		animator.Play ("Appear");
		active = true;
		positionIndex = 0;
	}

	public void FixedUpdate() {
		if (positionIndex < positions.Length - 3) {
			transform.position = new Vector3 (positions [positionIndex++], positions [positionIndex++], 0);

			if (positions[positionIndex++] != spriteFlipped) {
				spriteFlipped *= -1;
				GetComponent<SpriteRenderer> ().flipX = (spriteFlipped == -1);
			}
			if (positions [positionIndex++] == 1) {
				Shoot ();
			}
		} else if(active) {
			StopRoutine ();
		}
	}

	private void TelemarkShoot() {
		animator.speed = .8f;
		animator.Play ("Shoot");
		//Invoke ("Shoot", shootTelemarkTime);
	}

	// Lot of duplicate shoot code
	private void Shoot() {
		AudioManager.PlayEnemyShoot ();
		animator.speed = 1.0f;
		float direction = spriteFlipped;
		GameObject missile = Instantiate (projectile);
		missile.transform.position = transform.position + new Vector3((playerWidth * direction), 0, 0);
		missile.GetComponent <Missile>().Initialize(Vector3.right * direction, 8.0f);
	}

	public void StopRoutine() {
		active = false;
		animator.Play ("Disappear");
	}
}
