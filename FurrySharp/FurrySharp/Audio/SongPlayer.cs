extern alias TheCoolerVorbis;
using System;
using Microsoft.Xna.Framework.Audio;
using TheCoolerVorbis::NVorbis; // This makes reader.IsEndOfStream, reader.Tags, etc. possible.

namespace FurrySharp.Audio;

public class SongPlayer
{
    const int BufferMs = 128;

    private DynamicSoundEffectInstance player;
    private VorbisReader reader;

    // Vorb Samples
    float[] vorbs = new float[SoundEffect.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(BufferMs), 44100, AudioChannels.Mono)];

    public SongPlayer()
    {
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        float volume = player?.Volume ?? 1f;
        player?.Stop();
        player?.Dispose();
        player = new(44100, AudioChannels.Mono);
        player.Volume = volume;
        player.BufferNeeded += BufferNeeded;
    }

    private void BufferNeeded(object sender, EventArgs e)
    {
        if (reader.IsEndOfStream)
        {
            _ = int.TryParse(reader.Tags.GetTagSingle("LOOPSTART"), out int startLoop);
            reader.SeekTo(startLoop);
        }

        if (!reader.IsEndOfStream)
        {
            int total = reader.ReadSamples(vorbs, 0, vorbs.Length);

            byte[] res = new byte[total * 2];
            for (int i = 0; i < total; ++i)
            {
                int tmp = (int)(short.MaxValue * vorbs[i]);
                Math.Clamp(tmp, short.MinValue, short.MaxValue);
                short val = (short)tmp;
                res[i * 2] = (byte)(val & 0xFF);
                res[i * 2 + 1] = (byte)((val >> 8) & 0xFF);
            }

            player.SubmitBuffer(res);
        }
    }

    public float GetVolume()
    {
        return player.Volume;
    }

    public void Play(string song)
    {
        ResetPlayer();
        if (reader != null) reader.Dispose();
        reader = new VorbisReader(song);
        BufferNeeded(null, null);
        if (player.State != SoundState.Playing)
        {
            player.Play();
        }
    }

    public void Stop() => player.Stop();

    public void SetVolume(float v) => player.Volume = v;
}