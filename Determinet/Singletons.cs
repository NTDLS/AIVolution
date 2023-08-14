using System;

namespace Determinet
{
    internal static class Singletons
    {
        internal static class Generators
        {
            private static Random _random;

            public static Random Random
            {
                get
                {
                    _random ??= new Random();
                    return _random;
                }
            }
        }
    }
}
