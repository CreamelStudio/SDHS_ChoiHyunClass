using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public struct BGMData
{
    public AudioSource source;
    public float volume;
}

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioSource defaultAudioSource;
    public Dictionary<AudioClip, BGMData> bgmDataList = new Dictionary<AudioClip, BGMData>();

    public void PlayOneShot(AudioClip clip, float? volume = 1f)
    {
        defaultAudioSource.PlayOneShot(clip, volume ?? 1f);
    }

    public void PlayBGM(AudioClip clip, float volume)
    {
        AudioSource source = this.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.volume = volume;
        source.Play();

        BGMData data = new BGMData();
        data.source = source;
        data.volume = volume;
        bgmDataList[clip] = data;
    }

    public void StopBGM(AudioClip clip)
    {
        bgmDataList.TryGetValue(clip, out BGMData data);
        if (data.source != null)
        {
            data.source.Stop();
            Destroy(data.source);
            bgmDataList.Remove(clip);
        }
    }
}
