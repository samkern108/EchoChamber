using UnityEngine;
using System.Collections;
using System.IO;

public class ResourceLoader : MonoBehaviour {

	private static string pathToPrefabs = "Prefabs/";
	private static string pathToParticles = "Prefabs/Particles/";
	private static string pathToAudio = "Audio/";

	public static GameObject LoadPrefab(string name)
	{
		return Resources.Load <GameObject>(pathToPrefabs + name);
	}

	public static GameObject LoadParticle(string name)
	{
		return Resources.Load <GameObject>(pathToParticles + name);
	}

	public static AudioClip LoadAudioClip(string name)
	{
		return Resources.Load <AudioClip>(pathToAudio + name);
	}
}
