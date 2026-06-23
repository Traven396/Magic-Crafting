using UnityEngine;

[CreateAssetMenu(fileName = "Sound Manager", menuName = "Managers/Sound Manager")]
public class SoundManagerSO : ScriptableObject
{
    private static SoundManagerSO instance;
    public static SoundManagerSO Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<SoundManagerSO>("Sound Manager");
            return instance;
        }
    }

    [SerializeField] AudioSource AudioSourcePrefab;

    public static void PlayClipAtPoint(AudioClip clip, Vector3 worldPos, float volume, float pitchChangeModifier = 0, float volumeChangeModifier = 0)
    {
        float randVolume = Random.Range(volume - volumeChangeModifier, volume + volumeChangeModifier);
        float randPitch = Random.Range(1 - pitchChangeModifier, 1 + pitchChangeModifier);
        //Spawn object
        AudioSource audioSource = Instantiate(Instance.AudioSourcePrefab, worldPos, Quaternion.identity);

        //Give it the clip
        audioSource.clip = clip;

        //volume
        audioSource.volume = randVolume;
        audioSource.pitch = randPitch;

        audioSource.Play();

        //Find clip length
        float clipLength = clip.length;

        //Destroy after length
        Destroy(audioSource.gameObject, clipLength);
    }

    public static void PlayRandomClipAtPoint(AudioClip[] clips, Vector3 worldPos, float volume, float pitchChangeModifier = 0.1f, float volumeChangeModifier = 0)
    {
        float randVolume = Random.Range(volume - volumeChangeModifier, volume + volumeChangeModifier);
        float randPitch = Random.Range(1 - pitchChangeModifier, 1 + pitchChangeModifier);

        //Choose random clip
        var chosenClip = clips[Random.Range(0, clips.Length)];

        //Spawn object
        AudioSource audioSource = Instantiate(Instance.AudioSourcePrefab, worldPos, Quaternion.identity);

        //Give it the clip
        audioSource.clip = chosenClip;

        //volume
        audioSource.volume = randVolume;
        audioSource.pitch = randPitch;

        audioSource.Play();

        //Find clip length
        float clipLength = chosenClip.length;

        //Destroy after length
        Destroy(audioSource.gameObject, clipLength);

    }
}
