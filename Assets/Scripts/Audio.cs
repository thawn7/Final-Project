using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    public AudioClip clickClip;   // assign your single pop sound here
    [Range(0f, 1f)] public float volume = 1f;

    AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D
    }

    public void PlayClick()
    {
        if (clickClip != null) src.PlayOneShot(clickClip, volume);
    }
}
