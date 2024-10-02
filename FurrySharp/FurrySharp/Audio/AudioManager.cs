using System;
using FurrySharp.Registry;
using FurrySharp.Resources;
using Microsoft.Xna.Framework.Audio;

namespace FurrySharp.Audio;

public static class AudioManager
{
    private static float masterVolume = 1f;
    private static float currentVolume = 1f;
    private static float ambienceVolume = 1f;

    public static string CurrentSong { get; private set; }

    public static bool IsPlayingSong => CurrentSong != string.Empty;

    private static SongPlayer BGM = new();
    private static SongPlayer Ambience = new();

    static AudioManager()
    {
        CurrentSong = string.Empty;
    }

    public static bool PlaySong(string name, float volume = 1f)
    {
        if (CurrentSong == name)
        {
            SetSongVolume(volume);
            return false;
        }

        string song = ResourceManager.GetMusicPath(name);
        if (song != null)
        {
            CurrentSong = name;
            SetSongVolume(volume);
            BGM.Play(song);
            return true;
        }
        else
        {
            StopSong();
            return false;
        }
    }

    public static void SetMasterVolume(float volume)
    {
        masterVolume = Math.Clamp(volume, 0, 1);
        SetSongVolume(currentVolume);
    }

    public static float GetMasterVolume() => masterVolume;

    public static void SetSongVolume(float volume)
    {
        currentVolume = Math.Clamp(volume, 0, 1);
        BGM.SetVolume(masterVolume * currentVolume * GlobalState.Settings.MusicVolumeScale);
        SetAmbienceVolume(ambienceVolume);
    }

    public static void SetAmbienceVolume(float volume)
    {
        ambienceVolume = Math.Clamp(volume, 0, 1);
        Ambience.SetVolume(masterVolume * currentVolume * ambienceVolume * GlobalState.Settings.MusicVolumeScale);
    }

    public static float GetVolume() => currentVolume;

    public static void SetSongVolume() => SetSongVolume(1f);

    public static bool StopSong()
    {
        if (IsPlayingSong)
        {
            BGM.Stop();
            CurrentSong = string.Empty;
            return true;
        }

        return false;
    }

    public static void PlayAmbience(string name, float volume = 1f)
    {
        string sound = ResourceManager.GetAmbiencePath(name);
        SetAmbienceVolume(volume);
        if (sound != null)
        {
            Ambience.Play(sound);
        }
        else
        {
            Ambience.Stop();
        }
    }

    // public static SoundEffectInstance PlaySoundEffect(params string[] names)
    // {
    //     var sfx = names.Select((n) => ResourceManager.GetSFX(n)).Where((n) => n != null).ToArray();
    //     if (sfx.Length == 0) return null;
    //
    //     return CreateSoundInstance(sfx[GlobalState.RNG.Next(0, sfx.Length)]);
    // }

    // public static void PlayPitchedSoundEffect(string name, float pitch, float volume = 1)
    // {
    //     CreateSoundInstance(ResourceManager.GetSFX(name), volume, pitch);
    // }

    private static SoundEffectInstance CreateSoundInstance(SoundEffectInstance sfx, float volume = 1, float pitch = 0)
    {
        if (sfx != null)
        {
            sfx.Pitch = pitch;
            sfx.Volume = volume * GlobalState.Settings.SFXVolumeScale;
            sfx.Play();
        }

        return sfx;
    }
}