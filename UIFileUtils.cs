using System.IO;
using Microsoft.Win32;

namespace Somnisonus
{
    internal class UIFileUtils
    {
        private static String collectionDirectory = "Collections";
        private static String roadmapDirectory = "Roadmaps";
        private static string CollectionsDirectory = Path.Combine(Environment.CurrentDirectory, collectionDirectory);
        private static string RoadmapsDirectory = Path.Combine(Environment.CurrentDirectory, roadmapDirectory);

        public static void ProcessCollectionConfig()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (String filename in openFileDialog.FileNames)
                {
                    AudioCollectionParser parser = new AudioCollectionParser(filename);
                    parser.PreprocessAudioFiles(parser.UseFileOpenReadTextWithSystemTextJson());
                }
            }
            // Trigger change?
        }

        public static void ProcessRoadmapConfig()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (String filename in openFileDialog.FileNames)
                {
                    AudioRoadMap.CopyFileToRoadmapDirectory(filename);
                }
            }
            // Trigger change?
        }

        public static List<String> GetCollections()
        {
            return Directory.GetFiles(CollectionsDirectory, "*.json", SearchOption.AllDirectories).ToList();
        }
        
        public static List<String> GetRoadmaps()
        {
            return Directory.GetFiles(RoadmapsDirectory, "*.json").ToList();
        }

    }
}
