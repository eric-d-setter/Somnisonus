﻿using System.IO;
using System.Text.Json;

namespace Somnisonus
{
    internal class AudioCollectionParser
    {
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

        public AudioCollection Generate()
        {
            return new AudioCollection(UseFileOpenReadTextWithSystemTextJson());
        }

        public ParsedAudioCollection UseFileOpenReadTextWithSystemTextJson()
        {
            //using FileStream json = File.OpenRead(_sampleJsonFilePath);
            ParsedAudioCollection collection;
            var json = File.ReadAllText(_sampleJsonFilePath);
            try
            {
                collection = JsonSerializer.Deserialize<ParsedAudioCollection>(json, _options);
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

        public void PreprocessAudioFiles(ParsedAudioCollection collection)
        { 
            if (collection != null)
            {
                try
                {
                    ParsedAudioCollection machineReadJson = new ParsedAudioCollection();
                    List<ParsedAudioSegment> newSegments = new List<ParsedAudioSegment>();
                    machineReadJson.Collection_name = collection.Collection_name;
                    String appFolder = Environment.CurrentDirectory;
                    String collectionFolder = CreateDirectoryLibraryIfNotExist(appFolder, Constants.collectionDirectory);
                    String collectionLibrary = CreateDirectoryLibraryIfNotExist(collectionFolder, collection.Collection_name);

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
                    machineReadJson.Collection_data = newSegments;
                    PrettyWrite(machineReadJson, Path.Combine(Constants.collectionDirectory, machineReadJson.Collection_name + Constants.jsonFileExtension));
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
            if (Path.GetExtension(path).Equals(Constants.wavFileExtension))
            {
                String newFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(path) + 
                    String.Format("_{0}_{1}{2}", sound.Cutoff_start, sound.Cutoff_end, Constants.wavFileExtension));
                if (!File.Exists(newFileName))
                {
                    fileSources.Add(path);
                    WavFileUtils.TrimWavFile(path, newFileName, new TimeSpan(0, 0, 0, 0,sound.Cutoff_start), new TimeSpan(0, 0, 0, 0, sound.Cutoff_end));
                }
                else if (fileSources.Add(path)) 
                {
                    newFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(path) +
                    String.Format("_{0}_{1}_{2}{3}", sound.Cutoff_start, sound.Cutoff_end, sound.Path.GetHashCode(), Constants.wavFileExtension));
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
