using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Required] [SerializeField] public AudioMixer mixer;

    [Required] [SerializeField] [AssetList(Path = "/Audio/", AutoPopulate = true)] [ValidateInput("AudioClipValidateInput")]
    public List<AudioClip> clips = new();

    private readonly string _prefix = "nanako";

    private readonly Dictionary<string, AudioSource> audios = new();

    private void Start()
    {
        var findMatchingGroups = mixer.FindMatchingGroups("");
        var audioMixerGroups = findMatchingGroups.Skip(1);
        foreach (var audioMixerGroup in audioMixerGroups)
        {
            var addComponent = gameObject.AddComponent<AudioSource>();
            addComponent.outputAudioMixerGroup = audioMixerGroup;
            audios.Add(audioMixerGroup.name, addComponent);
        }
    }

    private bool AudioClipValidateInput(List<AudioClip> clips)
    {
        return clips?.Any() ?? false;
    }

    public List<string> GetExposed()
    {
        //Exposed Params
        var parameters = (Array)mixer.GetType().GetProperty("exposedParameters").GetValue(mixer, null);

        return parameters.Cast<object>()
            .Select((t, i) => parameters.GetValue(i))
            .Select(o => (string)o.GetType().GetField("name").GetValue(o))
            .ToList();
    }

    public float VolumeTodB(float value)
    {
        if (value == 0)
        {
            value = float.MinValue;
        }

        return Mathf.Log10(value) * 20;
    }

    public void SetVolume(string name, float value)
    {
        var volumeTodB = VolumeTodB(value);
        mixer.SetFloat(name, volumeTodB);
    }


    public void PlayVoice(string animationName, bool overwrite = false)
    {
        PlayAnimationCV(animationName, overwrite);
        PlayAnimationSE(animationName, overwrite);
    }

    [Button]
    public void PlayShortSE(string voiceName)
    {
        if (string.IsNullOrEmpty(voiceName))
        {
            return;
        }

        var audioClip = clips.Find(x => x.name == voiceName);

        if (audios.TryGetValue("SE", out var source))
        {
            source.PlayOneShot(audioClip);
        }
    }

    public void StopVoice()
    {
        StopCV();
        StopSE();
    }

    public async void PlayBGM()
    {
        await StopBGM();
        var voiceName = "BGM";
        var audioClip = clips.Find(x => x.name == voiceName);
        if (audios.TryGetValue("BGM", out var source) && !source.isPlaying)
        {
            source.volume = 1;
            source.loop = true;
            source.clip = audioClip;
            source.Play();
        }
    }

    private async UniTask StopBGM()
    {
        if (audios.TryGetValue("BGM", out var source) && source.isPlaying)
        {
            await DOTween.To(() => source.volume, x => source.volume = x, 0, 1f);
            source.Stop();
        }
    }

    private void PlayAnimationCV(string animationName, bool overwrite)
    {
        var voiceName = $"{_prefix}_{animationName}_cv";
        var audioClip = clips.Find(x => x.name == voiceName);
        audios.TryGetValue("Voice", out var source0);
        var source0Clip = source0.clip;
        if (audios.TryGetValue("Voice", out var source) && (source.clip?.name != voiceName || !source.isPlaying || overwrite))
        {
            source.loop = true;
            source.clip = audioClip;
            source.Play();
        }
    }

    private void PlayAnimationSE(string animationName, bool overwrite)
    {
        var voiceName = $"{_prefix}_{animationName}_se";
        var audioClip = clips.Find(x => x.name == voiceName);

        if (audios.TryGetValue("SE", out var source) && (source.clip?.name != voiceName || !source.isPlaying || overwrite))
        {
            source.loop = true;
            source.clip = audioClip;
            source.Play();
        }
    }

    private void StopCV()
    {
        if (audios.TryGetValue("Voice", out var source))
        {
            source.Stop();
        }
    }

    private void StopSE()
    {
        if (audios.TryGetValue("SE", out var source))
        {
            source.Stop();
        }
    }
}