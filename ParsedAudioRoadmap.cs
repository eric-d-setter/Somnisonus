namespace Somnisonus
{
    public class ParsedAudioRoadmap
    {
        public String Roadmap_name { get; set; }
        public List<ParsedAudioData> Collection_data { get; set; } = new List<ParsedAudioData>();
        public String Start {  get; set; }
    }
    public class ParsedAudioData
    {
        public String Collection_name { get; set; }
        public List<ParsedNextData> Next { get; set; } = new List<ParsedNextData>();
    }
    public class ParsedNextData
    {
        public String Next_name { get; set; }
    }
}
