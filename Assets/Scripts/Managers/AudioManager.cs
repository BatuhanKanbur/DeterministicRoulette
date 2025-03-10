using UnityEngine;

public class AudioManager : MonoBehaviour,IManager
{
    [SerializeField] private AudioSource audioSource;
    public async void PlaySound(string soundName)
    {
        var audioClip =await AssetManager<AudioClip>.LoadObject(soundName);
        audioSource.PlayOneShot(audioClip);
    }
    public void StopSound()
    {
        audioSource.Stop();
    }
}
