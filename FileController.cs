using System.IO;
using Microsoft.Win32;

namespace Somnisonus
{
    internal class FileController
    {
        private bool roadMapSet = false;
        private string filename = Constants.EmptyString;

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
            return Directory.GetFiles(Constants.CollectionsDirectory, "*.json", SearchOption.AllDirectories).ToList();
        }
        
        public static List<String> GetRoadmaps()
        {
            return Directory.GetFiles(Constants.RoadmapsDirectory, "*.json").ToList();
        }

        public void SetRoadmap(string filename)
        {
            this.filename = filename;
            roadMapSet = true;
        }
    }
}
