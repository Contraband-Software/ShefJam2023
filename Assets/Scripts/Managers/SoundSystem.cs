using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//#pragma warning disable UNT0014

namespace Managers
{
    [
        RequireComponent(typeof(AudioListener)),
        DisallowMultipleComponent
    ]
    public class SoundSystem : Backend.AbstractSingleton<SoundSystem>
    {
        [Header("References")]
        [SerializeField] Transform sfx;
        [SerializeField] Transform music;

        readonly Dictionary<string, AudioSource> sfxDictionary = new();
        readonly Dictionary<string, AudioSource> musicDictionary = new();

        AudioSource currentMusicSource;

        protected override void SingletonAwake()
        {
            foreach (Transform child in sfx)
            {
                sfxDictionary.Add(child.name, child.GetComponent<AudioSource>());
            }
            foreach (Transform child in music)
            {
                musicDictionary.Add(child.name, child.GetComponent<AudioSource>());
            }
        }

        public AudioSource GetSoundReference(string name)
        {
            return sfxDictionary[name];
        }

        /// <summary>
        /// Plays a sound by name in the heirarchy.
        /// </summary>
        /// <param name="sound"></param>
        /// <exception cref="ArgumentException">If the sound name doesn't exist</exception>
        public void PlaySound(string sound, bool oneShot)
        {
#if UNITY_EDITOR
            if (!sfxDictionary.ContainsKey(sound))
            {
                throw new ArgumentException("Unknown/invalid sound name, ensure the GameObject containing it has the same name you have supplied to this function.");
            }
#endif
            if (oneShot)
            {
                sfxDictionary[sound].PlayOneShot(sfxDictionary[sound].clip);
            } else
            {
                sfxDictionary[sound].Play();
            }
        }
        public void PlaySound(string sound)
        {
            PlaySound(sound, false);
        }

        /// <summary>
        /// Stops the current song, if any, and plays the passed in song. This function ensures only one song is playing at a time.
        /// </summary>
        /// <param name="song"></param>
        /// <exception cref="ArgumentException">If the song name doesn't exist</exception>
        public void PlayMusic(string song)
        {
#if UNITY_EDITOR
            if (!musicDictionary.ContainsKey(song))
            {
                throw new ArgumentException("Unknown/invalid song name, ensure the GameObject containing it has the same name you have supplied to this function.");
            }
#endif
            musicDictionary[song].Play();
            currentMusicSource = musicDictionary[song];
        }

        /// <summary>
        /// Stops current music playback, if any.
        /// </summary>
        public void StopMusic()
        {
            currentMusicSource?.Stop();
        }

        /// <summary>
        /// Stops all SFX
        /// </summary>
        public void StopAllSounds()
        {
            foreach (KeyValuePair<string, AudioSource> sfx in sfxDictionary)
            {
                sfx.Value.Stop();
            }
        }
    }
}