using UnityEngine;
using System.Collections.Generic;

public class AudioController : MonoBehaviour
{
    // 音效数据类，用于存储音频剪辑和名称
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public Sound[] sounds; // 在 Inspector 中设置的音效数组

    private Dictionary<string, AudioClip> soundDictionary;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        soundDictionary = new Dictionary<string, AudioClip>();

        // 初始化音效字典
        foreach (var sound in sounds)
        {
            soundDictionary[sound.name] = sound.clip;
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            audioSource.PlayOneShot(soundDictionary[soundName]);
        }
        else
        {
            Debug.LogWarning("音效名未找到: " + soundName);
        }
    }

    public void PlaySoundLoop(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            audioSource.clip = soundDictionary[soundName];
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("音效名未找到: " + soundName);
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
    }
}
