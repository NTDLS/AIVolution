﻿using System.Text;

namespace Determinet
{
    internal static class Utility
    {
        public static int Checksum(string buffer)
        {
            return Checksum(Encoding.ASCII.GetBytes(buffer));
        }

        public static int Checksum(byte[] buffer)
        {
            int sum = 0;
            foreach (var b in buffer)
            {
                sum += (int)(sum ^ b);
            }
            return sum;
        }
    }
}
