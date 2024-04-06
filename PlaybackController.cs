using NAudio.Wave;
using System.ComponentModel;

namespace Somnisonus
{
    internal class PlaybackController : INotifyPropertyChanged
    {
        private AudioRoadMap audioRoadmap;
        private QueuingSampleProvider queuer;
        private WaveOutEvent outputDevice;
        public MyWaveProvider NowPlaying { 
            get 
            {
                return queuer.NowPlaying;
            }
            private set { }
        }

        public List<string> NextOptions { get; private set; }
        public bool Loopable { get; private set; } = false;


        public event PropertyChangedEventHandler PropertyChanged;

        void PlaybackController_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "NextOptions":
                    NextOptions = queuer.NextOptions;
                    break;
                case "Loopable":
                    Loopable = queuer.Loopable;
                    break;
            }
        }

        public PlaybackController(string filename) 
        {
            PropertyChanged += PlaybackController_PropertyChanged;
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
