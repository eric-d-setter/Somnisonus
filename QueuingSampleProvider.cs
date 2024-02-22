using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Somnisonus
{
    class QueuingSampleProvider : ISampleProvider
    {
        private readonly List<ISampleProvider> sources; // Source needs to be kept at 1

        private float[] sourceBuffer;

        private const int MaxInputs = 1024;
        private readonly Queue<MyWaveProvider> queue;
        // TODO Need to add queue
        public MyWaveProvider NowPlaying { get; private set; } 
        // TODO Need to add the now playing

        //
        // Summary:
        //     Returns the mixer inputs (read-only - use AddMixerInput to add an input
        public IEnumerable<ISampleProvider> MixerInputs => sources;

        //
        // Summary:
        //     When set to true, the Read method always returns the number of samples requested,
        //     even if there are no inputs, or if the current inputs reach their end. Setting
        //     this to true effectively makes this a never-ending sample provider, so take care
        //     if you plan to write it out to a file.
        public bool ReadFully { get; set; }

        //
        // Summary:
        //     The output WaveFormat of this sample provider
        public WaveFormat WaveFormat { get; private set; }

        //
        // Summary:
        //     Raised when a mixer input has been removed because it has ended
        public event EventHandler<SampleProviderEventArgs> MixerInputEnded;

        //
        // Summary:
        //     Creates a new MixingSampleProvider, with no inputs, but a specified WaveFormat
        //
        //
        // Parameters:
        //   waveFormat:
        //     The WaveFormat of this mixer. All inputs must be in this format
        public QueuingSampleProvider(WaveFormat waveFormat)
        {
            if (waveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Mixer wave format must be IEEE float");
            }

            sources = new List<ISampleProvider>();
            queue = new Queue<MyWaveProvider>();
            WaveFormat = waveFormat;
        }

        //
        // Summary:
        //     Creates a new MixingSampleProvider, based on the given inputs
        //
        // Parameters:
        //   sources:
        //     Mixer inputs - must all have the same waveformat, and must all be of the same
        //     WaveFormat. There must be at least one input
        public QueuingSampleProvider(IEnumerable<MyWaveProvider> queuable)
        {
            this.sources = new List<ISampleProvider>();
            queue = new Queue<MyWaveProvider>();
            if (queuable.Count() == 0)
            {
                throw new ArgumentException("Queue must contain at least one input");
            }
            AddToQueue(queuable.ToList());
        }

        public void AddToQueue(IEnumerable<MyWaveProvider> queuable)
        {
            foreach (MyWaveProvider item in queuable)
            {
                AddToQueue(item);
            }
        }

        public void AddToQueue(MyWaveProvider queuable)
        {
            queue.Enqueue(queuable);
            if (sources.Count == 0)
            {
                PlayNext();
            }
        }

        public void StopLooping()
        {
            NowPlaying.Proceed();
        } 

        private void PlayNext()
        {
            if (queue.Count > 0)
            {
                NowPlaying = queue.Dequeue();
            }
            AddMixerInput(NowPlaying);

        }
        //
        // Summary:
        //     Adds a WaveProvider as a Mixer input. Must be PCM or IEEE float already
        //
        // Parameters:
        //   mixerInput:
        //     IWaveProvider mixer input
        private void AddMixerInput(IWaveProvider mixerInput)
        {
            AddMixerInput(mixerInput.ToSampleProvider());
        }

        //
        // Summary:
        //     Adds a new mixer input
        //
        // Parameters:
        //   mixerInput:
        //     Mixer input
        private void AddMixerInput(ISampleProvider mixerInput)
        {
            lock (sources)
            {
                if (sources.Count >= 1024)
                {
                    throw new InvalidOperationException("Too many mixer inputs");
                }

                sources.Add(mixerInput);
            }

            if (WaveFormat == null)
            {
                WaveFormat = mixerInput.WaveFormat;
            }
            else if (WaveFormat.SampleRate != mixerInput.WaveFormat.SampleRate || WaveFormat.Channels != mixerInput.WaveFormat.Channels)
            {
                throw new ArgumentException("All mixer inputs must have the same WaveFormat");
            }
        }

        //
        // Summary:
        //     Removes a mixer input
        //
        // Parameters:
        //   mixerInput:
        //     Mixer input to remove
        private void RemoveMixerInput(ISampleProvider mixerInput)
        {
            lock (sources)
            {
                sources.Remove(mixerInput);
            }
        }

        //
        // Summary:
        //     Removes all mixer inputs
        private void RemoveAllMixerInputs()
        {
            lock (sources)
            {
                sources.Clear();
            }
        }

        //
        // Summary:
        //     Reads samples from this sample provider
        //
        // Parameters:
        //   buffer:
        //     Sample buffer
        //
        //   offset:
        //     Offset into sample buffer
        //
        //   count:
        //     Number of samples required
        //
        // Returns:
        //     Number of samples read
        public int Read(float[] buffer, int offset, int count)
        {
            int num = 0;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, count);
            lock (sources)
            {
                for (int num2 = sources.Count - 1; num2 >= 0; num2--)
                {
                    ISampleProvider sampleProvider = sources[num2];
                    int num3 = sampleProvider.Read(sourceBuffer, 0, count);
                    int num4 = offset;
                    for (int i = 0; i < num3; i++)
                    {
                        if (i >= num)
                        {
                            buffer[num4++] = sourceBuffer[i];
                        }
                        else
                        {
                            buffer[num4++] += sourceBuffer[i];
                        }
                    }

                    num = Math.Max(num3, num);
                    if (num3 < count)
                    {
                        this.MixerInputEnded?.Invoke(this, new SampleProviderEventArgs(sampleProvider));
                        sources.RemoveAt(num2);
                        if (queue.Count > 0)
                        {
                            PlayNext();
                        }
                        // TODO after removing the source, add a new source on from the queue and update the now playing
                    }
                }
            }

            if (ReadFully && num < count)
            {
                int num5 = offset + num;
                while (num5 < offset + count)
                {
                    buffer[num5++] = 0f;
                }

                num = count;
            }

            return num;
        }
    }
}
