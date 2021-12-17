﻿using System;
using System.Collections.Generic;
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
            var vx0Min = (int)Math.Ceiling((-1 + Math.Sqrt(1 + 8 * x1)) / 2.0);

            var vy0Max = Math.Abs(y1) - 1;
            var vy0Min = 0;

            var yMax = -1;

            for (var vx0 = vx0Min; vx0 <= vx0Max; ++vx0)
            for (var vy0 = vy0Min; vy0 <= vy0Max; ++vy0)
            {
                var t = 0;
                while (++t > 0)
                {
                    var y = (vy0 + (vy0 - t + 1)) * t / 2;
                    if (y > y2) continue;
                    if (y < y1) break;

                    var x = t >= vx0
                        ? (vx0 + 1) * vx0 / 2
                        : (vx0 + (vx0 - t + 1)) * t / 2;

                    if (x >= x1 && x <= x2)
                    {
                        var h = (vy0 + 1) * vy0 / 2;
                        if (yMax < h)
                        {
                            yMax = h;
                        }
                    }
                }
            }

            yMax.Should().Be(expected);
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"target area: x=6..9, y=-1..-4", 6, 9, -3, 0, 3);
            yield return new TestCaseData(@"target area: x=20..30, y=-10..-5", 20, 30, -10, -5, 45);
            yield return new TestCaseData(@"target area: x=139..187, y=-148..-89", 139, 187, -148, -89, 10878);
        }
    }
}