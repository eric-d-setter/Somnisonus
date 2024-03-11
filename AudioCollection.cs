using NAudio.Wave;

namespace Somnisonus
{
    
    internal class AudioCollection
    {
        private String name { get; }
        private List<AudioSegment> segments { get; }

        public AudioCollection(ParsedAudioCollection jsonInput) 
        {
            if (jsonInput == null)
            {
                throw new ArgumentNullException("Json Audio Collection is null");
            }
            name = jsonInput.Collection_name;
            segments = new List<AudioSegment>();
            foreach (var jsonSegments in jsonInput.Collection_data)
            {
                segments.Add(new AudioSegment(jsonSegments));
            }
        }
    }
    internal class AudioSegment 
    {
        private static String LOOP = "LOOP";
        private AudioSegmentType type { get; }
        private int order { get; }
        private List<AudioSounds> sounds { get; }
        public AudioSegment(ParsedAudioSegment jsonInput) 
        {
            if (jsonInput == null)
            {
                throw new ArgumentNullException("Json Audio Segment is null");
            }
            type = jsonInput.Type.Equals(LOOP) ?  AudioSegmentType.LOOP : AudioSegmentType.NONLOOP;
            sounds = new List<AudioSounds>();
            order = jsonInput.Order;
            foreach (var jsonSounds in jsonInput.Sounds)
            {
                sounds.Add(new AudioSounds(jsonSounds));
            }

        }
    }
    internal enum AudioSegmentType
    {
        LOOP,
        NONLOOP
    }
    internal class AudioSounds 
    {
        public String Path { get; }
        public int CutoffStart { get; }
        public int CutoffEnd { get; }

        public AudioSounds(ParsedAudioSounds jsonInput) 
        {
            if (jsonInput == null)
            {
                throw new ArgumentNullException("Json Audio Sounds is null");
            }
            AudioFileReader audioFile = new AudioFileReader(jsonInput.Path); 
            // Use the WavFileUtil class to create new wave files at location
            Path = jsonInput.Path;
            CutoffStart = jsonInput.Cutoff_start;
            CutoffEnd = jsonInput.Cutoff_end;
        }
    }
}
