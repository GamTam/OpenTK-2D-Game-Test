using NAudio.Wave;

namespace Open_TK_Tut_1;

public class SoundManager
{
    private WaveOutEvent _waveOut = new WaveOutEvent();
    private AudioFileReader _audioFile;

    public void Play(string sound)
    {
        string audioFilePath = StaticUtilities.SoundDirectory + sound + ".wav";
        AudioFileReader audioFile = new AudioFileReader(audioFilePath);
        _waveOut = new WaveOutEvent();
        _waveOut.Init(audioFile);
        _waveOut.Play();
    }
}