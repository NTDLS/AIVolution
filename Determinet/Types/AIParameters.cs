﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Determinet.Types
{
    public class AIParameters<K, V> where V : IComparable<V>
    {
        private readonly Dictionary<K, V> _dictonary = new();

        public void Set(K key, V value)
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

        public void SetIfLess(K key, V value)
        {
            var existingValue = Get(key);

            if (existingValue.CompareTo(value) < 0)
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
        }

        public void SetIfGreater(K key, V value)
        {
            var existingValue = Get(key);

            if (existingValue.CompareTo(value) > 0)
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
        }

        public V[] ToArray()
        {
            var values = new V[_dictonary.Count];
            var keys = _dictonary.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                values[i] = _dictonary[keys[i]];
            }
            return values;
        }

        public V Get(K key)
        {
            return _dictonary[key];
        }

        public V Get(K key, V defaultValue)
        {
            if (_dictonary.TryGetValue(key, out V value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}
