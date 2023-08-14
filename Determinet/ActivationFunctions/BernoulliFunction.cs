﻿using System;

namespace Determinet.ActivationFunctions
{
    [Serializable]
    public class BernoulliFunction : IActivationMachine
    {
        private double alpha; // sigmoid's alpha value
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        public BernoulliFunction(object[] param)
        {
            if (param == null)
            {
                alpha = 1;
            }
            else if (param.Length != 1)
            {
                Alpha = (double)param[0];
            }
            else
            {
                throw new ArgumentException("Invalid number of parameters supplied for BernoulliFunction.");
            }
        }

        public double Activation(double x)
        {
            return (1 / (1 + Math.Exp(-alpha * x)));
        }

        public double Generate(double x)
        {
            double y = Activation(x);
            return y > Singletons.Generators.Random.NextDouble() ? 1 : 0;
        }

        public double Derivative(double x)
        {
            double y = Activation(x);

            return (alpha * y * (1 - y));
        }
    }
}