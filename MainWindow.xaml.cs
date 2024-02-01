using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.IO;
using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Somnisonus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaveOutEvent outputDevice;
        private WaveFileReader audioFile;
        private LoopingConcatSampleProvider loopingConcatSampleProvider;
        private CachedSoundSampleProvider cachedSoundSampleProvider;
        private string filename;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "WAV files (*.wav)|*.wav|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if (openFileDialog.ShowDialog() == true) {
                filename = openFileDialog.FileName;
                cachedSoundSampleProvider = new CachedSoundSampleProvider(new CachedSound(filename));
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile.Dispose();
            audioFile = null;
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (audioFile == null)
            {
                //audioFile = new WaveFileReader(filename);
                //LoopStream loop = new LoopStream(audioFile);
                //outputDevice.Init(loop);
                outputDevice.Init(loopingConcatSampleProvider);
            }
            outputDevice.Play();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            outputDevice?.Stop();
        }
    }
}