using System.Text.Json;
using System.IO;

namespace Somnisonus
{
    internal class AudioRoadMap
    {
        private readonly string _sampleJsonFilePath;
        private HashSet<String> fileSources;

        public Dictionary<String, AudioStanza> audioStanzas { get; } = new Dictionary<String, AudioStanza>();
        public AudioStanza startingAudioStanza { get; private set; }

        public static void CopyFileToRoadmapDirectory(string sampleJsonFilePath)
        {
            String roadMapFolder = CreateDirectoryLibraryIfNotExist(Constants.RoadmapsDirectory);
            File.Copy(sampleJsonFilePath, roadMapFolder + Path.GetFileName(sampleJsonFilePath), true);
        }

        public AudioRoadMap(string sampleJsonFilePath)
        {
            ParsedAudioRoadmap RoadMap;

            _sampleJsonFilePath = sampleJsonFilePath;
            fileSources = new HashSet<String>();
            
            String stanzaFolder = CreateDirectoryLibraryIfNotExist(Constants.StanzasDirectory);
            var json = File.ReadAllText(_sampleJsonFilePath);

            try
            {
                RoadMap = JsonSerializer.Deserialize<ParsedAudioRoadmap>(json, _options);
                if (RoadMap == null)
                {
                    Console.WriteLine("Stanza file is empty or formatted incorrectly.");
                }
                else
                {
                    foreach (ParsedAudioData stanza in RoadMap.Stanza_data) // Pass one to generate all data structures
                    {
                        String stanzaLibrary = Path.Combine(stanzaFolder, stanza.Stanza_name);
                        audioStanzas[stanza.Stanza_name] = new AudioStanzaParser(Path.Combine(stanzaLibrary, stanza.Stanza_name + Constants.JsonFileExtension)).Generate();
                    }
                    foreach (ParsedAudioData stanza in RoadMap.Stanza_data)
                    {
                        if (stanza.Next != null)
                        {
                            foreach (ParsedNextData nextOptions in stanza.Next)
                            {
                                audioStanzas[stanza.Stanza_name].playNextOptions.Add(audioStanzas[nextOptions.Next_name]);
                            }
                        }
                    }
                    startingAudioStanza = audioStanzas[RoadMap.Start];
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

        private static String CreateDirectoryLibraryIfNotExist(string name)
        {
            String path = name;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        private static String CreateDirectoryLibraryIfNotExist(String appFolder, String name)
        {
            String path = Path.Combine(appFolder, name);
            return CreateDirectoryLibraryIfNotExist(path);
        }
    }
}
