using System.Text.Json;
using System.IO;

namespace Somnisonus
{
    internal class AudioRoadMap
    {
        private readonly string _sampleJsonFilePath;
        private HashSet<String> fileSources;

        public Dictionary<String, AudioStanza> audioCollections { get; } = new Dictionary<String, AudioStanza>();
        public AudioStanza startingAudioCollection { get; private set; }

        public static void CopyFileToRoadmapDirectory(string sampleJsonFilePath)
        {
            String roadMapFolder = CreateDirectoryLibraryIfNotExist(Environment.CurrentDirectory, Constants.RoadmapDirectory);
            File.Copy(sampleJsonFilePath, roadMapFolder + Path.GetFileName(sampleJsonFilePath), true);
        }

        public AudioRoadMap(string sampleJsonFilePath)
        {
            ParsedAudioRoadmap RoadMap;

            _sampleJsonFilePath = sampleJsonFilePath;
            fileSources = new HashSet<String>();
            
            String appFolder = Environment.CurrentDirectory;
            String collectionFolder = CreateDirectoryLibraryIfNotExist(appFolder, Constants.StanzaDirectory);
            var json = File.ReadAllText(_sampleJsonFilePath);

            try
            {
                RoadMap = JsonSerializer.Deserialize<ParsedAudioRoadmap>(json, _options);
                if (RoadMap == null)
                {
                    Console.WriteLine("Collection file is empty or formatted incorrectly.");
                }
                else
                {
                    foreach (ParsedAudioData collection in RoadMap.Stanza_data) // Pass one to generate all data structures
                    {
                        String path = Path.Combine(appFolder, collection.Stanza_name);
                        audioCollections[collection.Stanza_name] = new AudioStanzaParser(Path.Combine(path, collection.Stanza_name + Constants.JsonFileExtension)).Generate();
                    }
                    foreach (ParsedAudioData collection in RoadMap.Stanza_data)
                    {
                        if (collection.Next != null)
                        {
                            foreach (ParsedNextData nextOptions in collection.Next)
                            {
                                audioCollections[collection.Stanza_name].playNextOptions.Add(audioCollections[nextOptions.Next_name]);
                            }
                        }
                    }
                    startingAudioCollection = audioCollections[RoadMap.Start];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot parse JSON file");
            }
        }

        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        private static void PrettyWrite(object obj, string fileName)
        {
            var jsonString = JsonSerializer.Serialize(obj, _options);
            File.WriteAllText(fileName, jsonString);
        }

        private static void CheckIfSourceExistsOrThrowException(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException(filepath);
            }
        }

        private static String CreateDirectoryLibraryIfNotExist(String appFolder, String name)
        {
            String path = Path.Combine(appFolder, name);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
