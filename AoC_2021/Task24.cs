using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task24
    {
        [Test]
        public void Solve()
        {
            var input = new Queue<int>();

            var strange = new[] { 11, 13, 12 };
            var c1 = new[] { 11, 13, 12, 15, 10, -1, 14, -8, -7, -8, 11, -2, -2, -13 };
            var c2 = new[] { 5, 5, 1, 15, 2, 2, 5, 8, 14, 12, 7, 14, 13, 6 };

            var z = 0;
            for (var i = 0; i < 14; ++i)
            {
                var x = z % 26 + c1[i];

                var w = input.Dequeue();
                x = x != w ? 1 : 0;

                z /= 26;//or z=z

                var y = (25 * x) + 1; // 26 or 1
                z *= y;//z * 26 or z

                y = (w + c2[i]) * x;//0 or w + c2[i]

                z += y;
            }
        }

        private static void ZiZZero()
        {
            var w = -1;
            var c2 = -2;
            var x = 1;

            var z = w + c2;//6..14


        }
    }
}