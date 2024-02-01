using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Somnisonus
{
    public class AudioCollection
    {
        private ConcatenatingSampleProvider concatenatingSampleProvider;
        private bool currentlyLooping;
        public AudioCollection(List<CachedSound> sounds, bool loop) {
            List<CachedSoundSampleProvider> sampleProviders = new List<CachedSoundSampleProvider>();
            foreach (var sound in sounds)
            {
                sampleProviders.Add(new CachedSoundSampleProvider(sound));
            }
            concatenatingSampleProvider = new ConcatenatingSampleProvider(sampleProviders);
        }
        
        public void DeactivateLoop()
        {
            currentlyLooping = false;
        }

        public bool GetLoopingStatus()
        {
            return currentlyLooping;
        }

        public ConcatenatingSampleProvider GetConcatenatingSampleProvider()
        {
            return concatenatingSampleProvider;
        }
        
    }
}
