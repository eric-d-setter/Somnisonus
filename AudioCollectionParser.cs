using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Text.Json;

namespace Somnisonus
{
    internal class AudioCollectionParser
    {
        private static String collectionDirectory = "Collections";
        private static String wavFileExtension = ".wav";
        private static String jsonFileExtension = ".json";
        private readonly string _sampleJsonFilePath;
        private HashSet<String> fileSources;
        public AudioCollectionParser(string sampleJsonFilePath)
        {
            _sampleJsonFilePath = sampleJsonFilePath;
            fileSources = new HashSet<String>();
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

        public ParsedAudioCollection UseFileOpenReadTextWithSystemTextJson()
        {
            using FileStream json = File.OpenRead(_sampleJsonFilePath);
            List<ParsedAudioCollection> collection = JsonSerializer.Deserialize<List<ParsedAudioCollection>>(json, _options);
            if (collection != null) 
            {
                return collection.First(); //One collection per JSON file
            }
            else
            {
                Console.WriteLine("Collection file is empty or formatted incorrectly.");
                return null;
            }
           
        }

        public void PreprocessAudioFiles(ParsedAudioCollection collection)
        {
            if (collection != null)
            {
                ParsedAudioCollection machineReadJson = new ParsedAudioCollection();
                List<ParsedAudioSegment> newSegments = new List<ParsedAudioSegment>();
                machineReadJson.Collection_name = collection.Collection_name;
                String appFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                String collectionFolder = CreateDirectoryLibraryIfNotExist(appFolder, collectionDirectory);
                String collectionLibrary = CreateDirectoryLibraryIfNotExist(collectionFolder, collection.Collection_name);
                try
                {
                    foreach (ParsedAudioSegment segment in collection.Collection_data)
                    {
                        ParsedAudioSegment newSegment = new ParsedAudioSegment();
                        newSegment.Order = segment.Order;
                        newSegment.Type = segment.Type;
                        List<ParsedAudioSounds> newSounds = new List<ParsedAudioSounds>();
                        foreach (ParsedAudioSounds sound in segment.Sounds)
                        {
                            newSounds.Add(CreateFile(sound, collectionLibrary));
                        }
                        newSegment.Sounds = newSounds;
                        newSegments.Add(newSegment);
                    }
                }
                catch (FileFormatException ex)
                {
                    Console.WriteLine(String.Format("File {0} is not a .wav file. All files must be .wav files for processing.", ex.Message));
                }
                machineReadJson.Collection_data = newSegments;
                PrettyWrite(machineReadJson, Path.Combine(collectionDirectory, machineReadJson.Collection_name + jsonFileExtension));
            }
        }

        private ParsedAudioSounds CreateFile(ParsedAudioSounds sound, String directory)
        {
            ParsedAudioSounds result = new ParsedAudioSounds();
            if (Path.GetExtension(sound.Path).Equals(wavFileExtension))
            {
                String newFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(sound.Path) + 
                    String.Format("_{0}_{1}{2}", sound.Cutoff_start, sound.Cutoff_end, wavFileExtension));
                if (!File.Exists(newFileName))
                {
                    fileSources.Add(sound.Path);
                    WavFileUtils.TrimWavFile(sound.Path, newFileName, new TimeSpan(0, 0, 0, 0,sound.Cutoff_start), new TimeSpan(0, 0, 0, 0, sound.Cutoff_end));
                }
                else if (fileSources.Add(sound.Path)) 
                {
                    newFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(sound.Path) +
                    String.Format("_{0}_{1}_{2}{3}", sound.Cutoff_start, sound.Cutoff_end, sound.Path.GetHashCode(), wavFileExtension));
                    WavFileUtils.TrimWavFile(sound.Path, newFileName, new TimeSpan(0, 0, 0, 0, sound.Cutoff_start), new TimeSpan(0, 0, 0, 0, sound.Cutoff_end));
                }
                else
                {
                    fileSources.Add(sound.Path);
                    Console.WriteLine("File already exists, moving to next file.");
                }
                result.Path = newFileName;
                return result;
            }
            else
            {
                throw new FileFormatException(Path.GetFileName(sound.Path));
            }


        }

        private String CreateDirectoryLibraryIfNotExist(String appFolder, String name)
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
