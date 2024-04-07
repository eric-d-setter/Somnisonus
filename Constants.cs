using System.IO;

namespace Somnisonus
{
    public class Constants
    {
        public static string EmptyString = string.Empty;
        public static String StanzaDirectory = "Stanzas";
        public static String RoadmapDirectory = "Roadmaps";
        public static String JsonFileExtension = ".json";
        public static String WavFileExtension = ".wav";
        public static string CollectionsDirectory = Path.Combine(Environment.CurrentDirectory, StanzaDirectory);
        public static string RoadmapsDirectory = Path.Combine(Environment.CurrentDirectory, RoadmapDirectory);
    }
}
