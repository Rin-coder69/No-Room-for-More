using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicTracks;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        if (musicTracks.Length == 0)
        {
            Debug.LogWarning("No music tracks assigned!");
            return;
        }

        // Pick random track
        int randomIndex = Random.Range(0, musicTracks.Length);
        AudioClip selectedTrack = musicTracks[randomIndex];

        // Play it
        audioSource.clip = selectedTrack;
        audioSource.loop = true; // Loop the music
        audioSource.volume = 0.01f;
        audioSource.Play();

        Debug.Log($"Playing: {selectedTrack.name}");
    }
}