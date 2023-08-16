using Newtonsoft.Json;

namespace Determinet.Types
{
    /// <summary>
    /// Used to pass optional parameters to activation functions.
    /// </summary>
    [Serializable]
    public class DniNamedFunctionParameters
    {
        [JsonProperty]
        private readonly Dictionary<string, object> _dictonary = new();

        public void Set(string key, object value)
        {
            key = key.ToLower();
            if (_dictonary.ContainsKey(key))
            {
                _dictonary[key] = value;
            }
            else
            {
                _dictonary.Add(key, value);
            }
        }

        public object[] ToArray()
        {
            var values = new object[_dictonary.Count];
            var keys = _dictonary.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                values[i] = _dictonary[keys[i]];
            }
            return values;
        }

        public T Get<T>(string key)
        {
            key = key.ToLower();
            return (T)_dictonary[key];
        }

        public T Get<T>(string key, T defaultValue)
        {
            key = key.ToLower();
            if (_dictonary.ContainsKey(key))
            {
                return (T)_dictonary[key];
            }
            return defaultValue;
        }

        public KeyValuePair<string, object> Get(int index)
        {
            return _dictonary.ElementAt(index);
        }

        public object Get(string key, object defaultValue)
        {
            if (_dictonary.TryGetValue(key, out object? value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}
