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
        // OLD STUFF TO REMOVE
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile1;
        private AudioFileReader audioFile2;
        private AudioFileReader audioFile3;
        private LoopingConcatWaveProvider loopingConcatSampleProvider;
        private ConcatenatingSampleProvider concatenatingSampleProvider;
        private QueuingSampleProvider mixer;
        private string filename1;
        private string filename2;
        private string filename3;
        // OLD STUFF END

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

        private void btn_OpenFile_End_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "WAV files (*.wav)|*.wav|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if (openFileDialog.ShowDialog() == true)
            {
                filename3 = openFileDialog.FileName;
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
            // Add pause ability
            if (outputDevice == null) 
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (audioFile1 == null && audioFile2 == null && audioFile3 == null)
            {
                audioFile1 = new AudioFileReader(filename1);
                
                audioFile2 = new AudioFileReader(filename2);

                audioFile3 = new AudioFileReader(filename3);

                
                //LoopStream loop = new LoopStream(audioFile);
                //outputDevice.Init(loop);
                //MyWaveProvider finalSound = new MyWaveProviderWrapper(audioFile3); 
                loopingConcatSampleProvider = new LoopingConcatWaveProvider(new AudioFileReader[] { audioFile1, audioFile2 });
                //concatenatingSampleProvider = new ConcatenatingSampleProvider(new ISampleProvider[] { loopingConcatSampleProvider.ToSampleProvider() });

                //mixer = new QueuingSampleProvider(new MyWaveProvider[] { loopingConcatSampleProvider, finalSound });
                outputDevice.Init(mixer);

            }
            outputDevice.Play();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            outputDevice?.Stop();
        }

        private void StopLoop_Click(object sender, RoutedEventArgs e)
        {
            mixer.StopLooping();
        }

        private void processFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (String filename in openFileDialog.FileNames)
                {
                    AudioStanzaParser parser = new AudioStanzaParser(filename);
                    parser.PreprocessAudioFiles(parser.UseFileOpenReadTextWithSystemTextJson());
                }                
            }
        }
    }
}