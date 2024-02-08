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
        private AudioFileReader audioFile1;
        private AudioFileReader audioFile2;
        private LoopingConcatWaveProvider loopingConcatSampleProvider;
        private CachedSoundSampleProvider cachedSoundSampleProvider;
        private CachedSoundSampleProvider cachedSoundSampleProvider2;
        private string filename1;
        private string filename2;


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
                filename1 = openFileDialog.FileName;
            }
        }


        private void btnOpenFiles_Click2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "WAV files (*.wav)|*.wav|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if (openFileDialog.ShowDialog() == true)
            {
                filename2 = openFileDialog.FileName;
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile1.Dispose();
            audioFile1 = null;
            audioFile2.Dispose();
            audioFile2 = null;
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (audioFile1 == null && audioFile2 == null)
            {
                audioFile1 = new AudioFileReader(filename1);
                
                audioFile2 = new AudioFileReader(filename2);

                //LoopStream loop = new LoopStream(audioFile);
                //outputDevice.Init(loop);
                loopingConcatSampleProvider = new LoopingConcatWaveProvider(new AudioFileReader[] { audioFile1, audioFile2 });
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