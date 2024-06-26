﻿namespace Somnisonus
{
    public class ParsedAudioStanza
    {
        public String Stanza_name { get; set; }
        public List<ParsedAudioSegment> Stanza_data { get; set; } = new List<ParsedAudioSegment>();
    }
    public class ParsedAudioSegment 
    {
        public String Type { get; set; }
        public int Order { get; set; }
        public List<ParsedAudioSounds> Sounds { get; set; } = new List<ParsedAudioSounds>();
    }
    public class ParsedAudioSounds
    {
        public int Order { get; set; }
        public String Path { get; set; }
        public int Cutoff_start { get; set; } = 0;
        public int Cutoff_end { get; set; } = 0;
    }
}
