﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task20_2
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int stepCount, int expected)
        {
            var blocks = input.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);
            var algorithmString = blocks[0];
            var lines = blocks[1].Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var flash = algorithmString[0] == '#';

            var map = new Map(flash);
            for (var y = 0; y < lines.Length; ++y)
            for (var x = 0; x < lines[y].Length; ++x)
            {
                if (lines[y][x] == '#') map.AddPixel((y, x));
            }

            for (var i = 0; i < stepCount; ++i)
            {
                //var dbg = Dbg(map, i);
                var newMap = new Map(flash);

                for (var y = map.MinY - 2; y <= map.MaxY + 2; ++y)
                for (var x = map.MinX - 2; x <= map.MaxX + 2; ++x)
                {
                    var code = new string(GetNeighborsAndSelf((y, x)).Select(p => map.IsLight(p, i) ? '1' : '0')
                        .ToArray()).ToIntFromBin();
                    if (algorithmString[code] == '#')
                    {
                        newMap.AddPixel((y, x));
                    }
                }

                map = newMap;
            }

            map.Lights.Count.Should().Be(expected);
        }

        private static string Dbg(Map map, int stepCount)
        {
            var sb = new StringBuilder();
            for (var y = map.MinY - 2; y <= map.MaxY + 2; ++y)
            {
                for (var x = map.MinX - 2; x <= map.MaxX + 2; ++x)
                {
                    sb.Append(map.IsLight((y, x), stepCount) ? '#' : '.');
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        class Map
        {
            private readonly bool flash;

            public Map(bool flash)
            {
                this.flash = flash;
                Lights = new HashSet<(int X, int Y)>();
            }

            public int MaxX { get; set; }
            public int MaxY { get; set; }
            public int MinY { get; set; }
            public int MinX { get; set; }

            public HashSet<(int X, int Y)> Lights { get; set; }

            public Map AddPixel((int Y, int X) pixel)
            {
                Lights.Add(pixel);

                var (y, x) = pixel;

                if (x > MaxX) MaxX = x;
                if (x < MinX) MinX = x;
                if (y < MinY) MinY = y;
                if (y > MaxY) MaxY = y;

                return this;
            }

            public bool IsLight((int Y, int X) pixel, int stepCount)
            {
                if (!flash)
                {
                    return Lights.Contains(pixel);
                }

                var (y, x) = pixel;

                if (y > MaxY || y < MinY || x > MaxX || x < MinX)
                {
                    if (stepCount % 2 == 0)
                        return false;

                    return true;
                }

                return Lights.Contains(pixel);
            }
        }

        private IEnumerable<(int Y, int X)> GetNeighborsAndSelf((int Y, int X) pixel)
        {
            var (y, x) = pixel;

            yield return (y - 1, x - 1);
            yield return (y - 1, x);
            yield return (y - 1, x + 1);
            yield return (y, x - 1);
            yield return (y, x);
            yield return (y, x + 1);
            yield return (y + 1, x - 1);
            yield return (y + 1, x);
            yield return (y + 1, x + 1);
        }


        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(
                @"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###", 50, 3351);
            yield return new TestCaseData(
                @"#####..#.#######.##.#.#.#.#####..######...#.##.###...#.##..#####..###.#.####..#.##.#....#...##.###..#.#......####.#...#.####.#..#.###.#.#.#.###.###..########.##.#.#....#.#####.......###.#..#.#.###.###.###.#.......#.#....#.##.###.....##...#.#.#..#.#....##..#####...##...#.##..##.##.#...###...#.##...#...#.####.###...........#...#..##...##.###.##.###.....#.##...###.....#.#.###..##..#..##..#.....##..##....#....####...#.###.#######.#.##.####.....####.#.#.##.#####...#.##.##.#.##..####.#.#.######.#..###..#.##.##...

##.##....#..#....#.....#..##.#...######.##.##..#..##.###.##..##.#..#..####..##..#.#..###..##...##.##
..##.###.##..##.###..#.###....##.#.#...##.#.#..####.#..#..##...##......###...###.#..##...##.##.##..#
.#..####..##..###..#.....#....##....###..####.##.##.#.#.##..######..#####.#...##.#..##..##..##...##.
..###.......##.#..#.##.##.#..#######..##.......###.###.###.#..###...#.#####..###....###.##..###..##.
.#..####..###.#.#..#.#..##...#...#.##...##..##..##.#######.....#.#..#.#.#.#.#.#..#.##......####..##.
##..#...###.##....####.###.###.#.#.#........##...#.#....#######.######...##.###...##.###.##.######..
##..##...#....#....#####.#.....#....#.#..#.#.##...##.##...#.####...#####.#.#.##...#..##.##.##.##..##
..#..##..#..#...####....#.....###....##.#..#...#..#.##.#....#.#.#...##.#..#..#..##....#.##..#..##.##
..##....######.###.#.....##.##....###...#..#.....#...###.###....#...#.##.####..##.###....#####...##.
.#..##.###.#......#.#####.####..####......###.....####..####..####..#.######.####..##.#...#.##.####.
####..##.##.##..#.#...##.####..####.#...#.#.##..#.###.#.....#....###.####.###..#.#..#.#...#.#.##..##
.##..#....###..#####.##.....###.##.###.######.#.###.#.##.#.#######....##..#.#.##..#.##...#######..##
###.......###.....#.#.##....##..#.##...#....####.##.###.....#.####..#........#...####.##.#.##...##.#
##.##....##..#####.#...####.....##..#..#...##.#...##.#..#..########...###.##...#.##...##.#..##.##...
.#.###...#.###..##...####..#.###.##.#.#...#..###.#.####..#..##.##.####.###...##.##.###..#.#.####.##.
#..#.#.#.#.#.#.#..#####.#....####.#.......#..#####..##..#..###....#...#..###.#..###.##..#.###.##.###
....##..##.###..####.#.###.##.....#...#...##.##.#.###..##.#.#..#....#.####.##...#.#...#.#...###...#.
##...#....##.#....###....#..##.##.####.#.#.##..##.####...#.#....####..#.##.....#....#...#..##...#...
#.##...#.##..#.##.###..##...##.#.#..##..#.....###.#.####....#######.#.#.#####.#....##.#.....####....
.....#####..###...#.#.#.#...#.#..#.###..##.##....##.###..###..###.#.#...#.#.##.###...#...##.#..#.##.
...###.###.###.#..#.#.####.##...##.##..#####.......#.##.....#####..#.#######...#..#.###......#######
#..##.....###..#.###..#..###.#.##...#...#......#.###..#...###...#..##......#.....##.####..##...#...#
.....##..#.#.#..#.#.#.#####...#..#.......#..#..#....#..##..##.#.#..#.####..#...#..##...##...#..#####
...##..#.#...#...#.####..#.##.###..##..###.......#..######.#.###.#..#.##.###.....########..#..#...#.
#.#...##...#.##.##.#..#..#.#....##.#.#...#.#..####.#...#.#..######.#.##....#.#...##....#..#.#.##.###
...#####.##.####.##...#..##.#..##...#..##..#.##...##..#.#.###......#.###.#.#..###...##..##.##..###..
.##...###..#....#.#.....#..#.##..###.#..###..###.#..#.#.###..#...#..####..#.###.##.##.#.#.#..#..#..#
.#..#.#.#..#####.##...#.#...###.####.##..###....######.##..##.#...#.###..###.###....##.#......#.##.#
#..####.######....###..####.##.##.....#.##.####.##..##.#..##.#.#.#.##.##..#.###.##..####.##.#.##..##
....#.#..........#.##....#.#..#.###.##.####.#.#..##.#..##.#..#.....###.#..###.######..##.#......####
...#..#.##.###########...###.###..##......#..###.#.#...##.##...####.##..#.........##...##.###.###.#.
.##.#####.#...#.###.#..#..#.#..#.##...#.#..##.#.####.##..######....##.####...#..###.####.#...##.#...
...#.##..#..#.#..##..#.#..##.....#.##.#....#..#..#.#......##.####...####...##......##..##....####.#.
####.##.#...#....#.#.#..###.##.###..#.###.#.#..#.##.##..##.##.#.#..#.####...###.####.##.....#.#...##
#........#.###..#.#.#####...#.##.#.###...#..#...##.##..#.###....#.###..#.#.#.###.#.##.#....#......##
...#.##..##.#..........#..#....###.#..#..#.#...###.##..#.#...###.####...####..#.####.######.#....#..
##.#.####.##.#.##..#..##...###...###.##..##.#####..###..#..#.#..#.##...#.####...##.##..#.#.#..###...
#.####..##..#.#..##..##.#..#..#...#.###.#.##.####.###.###...##.#####..##..####...##.##########.#.###
####.##..###.####.#.######...#..#.##.#.....##.#....#...#.############..#.#.###.####.#.#..#.###....#.
..###..#...##.#.##.##..######.###.###..#..#####....#.#....####.#..#.......##..#####.#...#.#.##....#.
.#.##...#..#.##.####......#.#......######.#.#.##.#..##..#....##..##....#.##.###..#.##..#.#.....##...
###..###.########..####.###.#...###.........##....##.##..#...#.##..##..#.#.####....######..#..#...##
##.#..###..#.#..#...##.####.#..##..##...###.##.#......#........###...#.###..####..####..#####..###..
#.##...##..##..#.#.###..###...###.....#.#.######.....##.....#.##..##.#...###.###.#.#..#.##.##.##.#..
#..#.#####..#..#...#.#...#..##.#..#.###...#..###.###.#.#...#.###..#.###..#..##.##......##...#...###.
#....#.##.....#.#...###....#..#.#.#..##.##..###.###..#..#.#..##.....###.#.#.#...#.##.#..##....####..
#..#####...#...#####.###.#..#..#...####.##.#.#..#...#.####...##.##.....#.#.##.##.##.#....#.######..#
###...##..##.##.##.#..##.##.#.##.##.#..#.#####..#.#..#.#..####....#.###..###...####.#....#...#.###.#
##...####...##.######...#...#..##..###..###..#.##.#.#.#...##.#.###...#...########..#.#######...###..
.###...#.#.####.#.#...##.#.###.#.###.#####.###..##.#..###..#.#...##...#####..##....#..#..##.#.#....#
...#..#....#..#....#.#.##..#..#####....#..#.#..####..####.####.###......###..#..#..###.##.###...###.
#.#...#.###.###....#..###..###...#..#####..#.##........###....###.###.##.###.#..##..#.....#.#.####.#
#...#.#...#.#..#..#.##.##.#......#.#.#..###....#.#.#.#..#.#.###..#..#..##.#.#.#.##...#..###.####..#.
#.#.##.####........#.##..#.#.#...##..#.#####.#..#.#..#.##.#..####.###.#####.#.##.#....##..#####.####
#######.##..#.#..#...#..#..#.######..##.#.###.#...#..####..##.#.##.###.#.##.#.##..#...#.#..#.#.####.
.##..#####.##.####.##.#....##.....###.......#.####.###....#...#.#####.###.#..#...##.#..##.#....###.#
..####.#.#.....#...###.#...#.#...#.####..##..#..######......#...#.##.####..#.......#.##.......#.###.
.##..#.#####..##..##.#..####.##.##.#.###.#.#.....#.....#.#...####....##.##...####......##.##.#######
#.#.#..###..###..##.#..##...###..###..#..#.###.#..##.##...##..#..####....####.##.#.###...#..##.#.##.
##.##.....######.#..##.#.#.###.##.###.##....######.##.#.##..#..##...#...#..#.#.#.######..#.###.##..#
#..#.#.###.#.#.#.....###.######..##.##.#.#...#####.#...##.##.##.##....##....#..#.##.#.##..##...#...#
..#.....#....#...#######..#.#.#.#.###.##.#.##.###..#..##..###..#.#####..###.......####.#..####..#...
#..#.#...##.##..#.#.....###.##.#.#....###....#.##..#....#####.##.#####..###...####.#.#.#.###.#....##
#....##..#..##..#.####..##.##....####..##..#..####.######.##..#.###.#.##....###.#....##....##.#.##.#
.#.#.#.##.##...###.#.#...#...#..######.##.#.#.##.####.....#.#..#.######.......##.#####..#....#####.#
.###.#.##...###.....##..#.#..##....#.###..##.##..####..##.#.###..##.#.#...##.#####.#.....#..#..#####
#.#.###..#....####....#...#.###.#..#...###..###......##..##.#.#..#....#......###...##..#.##.##...#.#
###.####.##.#.##.......##.....#.#.###......##..#.#....###..#.#.##.###.##.##.##.#..#.####.#......###.
####...#...##...#.#.#..######.#####..##...#.##.##.####.##.#.##..#..#.#.##.#...#......#.##.#######...
.##...#....#.##.#...#..#..#.####..#.#..........#.#.##......#.##.######.###.#####.#.#####.###.####...
.##.##.##.###..###..##.#..#.###..#####.#..##.##.#.####....###.#..###...##..##..#.##..#...#..###.##.#
#.....##..#...##.#...#.###.##..#.....##..#.##...#.##...#.#.##.#...#.#..###...#...#######...#.#.....#
.######..#.####.#.##.##.#.#.#.#.###.#######...#..####.#.#..#####..#.....###.##...#.###.####.#.#.....
#.####..##...####.##.#.##...####..##.#.#..####.##.#.###.##..##.##..###.##...##........###..#.#.##...
#...#.##.....###.##.....####.####.##.##.##....##.#.#.#.##...#.#.##..#..##.....##.#...##..##...##..##
#..#.###.#....#.##...##...#.#......##.###...#......##..#..##...##.#..###.#..#.#.#.##.........##.#.##
.#.#..#..#.#..##...#.#...#..###.#...#.#.##.##....#.##.#...#.##.#..#.#..##..#.##...#.#.#..###.#.###..
###...#.#..##.###...#..##.#..##.#.######..####..#.#.....##....##.####.###...#.#...#.#..#..#.###....#
.####.###..##.##...#.....##.###.#..##..####.....#....##.....#....##..##...##....#..#..###.#...###.##
..###..###.#.#....#.##........#...####...####...#.####.##.###.##...#.####.#.###..###.##...##...#.#.#
###.#..#.#...#.#.#.....##.###...#..###.#....#.##.##.###..#######.###.#...###.####...#.#.###.#...####
#.#....####..#.#.###..###.###.#.#.##...###.#..###.#.#.#...###..#...##...#..#..###..##.#..#.##...###.
.###..##.#..#.#.#####.#..#.........##..##...#...#.#...##..#.##...###..#.#.#..####....#.####.##.#....
##.....#######....#.###.##...##.##.#####.#....#########.##....##.###...#.###..##.###.#..##..#....#.#
...#.##.#.......#.#....#..##..#..##...#.#.####...###.##..##...##.#.##.#.###.###.###..####..#######.#
.#.###...##.##.#.......#.#...#..#####...##...#...#.#####.##.#......#.##...#.###.#.#..#.#..##..##.#.#
...##...##.#.#...#...#.###..###....#.##...##.....#...##..##....#..##..#.#..#.#.#.........##.#####..#
........#...#.......#.#..#..#......##..##...##......#.....#.#.......#.#####.#.#.#..#...#####.#..####
#.#.###...###....#...######...#.#.#####.##..#..##.##..#..#..#..##...###..#.#.#..##...#.#.#.#.####..#
.#..#...##...#....#.#.##.#.#....##.#....#...#.#.##.....#########.#....#.##..#..#.#.##..#.#..###....#
##..#.#.#..###.....###.#..#.##..##....###.#..####.#.#.#..######..###..#.#.#...#...#...#.#..#.#..###.
.#.##.#.####....#..#.#...#.##......#..#.....#...#....#.##########..#...#.#.###...####..###......##.#
###...#....###.#.#..#...#..####.#...#.##..#.##.#...###.#.#..#.###.###...#......#..#.##.#..#####...##
.###.##.#..###...#..####.###...#.##..##....######.#..##...#...#.##.....#.####.########.##....#.##.##
.......######.#...#...#.###....##..#.###.#...##.##..###.#..##...#..########.#.#.#...#..#..##.##..#.#
..###.##..#..#####.#.##.#..#...##..#.#..###.###....###...#..##......###..#....#.#.....#...#..##.#..#
###..##..#.#....#####.#######..#..##...#..#...#.#........#...###.#.###..##.####..#.####.#..#..#...#.
...#..###.#.#.#.###...#.......####..###.#.##.####.#..#.#..#########.#.....#...##..#.#.#..#.#.#.#...#
####.#..##.##..#..###.######.#.#.###...#####.#.###.#####.#.#.#.#...#.#.#...#.#########.#..###.##.###
.##...#.#.###....##..##..#.#.#.#.#..#..##.###...####..#..#.#.#.#####..##.#.#.##.##...#.....#...###.#", 50, 16014);
        }
    }
}