namespace Somnisonus
{
    public class ParsedAudioCollection
    {
        public String Collection_name { get; set; }
        public List<ParsedAudioSegment> Collection_data { get; set; }
    }
    public class ParsedAudioSegment 
    {
        public String Type { get; set; }
        public int Order { get; set; }
        public List<ParsedAudioSounds> Sounds { get; set; }
    }
    public class ParsedAudioSounds
    {
        public String Path { get; set; }
        public int Cutoff_start { get; set; } = 0;
        public int Cutoff_end { get; set; } = 0;
    }
}
