using System.IO;
using System.Text.Json;

namespace Somnisonus
{
    internal class AudioStanzaParser
    {
        private readonly string _sampleJsonFilePath;
        private HashSet<String> fileSources;
        public AudioStanzaParser(string sampleJsonFilePath)
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

        public AudioStanza Generate()
        {
            return new AudioStanza(UseFileOpenReadTextWithSystemTextJson());
        }

        public ParsedAudioStanza UseFileOpenReadTextWithSystemTextJson()
        {
            ParsedAudioStanza collection;
            var json = File.ReadAllText(_sampleJsonFilePath);
            try
            {
                collection = JsonSerializer.Deserialize<ParsedAudioStanza>(json, _options);
                if (collection != null)
                {
                    return collection; //One collection per JSON file
                }
                else
                {
                    Console.WriteLine("Collection file is empty or formatted incorrectly.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot parse JSON file");
            }
            return null;
        }

        public void PreprocessAudioFiles(ParsedAudioStanza collection)
        { 
            if (collection != null)
            {
                try
                {
                    ParsedAudioStanza machineReadJson = new ParsedAudioStanza();
                    List<ParsedAudioSegment> newSegments = new List<ParsedAudioSegment>();
                    machineReadJson.Stanza_name = collection.Stanza_name;
                    String appFolder = Environment.CurrentDirectory;
                    String collectionFolder = CreateDirectoryLibraryIfNotExist(appFolder, Constants.StanzaDirectory);
                    String collectionLibrary = CreateDirectoryLibraryIfNotExist(collectionFolder, collection.Stanza_name);

                    foreach (ParsedAudioSegment segment in collection.Stanza_data)
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
                    machineReadJson.Stanza_data = newSegments;
                    PrettyWrite(machineReadJson, Path.Combine(Constants.StanzaDirectory, machineReadJson.Stanza_name + Constants.JsonFileExtension));
                }
                catch (FileFormatException ex)
                {
                    Console.WriteLine(String.Format("File {0} is not a .wav file. All files must be .wav files for processing.", ex.Message));
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine("Failed to find source audio file @:" + ex.Message);
                }
            }
        }

        private ParsedAudioSounds CreateFile(ParsedAudioSounds sound, String directory)
        {
            ParsedAudioSounds result = new ParsedAudioSounds();
            String path = @sound.Path;

            result.Order = sound.Order;
            if (Path.GetExtension(path).Equals(Constants.WavFileExtension))
            {
                String newFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(path) + 
                    String.Format("_{0}_{1}{2}", sound.Cutoff_start, sound.Cutoff_end, Constants.WavFileExtension));
                if (!File.Exists(newFileName))
                {
                    fileSources.Add(path);
                    WavFileUtils.TrimWavFile(path, newFileName, new TimeSpan(0, 0, 0, 0,sound.Cutoff_start), new TimeSpan(0, 0, 0, 0, sound.Cutoff_end));
                }
                else if (fileSources.Add(path)) 
                {
                    newFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(path) +
                    String.Format("_{0}_{1}_{2}{3}", sound.Cutoff_start, sound.Cutoff_end, sound.Path.GetHashCode(), Constants.WavFileExtension));
                    WavFileUtils.TrimWavFile(path, newFileName, new TimeSpan(0, 0, 0, 0, sound.Cutoff_start), new TimeSpan(0, 0, 0, 0, sound.Cutoff_end));
                }
                else
                {
                    fileSources.Add(path);
                    Console.WriteLine("File already exists, moving to next file.");
                }
                result.Path = newFileName;
                return result;
            }
            else
            {
                throw new FileFormatException(Path.GetFileName(path));
            }


        }

        private void CheckIfSourceExistsOrThrowException(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException(filepath);
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
