using System.IO;

namespace Somnisonus
{
    public class Constants
    {
        public static string EmptyString = string.Empty;
        public static String StanzaDirectoryName = "Stanzas";
        public static String RoadmapDirectoryName = "Roadmaps";
        public static String JsonFileExtension = ".json";
        public static String WavFileExtension = ".wav";
        public static string StanzasDirectory = Path.Combine(Environment.CurrentDirectory, StanzaDirectoryName);
        public static string RoadmapsDirectory = Path.Combine(Environment.CurrentDirectory, RoadmapDirectoryName);
    }
}
