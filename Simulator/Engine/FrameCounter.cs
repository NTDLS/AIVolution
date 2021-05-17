using System;

namespace Simulator.Engine
{
    /// <summary>
    /// Keeps track of the enigne frame-rate.
    /// </summary>
    public class FrameCounter
    {
        public DateTime _lastFrame;
        public int _frameRateSamples;
        public double _totalFrameRate;

        public double CurrentFrameRate { get; private set; }
        public double AverageFrameRate { get; private set; }
        public double FrameRateMin { get; private set; }
        public double FrameRateMax { get; private set; }

        public FrameCounter()
        {
            Reset();
        }

        public void Reset()
        {
            _lastFrame = DateTime.MinValue;
            _totalFrameRate = 0;
            _frameRateSamples = 0;

            CurrentFrameRate = 0;
            AverageFrameRate = double.PositiveInfinity;
            FrameRateMin = double.PositiveInfinity;
            FrameRateMax = double.NegativeInfinity;
        }

        public void Calculate()
        {
            if (_lastFrame != DateTime.MinValue)
            {
                if (_frameRateSamples == 0 || _frameRateSamples > 1000)
                {
                    _frameRateSamples = 1;
                    _totalFrameRate = 0;
                }

                CurrentFrameRate = 1000.0 / (DateTime.Now - _lastFrame).TotalMilliseconds;
                _totalFrameRate += CurrentFrameRate;

                if (_frameRateSamples > 100)
                {
                    FrameRateMin = CurrentFrameRate < FrameRateMin ? CurrentFrameRate : FrameRateMin;
                    FrameRateMax = CurrentFrameRate > FrameRateMax ? CurrentFrameRate : FrameRateMax;
                    AverageFrameRate = _totalFrameRate / (double)_frameRateSamples;
                }
                _frameRateSamples++;
            }

            _lastFrame = DateTime.Now;
        }
    }
}


