using NAudio.Wave;

namespace Open_TK_Tut_1;

public class MusicManager
{
    private WaveOutEvent _waveOut = new WaveOutEvent();
    private AudioFileReader _audioFile;

    private Music _songPlaying;

    public readonly Music[] Soundtrack = new[]
    {
        new Music()
        {
            Name = "Descole",
            StartPoint = 1.547445f,
            LoopPoint = 155.504833f
        },
        
        new Music()
        {
            Name = "Keera",
            StartPoint = 3.453860f,
            LoopPoint = 80.590078f
        },
        
        new Music()
        {
            Name = "Layton",
            StartPoint = 7.928472f,
            LoopPoint = 216.815329f
        }
    };

    public void Play(string name, float volume=1f)
    {
        if (_waveOut.PlaybackState == PlaybackState.Playing)
        {
            if (_songPlaying.Name.Equals(name)) return;
            
            _waveOut.Stop();
        }

        bool songInSoundtrack = false;
        foreach (Music song in Soundtrack)
        {
            if (name.Equals(song.Name))
            {
                songInSoundtrack = true;
                _songPlaying = song;
                break;
            }
        }

        if (!songInSoundtrack)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: Unable to find song \"{name}\" in soundtrack");
            Console.ForegroundColor = ConsoleColor.White;
            _songPlaying = null;
            return;
        }
        
        string audioFilePath = StaticUtilities.MusicDirectory + name + ".wav";
        _audioFile = new AudioFileReader(audioFilePath);
        _waveOut.Init(_audioFile);
        _waveOut.Volume = volume * StaticUtilities.MusicVolume;
        _waveOut.Play();
    }

    public void Stop()
    {
        _waveOut.Stop();
        _songPlaying = null;
    }

    public void Update()
    {
        if (_songPlaying == null) return;

        if (_audioFile.Position >= _audioFile.WaveFormat.AverageBytesPerSecond * _songPlaying.LoopPoint)
        {
            _audioFile.Position -= (long) ((_songPlaying.LoopPoint - _songPlaying.StartPoint) * _audioFile.WaveFormat.AverageBytesPerSecond);
            Console.WriteLine($"Looped song \"{_songPlaying.Name}\"");
        }
    }
}

public class Music
{
    public string Name;
    public float StartPoint;
    public float LoopPoint;
}