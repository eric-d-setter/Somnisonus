namespace Somnisonus
{
    public class ParsedAudioRoadmap
    {
        public String Roadmap_name { get; set; }
        public List<ParsedAudioData> Collection_data { get; set; }
        public String Start {  get; set; }
    }
    public class ParsedAudioData
    {
        public String Collection_name { get; set; }
        public List<ParsedNextData>? Next { get; set; } = null;
    }
    public class ParsedNextData
    {
        public String Next_name { get; set; }
    }
}
