using System;
using System.Collections.Generic;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>
    /// Plays audio cues. A cue without an assigned clip plays a generated placeholder beep,
    /// so real sound files can be dropped in later without code changes.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [Serializable]
        public struct CueClip
        {
            public AudioCue cue;
            public AudioClip clip;
        }

        [Tooltip("Real sound per cue. Leave a cue out to use its placeholder beep.")]
        [SerializeField] List<CueClip> clips = new List<CueClip>();

        AudioSource source;
        readonly Dictionary<AudioCue, AudioClip> lookup = new Dictionary<AudioCue, AudioClip>();

        void Awake()
        {
            source = GetComponent<AudioSource>();
            foreach (var entry in clips)
                if (entry.clip != null) lookup[entry.cue] = entry.clip;
        }

        public void Play(AudioCue cue)
        {
            if (!lookup.TryGetValue(cue, out var clip))
            {
                clip = CreateBeep(cue);
                lookup[cue] = clip;
            }
            source.PlayOneShot(clip);
        }

        // Short sine beep with a fade-out; each cue gets its own pitch.
        static AudioClip CreateBeep(AudioCue cue)
        {
            const int sampleRate = 44100;
            const float duration = 0.15f;
            float frequency = 880f * Mathf.Pow(1.25f, (int)cue);
            int length = Mathf.RoundToInt(sampleRate * duration);
            var samples = new float[length];
            for (int i = 0; i < length; i++)
            {
                float fade = 1f - (float)i / length;
                samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * i / sampleRate) * 0.3f * fade;
            }
            var clip = AudioClip.Create("Placeholder " + cue, length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
