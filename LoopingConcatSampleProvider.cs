﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Somnisonus
{
    class LoopingConcatSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider[] providers;

        private int currentProviderIndex;

        //
        // Summary:
        //     The WaveFormat of this Sample Provider
        public WaveFormat WaveFormat => providers[0].WaveFormat;

        //
        // Summary:
        //     Creates a new ConcatenatingSampleProvider
        //
        // Parameters:
        //   providers:
        //     The source providers to play one after the other. Must all share the same sample
        //     rate and channel count
        public LoopingConcatSampleProvider(IEnumerable<ISampleProvider> providers)
        {
            this.EnableLooping = true;
            if (providers == null)
            {
                throw new ArgumentNullException("providers");
            }

            this.providers = providers.ToArray();
            if (this.providers.Length == 0)
            {
                throw new ArgumentException("Must provide at least one input", "providers");
            }

            if (this.providers.Any((ISampleProvider p) => p.WaveFormat.Channels != WaveFormat.Channels))
            {
                throw new ArgumentException("All inputs must have the same channel count", "providers");
            }

            if (this.providers.Any((ISampleProvider p) => p.WaveFormat.SampleRate != WaveFormat.SampleRate))
            {
                throw new ArgumentException("All inputs must have the same sample rate", "providers");
            }
        }

        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        public void Proceed(ISampleProvider provider)
        {
            this.EnableLooping = false;
            this.FollowedBy(provider);
        }

        //
        // Summary:
        //     Read Samples from this sample provider
        public int Read(float[] buffer, int offset, int count)
        {
            int num = 0;
            while (num < count && currentProviderIndex < providers.Length)
            {
                int count2 = count - num;
                int num2 = providers[currentProviderIndex].Read(buffer, offset + num, count2);
                num += num2;
                if (num2 == 0)
                {
                    currentProviderIndex++;
                }
                if (currentProviderIndex > providers.Length && EnableLooping)
                {
                    currentProviderIndex = 0;
                }
            }

            return num;
        }
    }
}