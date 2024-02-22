using NAudio.Wave;

namespace Somnisonus
{
    class MyWaveProviderWrapper : MyWaveProvider
    {
        private IWaveProvider waveProvider;
        public WaveFormat WaveFormat => waveProvider.WaveFormat;

        public MyWaveProviderWrapper(IWaveProvider waveProvider)
        {
            this.waveProvider = waveProvider;
        }

        public bool IsLoopable()
        {
            return false;
        }

        public void Proceed() {}

        public int Read(byte[] buffer, int offset, int count)
        {
            return waveProvider.Read(buffer, offset, count);
        }
    }
}
