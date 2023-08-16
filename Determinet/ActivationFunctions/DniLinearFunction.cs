﻿using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// Linear bounded activation function.
    /// </summary>
    [Serializable]
    public class DniLinearFunction : DniIActivationFunction
    {
        /// <summary>
        /// Linear slope value.
        /// </summary>
        [JsonProperty]
        public double Alpha { get; set; }

        /// <summary>
        /// Function output range.
        /// </summary>
        [JsonProperty]
        public DniRange Range { get; set; }

        public DniLinearFunction(DniNamedFunctionParameters? param)
        {

            if (param == null)
            {
                Alpha = 1;
                Range = new DniRange(-1, +1);
            }
            else
            {
                Alpha = param.Get<double>("alpha", 1);
                Range = param.Get<DniRange>("range", new DniRange(-1, +1));
            }
        }

        public double Activation(double x)
        {
            double y = Alpha * x;

            if (y > Range.Max)
                return Range.Max;
            else if (y < Range.Min)
                return Range.Min;
            return y;
        }

        public double Derivative(double x)
        {
            double y = Alpha * x;

            if (y <= Range.Min || y >= Range.Max)
                return 0;
            return Alpha;
        }
    }
}

