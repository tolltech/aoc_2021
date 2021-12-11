using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task11
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int stepsCount, int expected)
        {
            var lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            var map = new int[lines.Length][];
            for (var i = 0; i < lines.Length; ++i)
            {
                var line = lines[i];
                map[i] = new int[line.Length];
                for (var j = 0; j < line.Length; ++j)
                    map[i][j] = int.Parse(line[j].ToString());
            }

            var acc = 0;

            for (var step = 0; step < stepsCount; ++step)
            {
                var highlited = new HashSet<(int Y, int X)>();
                for (var y = 0; y < map.Length; ++y)
                for (var x = 0; x < map[y].Length; ++x)
                {
                    Flash(map, y, x, highlited);
                }

                acc += highlited.Count;
            }

            acc.Should().Be(expected);
        }

        private void Flash(int[][] map, int y, int x, HashSet<(int Y, int X)> highlited)
        {
            if (highlited.Contains((y, x)))
                return;

            if (map[y][x] >= 9)
            {
                map[y][x] = 0;
                highlited.Add((y, x));

                var neighbors = GetNeighbours(y, x, map).ToArray();
                foreach (var neighbor in neighbors)
                {
                    Flash(map, neighbor.Y, neighbor.X, highlited);
                }
            }
            else
            {
                ++map[y][x];
            }
        }

        private IEnumerable<(int Y, int X, int Value)> GetNeighbours(int y, int x, int[][] map)
        {
            if (y < map.Length - 1)
                yield return (y + 1, x, map[y + 1][x]);

            if (y > 0)
                yield return (y - 1, x, map[y - 1][x]);

            if (x < map[y].Length - 1)
                yield return (y, x + 1, map[y][x + 1]);

            if (x > 0)
                yield return (y, x - 1, map[y][x - 1]);

            if (y < map.Length - 1)
            {
                if (x < map[y].Length - 1)
                    yield return (y + 1, x + 1, map[y + 1][x + 1]);

                if (x > 0)
                    yield return (y + 1, x - 1, map[y + 1][x - 1]);
            }

            if (y > 0)
            {
                if (x < map[y].Length - 1)
                    yield return (y - 1, x + 1, map[y - 1][x + 1]);

                if (x > 0)
                    yield return (y - 1, x - 1, map[y - 1][x - 1]);
            }
        }


        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"11111
19991
19191
19991
11111", 2, 9);
            yield return new TestCaseData(
                @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526", 10, 204);
            yield return new TestCaseData(
                @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526", 100, 1656);
            yield return new TestCaseData(
                @"2478668324
4283474125
1663463374
1738271323
4285744861
3551311515
8574335438
7843525826
1366237577
3554687226", 100, 1700);
        }
    }
}