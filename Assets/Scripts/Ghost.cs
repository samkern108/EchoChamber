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
	private Animate animate;
	private SpriteRenderer spriteRenderer;

	private Color currentColor;

	public void Initialize (float[] positions) {
		this.positions = positions;

		spriteRenderer = GetComponent <SpriteRenderer> ();
		animator = GetComponent <Animator>();
		animate = GetComponent <Animate>();

		playerWidth = spriteRenderer.bounds.size.x;

		currentColor = Palette.GhostColor;
		currentColor.a = .9f;
		spriteRenderer.color = currentColor;
	}

	public void EnactRoutine () {
		currentColor.a -= .05f;

		if (currentColor.a <= 0) {
			Destroy (this.gameObject);
		}

		animate.AnimateToColor (Palette.invisible,currentColor,.3f);
		active = true;
		positionIndex = 0;
	}

	public void FixedUpdate() {
		if(((positionIndex + (4 * 30) - 1) < positions.Length) && (positions[positionIndex + (4 * 30) - 1] == 1)) {
			TelemarkShoot ();
		}

		// (x | y | flip | shoot)
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
		animate.AnimateToColor (currentColor,Color.red,.3f);
	}

	// Lot of duplicate shoot code
	private void Shoot() {
		animate.AnimateToColor (Color.red,currentColor,.3f);
		AudioManager.PlayEnemyShoot ();
		float direction = spriteFlipped;
		GameObject missile = Instantiate (projectile);
		missile.transform.position = transform.position + new Vector3((playerWidth * direction), 0, 0);
		missile.GetComponent <Missile>().Initialize(Vector3.right * direction, 8.0f);
	}

	public void StopRoutine() {
		active = false;
		animate.AnimateToColor (currentColor,Palette.invisible,.3f);
	}
}
