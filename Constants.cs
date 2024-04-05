using System.IO;

namespace Somnisonus
{
    public class Constants
    {
        public static string EMPTY_STRING = string.Empty;
        public static String collectionDirectory = "Collections";
        public static String roadmapDirectory = "Roadmaps";
        public static String jsonFileExtension = ".json";
        public static String wavFileExtension = ".wav";
        public static string CollectionsDirectory = Path.Combine(Environment.CurrentDirectory, collectionDirectory);
        public static string RoadmapsDirectory = Path.Combine(Environment.CurrentDirectory, roadmapDirectory);
    }
}
