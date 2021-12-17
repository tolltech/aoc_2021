using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task17
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int x1, int x2, int y1, int y2, int expected)
        {
            //x1++;
            //x2++;
            //y1++;
            //y2++;

            var vx0Max = x2;
            var vx0Min = Math.Ceiling((-1 + Math.Sqrt(1 + 8 * x1)) / 2.0);

            var yMax = -1;

            for (var y = y1; y <= y2; ++y)
            for (var x = x1; x <= x2; ++x)
            for (var vx0 = vx0Min; vx0 <= vx0Max; ++vx0)
            {
                var t1 = (2 * vx0 + 1 - Math.Sqrt(Math.Pow(2 * vx0 + 1, 2) - 8 * x)) / 2;
                var t2 = (2 * vx0 + 1 + Math.Sqrt(Math.Pow(2 * vx0 + 1, 2) - 8 * x)) / 2;

                var ts = new[] { t1, t2 }.Where(xx => xx >= 0).ToArray();

                if (ts.Length <= 0) continue;

                var td = ts.Min();
                if (td <= 0) continue;
                if (td != (int)td) continue;

                var t = (int)td;

                var y0 = (2 * y + t - 1) / 2.0;

                if (y0 != (int)y0) continue;

                if (y0 <= 0) continue;

                var h = (int)((y0 + 1) * y0 / 2);

                if (yMax < h)
                {
                    yMax = h;
                }
            }

            yMax.Should().Be(expected);
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"target area: x=6..9, y=-1..-4", 6, 9, -3, 0, -3);
            yield return new TestCaseData(@"target area: x=20..30, y=-10..-5", 20, 30, -10, -5, 45);
            yield return new TestCaseData(@"target area: x=139..187, y=-148..-89", 139, 187, -148, -89, 12);
        }
    }
}