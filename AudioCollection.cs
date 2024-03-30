using System.IO;
using NAudio.Wave;
using System.Text.RegularExpressions;

namespace Somnisonus
{
    
    internal class AudioCollection
    {
        public String name { get; }
        public List<AudioSegment> segments { get; }
        public List<AudioCollection> playNextOptions { get; }

        public AudioCollection(ParsedAudioCollection jsonInput) 
        {
            playNextOptions = new List<AudioCollection>();
            try
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
                segments = segments.OrderBy(o => o.order).ToList();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File in collection: " + name + "@ " + ex.Message + " was not found. Please attempt recreating the collection.");
            }
        }
    }
    internal class AudioSegment 
    {
        private static String LOOP = "LOOP";
        public int order { get; }
        public MyWaveProvider sounds { get; }
        public AudioSegment(ParsedAudioSegment jsonInput) 
        {
            if (jsonInput == null)
            {
                throw new ArgumentNullException("Json Audio Segment is null");
            }
            List<AudioSounds> audiosounds = new List<AudioSounds>();
            List<AudioFileReader> sources = new List<AudioFileReader>();
            order = jsonInput.Order;

            // Get data and sort
            foreach (var jsonSounds in jsonInput.Sounds)
            {
                audiosounds.Add(new AudioSounds(jsonSounds));
            }
            audiosounds = audiosounds.OrderBy(o => o.order).ToList();
            // Turn data into wave provider
            foreach (var sound in audiosounds)
            {
                sources.Add(sound.audioFile);
            }
           
            sounds = jsonInput.Type.Equals(LOOP) ? new LoopingConcatWaveProvider(sources) : new NonLoopingConcatWaveProvider(sources);
        }
    }
    internal class AudioSounds 
    {
        public int order { get; }
        public AudioFileReader audioFile { get; }

        public AudioSounds(ParsedAudioSounds jsonInput) 
        {
            if (jsonInput == null)
            {
                throw new ArgumentNullException("Json Audio Sounds is null");
            }
            String unescapedPath = Regex.Unescape(jsonInput.Path);
            if (File.Exists(unescapedPath))
            {
                audioFile = new AudioFileReader(unescapedPath);
            }
            else
            {
                throw new FileNotFoundException(unescapedPath);
            }
             
        }
    }
}
