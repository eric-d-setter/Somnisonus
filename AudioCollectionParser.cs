using System.IO;
using System.Text.Json;

namespace Somnisonus
{
    internal class AudioCollectionParser
    {
        private readonly string _sampleJsonFilePath;
        public AudioCollectionParser(string sampleJsonFilePath)
        {
            _sampleJsonFilePath = sampleJsonFilePath;
        }

        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

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
                return null;
            }
           
        }
    }
}
