using NAudio.Wave;

namespace Somnisonus
{
    internal interface MyWaveProvider : IWaveProvider
    {
        void Proceed();

        bool IsLoopable();

        void Reset();
    }
}
