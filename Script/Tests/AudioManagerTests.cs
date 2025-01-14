using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.TestTools;

[Category("Script")]
public class AudioManagerTests
{
    private readonly string _clipNameCV = "nanako_test_clip_cv";
    private readonly string _clipNameSE = "nanako_test_clip_se";
    private readonly string _voiceName = "test_clip";
    private AudioListener _audioListener;
    private AudioManager _audioManager;

    [SetUp]
    public void Setup()
    {
        _audioListener = new GameObject().AddComponent<AudioListener>();
        var mixer = Resources.Load<AudioMixer>("TestMixer");
        _audioManager = new GameObject().AddComponent<AudioManager>();
        _audioManager.clips.Add(CreatClip(_clipNameCV));
        _audioManager.clips.Add(CreatClip(_clipNameSE));
        _audioManager.mixer = mixer;
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(_audioListener.gameObject);
    }

    [UnityTest]
    public IEnumerator PlayVoice_ShouldPlayAudioClip()
    {
        // Arrange

        // Act
        _audioManager.PlayVoice(_voiceName);

        // Assert
        yield return null;
        var any = _audioManager.GetComponents<AudioSource>().Any(x => x && x.clip.name == _clipNameCV && x.isPlaying);
        var any2 = _audioManager.GetComponents<AudioSource>().Any(x => x && x.clip?.name == _clipNameSE && x.isPlaying);
        Assert.IsTrue(any && any2);
    }

    [UnityTest]
    public IEnumerator StopVoice_ShouldStopAudioClip()
    {
        // Arrange

        _audioManager.PlayVoice(_voiceName);

        // Act
        _audioManager.StopVoice();

        // Assert
        yield return null;
        var any = _audioManager.GetComponents<AudioSource>().Any(x => x && x.clip?.name == _clipNameCV && x.isPlaying);
        var any2 = _audioManager.GetComponents<AudioSource>().Any(x => x && x.clip?.name == _clipNameSE && x.isPlaying);
        Assert.IsFalse(any && any2);
    }

    [Test]
    public void SetVoiceVolume_ShouldSetVolume()
    {
        // Arrange

        // Act
        _audioManager.SetVolume("Voice", 0.5f);

        // Assert
        _audioManager.mixer.GetFloat("Voice", out var value);
        Assert.AreEqual(_audioManager.VolumeTodB(0.5f), value);
    }

    private AudioClip CreatClip(string name)
    {
        // Define the properties of the AudioClip
        var lengthSamples = 44100; // Length of the AudioClip in samples (1 second at 44.1 kHz)
        var channels = 1; // Mono audio
        var frequency = 44100; // Sample rate in Hz
        var stream = false; // We're not streaming audio

        // Create an array to hold the raw audio data
        var data = new float[lengthSamples];

        // Generate a simple sine wave
        var frequencyHz = 440.0f; // A4 note
        for (var i = 0; i < lengthSamples; i++)
        {
            var time = i / (float)frequency;
            data[i] = Mathf.Sin(2 * Mathf.PI * frequencyHz * time);
        }

        // Create the AudioClip
        var audioClip = AudioClip.Create(name, lengthSamples, channels, frequency, stream);

        // Set the raw audio data
        audioClip.SetData(data, 0);
        return audioClip;
    }
}