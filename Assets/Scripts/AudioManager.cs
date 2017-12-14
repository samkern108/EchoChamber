using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MovementEffects;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;

	public static AudioClip projectileShoot, projectileExplode, 
	playerBoostCharge, playerBoostRelease, playerWallHit, playerTurn, playerMove, playerDeath,
	levelComplete, gameStart, 
	dotPickup;

	private static GameObject sourceTemplate;
	private static int activeSources = 0;
	private static List<AudioSource> freeSources = new List<AudioSource>();

	public static AudioSource dotAS, playerAS, playerMoveAS;

	// ## Initialization >> //

	// Member initialization
	public void Awake() {
		instance = this;

		sourceTemplate = new GameObject("SourceTemplate");
		sourceTemplate.AddComponent <AudioSource>();
		sourceTemplate.transform.SetParent (this.transform);

		dotAS = transform.Find ("DotAS").GetComponent<AudioSource>();
		playerAS = transform.Find ("PlayerAS").GetComponent<AudioSource>();
		//playerMoveAS = transform.FindChild ("PlayerMoveAS").GetComponent<AudioSource>();

		Initialize ();
	}

	// Static initialization
	public static void Initialize () {
		projectileShoot = ResourceLoader.LoadAudioClip ("Shoot");
		projectileExplode = ResourceLoader.LoadAudioClip ("Projectile Hit Wall");
		playerBoostCharge = ResourceLoader.LoadAudioClip ("Player Boost Charge");
		playerBoostRelease = ResourceLoader.LoadAudioClip ("Player Boost 6");
		playerWallHit = ResourceLoader.LoadAudioClip ("Player Wall Hit");
		//playerMove = ResourceLoader.LoadAudioClip ("Player Move");
		playerTurn = ResourceLoader.LoadAudioClip ("Player Turn");
		playerDeath = ResourceLoader.LoadAudioClip ("Player Death");
		levelComplete = ResourceLoader.LoadAudioClip ("Level Complete");
		gameStart = ResourceLoader.LoadAudioClip ("Game Start");
		dotPickup = ResourceLoader.LoadAudioClip ("Dot Pickup");
	}

	// << Initialization ## //

	// ## Audio Source Handling >> //

	private static AudioSource GetFreeAudioSource(float recycleTime) {
		/*if (freeSources.Count == 0) {
			GameObject obj = Instantiate (sourceTemplate);
			obj.transform.SetParent (AudioManager.instance.transform);
			activeSources++;
			return obj.GetComponent <AudioSource>();
		}
		else {
			// Race condition?
			AudioSource source = freeSources[0];
			freeSources.RemoveAt (0);
			// Figure out a better way to determine if sources can be removed.
			if (freeSources.Count >= (int)Mathf.Ceil (activeSources / 2)) {
				freeSources.RemoveRange (0, (int)Mathf.Floor (activeSources / 2));
			}
			Timing.RunCoroutine (RecycleSource(recycleTime + .1f, source));
			return source;
		}*/
		GameObject obj = Instantiate (sourceTemplate);
		obj.transform.SetParent (AudioManager.instance.transform);
		return obj.GetComponent <AudioSource>();
	}

	private static IEnumerator<float> RecycleSource (float wait, AudioSource source) {
		yield return wait;
		source.pitch = 1.0f;
		freeSources.Add (source);
	}

	// << Audio Source Handling ## //

	// ## Dot SFX >> //
	private static bool resetPickup = true;
	public void N_LevelLoaded(int level) {
		resetPickup = true;
	}

	public void PlayDotPickup() {
		if (resetPickup) {
			resetPickup = false;
			dotAS.pitch = .55f;
		}

		dotAS.volume = .7f;
		dotAS.PlayOneShot (dotPickup);
		dotAS.pitch += .26f;
	}

	// << Dot SFX ## //

	// ## Player SFX >> //

	public static void PlayPlayerDeath() {
		playerAS.volume = .9f;
		playerAS.pitch = Random.Range (.8f, 1.1f);
		playerAS.PlayOneShot (playerDeath);
	}

	private static bool playerTurnSafeToPlay = true;
	private void PlayerTurnSafeToPlay() {
		playerTurnSafeToPlay = true;
	}

	public static void PlayPlayerTurn() {
		if (!playerTurnSafeToPlay) return;
		playerTurnSafeToPlay = false;
		playerAS.volume = Random.Range(.2f, .4f);
		playerAS.pitch = Random.Range(.6f, .8f);
		playerAS.PlayOneShot (playerTurn);
		AudioManager.instance.Invoke ("PlayerTurnSafeToPlay", playerTurn.length/3);
	}

	public static void PlayPlayerBoostCharge() {
		playerAS.PlayOneShot (playerBoostCharge);
	}

	public static void PlayPlayerBoostRelease(float strength, float volume) {
		playerAS.PlayOneShot (playerBoostRelease);
		playerAS.volume = volume;
		playerAS.pitch = strength;
	}

	private static bool wallHitSafeToPlay = true;
	private void WallHitSafeToPlay() {
		wallHitSafeToPlay = true;
	}

	public static void PlayPlayerWallHit() {
		if (!wallHitSafeToPlay) return;

		playerAS.volume = .9f;
		playerAS.pitch = Random.Range (.8f, 1.2f);
		playerAS.PlayOneShot (playerTurn);
		wallHitSafeToPlay = false;
		AudioManager.instance.Invoke ("WallHitSafeToPlay", .2f);
	}

	/*private static bool playPlayerMove = false;

	public void PlayPlayerMove(bool play) {
		playPlayerMove = play;
	}

	private void RepeatPlayerMove() {
		if(playPlayerMove)
			playerMoveAS.PlayOneShot (playerMove);
	}*/

	// << Player SFX ## //

	// ## Enemy SFX >> //

	public static void PlayEnemyShoot() {
		AudioSource source = GetFreeAudioSource(projectileShoot.length);
		source.volume = Random.Range (.4f, .8f);
		source.pitch = Random.Range (1.2f, 1.6f);
		source.PlayOneShot (projectileShoot);
	}

	public static void PlayShoot() {
		AudioSource source = GetFreeAudioSource(projectileShoot.length);
		source.volume = Random.Range (.4f, .8f);
		source.pitch = Random.Range (.8f, 1.2f);
		source.PlayOneShot (projectileShoot);
	}

	public static void PlayProjectileExplode() {
		AudioSource source = GetFreeAudioSource(projectileExplode.length);
		source.volume = Random.Range (.2f, .4f);
		source.pitch = Random.Range (1.0f, 1.8f);
		source.PlayOneShot (projectileExplode);
	}

	// << Enemy SFX ## //
}
