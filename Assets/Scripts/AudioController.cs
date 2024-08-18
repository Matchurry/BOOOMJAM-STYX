using UnityEngine;
using System.Collections.Generic;

public class AudioController : MonoBehaviour
{
    // ��Ч�����࣬���ڴ洢��Ƶ����������
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public Sound[] sounds; // �� Inspector �����õ���Ч����

    private Dictionary<string, AudioClip> soundDictionary;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        soundDictionary = new Dictionary<string, AudioClip>();

        // ��ʼ����Ч�ֵ�
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
            Debug.LogWarning("��Ч��δ�ҵ�: " + soundName);
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
            Debug.LogWarning("��Ч��δ�ҵ�: " + soundName);
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
    }
}
