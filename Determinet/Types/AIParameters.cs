using System.Collections.Generic;
using System.Linq;

namespace Determinet.Types
{
    public class AIParameters
    {
        private readonly Dictionary<string, double> _dictonary = new();

        public void Set(string key, double value)
        {
            if (_dictonary.ContainsKey(key))
            {
                _dictonary[key] = value;
            }
            else
            {
                _dictonary.Add(key, value);
            }
        }

        public void SetIfLess(string key, double value)
        {
            if (_dictonary.ContainsKey(key) == false)
            {
                _dictonary.Add(key, value);
            }
            else
            {
                var existingValue = _dictonary[key];

                if (value < existingValue)
                {
                    _dictonary[key] = value;
                }
            }
        }

        public void SetIfGreater(string key, double value)
        {
            if (_dictonary.ContainsKey(key) == false)
            {
                _dictonary.Add(key, value);
            }
            else
            {
                var existingValue = _dictonary[key];

                if (value > existingValue)
                {
                    _dictonary[key] = value;
                }
            }
        }

        public double[] ToArray()
        {
            var values = new double[_dictonary.Count];
            var keys = _dictonary.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                values[i] = _dictonary[keys[i]];
            }
            return values;
        }

        public double Get(string key)
        {
            return _dictonary[key];
        }

        public KeyValuePair<string, double> Get(int index)
        {
            return _dictonary.ElementAt(index);
        }

        public double Get(string key, double defaultValue)
        {
            if (_dictonary.TryGetValue(key, out double value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}
