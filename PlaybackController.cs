using NAudio.Wave;

namespace Somnisonus
{
    internal class PlaybackController
    {
        private AudioRoadMap audioRoadmap;
        private QueuingSampleProvider queuer;
        private WaveOutEvent outputDevice;

        public PlaybackController(string filename) 
        {
            audioRoadmap = new AudioRoadMap(Constants.RoadmapsDirectory + filename);
            queuer = new QueuingSampleProvider(audioRoadmap);
        }

        public void PlayPause() 
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
                outputDevice.Init(queuer);
            }
            if (outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                outputDevice.Play();
            }
            if (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Pause();
            }
            else if (outputDevice.PlaybackState == PlaybackState.Paused)
            {
                outputDevice.Play();
            }
            
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
        }

        public void Stop() 
        {
            outputDevice?.Stop();
        }

        public List<string> NextOptions()
        {
            return queuer.NextOptions;
        }

        public void SetNext(string option)
        {
            queuer.NextCollection = option;
        }

        public void StopLooping()
        {
            queuer.StopLooping();
        }
    }
}
