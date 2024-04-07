namespace Somnisonus
{
    public class ParsedAudioRoadmap
    {
        public String Roadmap_name { get; set; }
        public List<ParsedAudioData> Stanza_data { get; set; } = new List<ParsedAudioData>();
        public String Start {  get; set; }
    }
    public class ParsedAudioData
    {
        public String Stanza_name { get; set; }
        public List<ParsedNextData> Next { get; set; } = new List<ParsedNextData>();
    }
    public class ParsedNextData
    {
        public String Next_name { get; set; }
    }
}
