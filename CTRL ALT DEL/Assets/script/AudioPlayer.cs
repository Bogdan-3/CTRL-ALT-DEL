using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip bcgMusic;
    public AudioSource player;
    void OnTriggerEnter(Collider other)
    {
        if (player.isPlaying)
        {
            player.Stop();
        }
        player.clip = bcgMusic;
        player.Play();
        gameObject.SetActive(false);
    }
}
