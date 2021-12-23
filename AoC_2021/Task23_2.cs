using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task23_2
    {
        private static int maxY;

        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        //[Ignore("dsad")]
        public void Solve(string input, int expected)
        {
            var current = ParseMap(input);

            var opened = new HashSet<Map>();
            var weights = new Dictionary<Map, int>
            {
                [current] = 0
            };

            var cache = new ConcurrentDictionary<Map, (Map Map, int Cost)[]>();

            var notOpenWeights = new Dictionary<Map, int> { { current, 0 } };
            var cnt = 0;
            while (true)
            {
                //if (++cnt >= 500) break;

                var allNextMaps = cache.GetOrAdd(current, map => map.GetPossibleMoves().ToArray());

                var nextMaps = allNextMaps
                    .Where(x => !opened.Contains(x.Map))
                    .ToArray();

                var currentWeight = weights[current];
                foreach (var nextMap in nextMaps)
                {
                    if (!weights.TryGetValue(nextMap.Map, out var w) || w > currentWeight + nextMap.Cost)
                    {
                        weights[nextMap.Map] = currentWeight + nextMap.Cost;
                        notOpenWeights[nextMap.Map] = currentWeight + nextMap.Cost;
                    }
                }

                opened.Add(current);
                notOpenWeights.Remove(current);

                if (current.Pods.All(x => x.IsInTarget))
                {
                    break;
                }

                current = notOpenWeights.OrderBy(x => x.Value).Select(x => x.Key).First();
            }

            weights[current].Should().Be(expected);
        }

        private static Map ParseMap(string input)
        {
            var lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            var mapChars = lines.Select(x => x.ToCharArray()).ToArray();

            var pods = new List<Pod>();
            for (var y = 1; y < mapChars.Length - 1; ++y)
            for (var x = 1; x <= 11; ++x)
            {
                var letter = mapChars[y][x];

                if (!char.IsLetter(letter)) continue;

                var pod = new Pod
                {
                    Letter = letter,
                    Point = (y - 1, x - 1)
                };
                pods.Add(pod);
            }

            maxY = pods.Select(x => x.Point.Y).Max();

            var current = new Map(pods);
            return current;
        }

        [DebuggerDisplay("{MapStr}")]
        class Map
        {
            private readonly IReadOnlyList<Pod> podsOrderedList;
            private readonly Dictionary<int, Pod[]> rooms;
            private readonly Dictionary<(int Y, int X), Pod> podByPoints;
            private static readonly HashSet<int> denyZeroYXs = new() { 2, 4, 6, 8 };
            private readonly string strView;

            private bool RoomIsFree(int xRoom)
            {
                return rooms.SafeGet(xRoom, Array.Empty<Pod>()).All(x => x.IsInTargetX);
            }

            public string MapStr
            {
                get
                {
                    var sb = new StringBuilder();
                    for (var y = 0; y <= maxY; ++y)
                    {
                        for (var x = 0; x < 11; ++x)
                        {
                            if (y == 0)
                            {
                                sb.Append(podByPoints.TryGetValue((y, x), out var p1) ? p1.Letter : '.');
                                continue;
                            }

                            if (podByPoints.TryGetValue((y, x), out var p))
                            {
                                sb.Append(p.Letter);
                                continue;
                            }

                            sb.Append(x % 2 == 0 ? '.' : '#');
                        }

                        sb.AppendLine();
                    }

                    return sb.ToString();
                }
            }

            public Map(IEnumerable<Pod> pods)
            {
                Pods = ImmutableHashSet.Create(pods.ToArray());
                podsOrderedList = Pods.OrderBy(x => x.Point.X).ThenBy(x => x.Point.Y).ToList();
                rooms = Pods.Where(x => x.IsInRoom).GroupBy(x => x.Point.X)
                    .ToDictionary(x => x.Key, x => x.OrderBy(p => p.Point.Y).ToArray());
                podByPoints = Pods.ToDictionary(x => x.Point, x => x);

                strView = string.Join(string.Empty, podsOrderedList.Select(x => $"{x.Point.Y}{x.Point.X}{x.Letter}"));
            }

            private bool Equals(Map other)
            {
                return strView == other.strView;
            }

            public override bool Equals(object obj)
            {
                return obj is Map other && Equals(other);
            }

            public override int GetHashCode()
            {
                return strView.GetHashCode();
            }

            public ImmutableHashSet<Pod> Pods { get; }
            private IEnumerable<Pod> MovablePods => Pods.Where(IsMovable);

            private (Map Map, int Cost) getNewMap(Pod oldPod, int y, int x)
            {
                var newPod = oldPod;
                newPod.Point.Y = y;
                newPod.Point.X = x;

                return (new Map(new[] { newPod }.Concat(Pods.Where(p => !p.Equals(oldPod)))),
                    oldPod.GetPathCost(newPod.Point));
            }

            public IEnumerable<(Map Map, int Cost)> GetPossibleMoves()
            {
                //if (targetRoom.All(x => x.IsInTargetX))
                //{
                //    var newPod = pod;
                //    newPod.Point.Y = 0;

                //    yield return (new Map(new[] { newPod }.Concat(podsCopy.Where(p => !p.Equals(pod)))),
                //        pod.GetPathCost(newPod.Point));
                //    continue;
                //}

                var movablePods = MovablePods.ToArray();
                var sureMove = movablePods.Select(x => (Pod: x, Move: GetSureMove(x)))
                    .FirstOrDefault(x => x.Move != null);
                if (sureMove.Move != null)
                {
                    yield return getNewMap(sureMove.Pod, sureMove.Move.Value.Y, sureMove.Move.Value.X);
                    yield break;
                }

                foreach (var pod in movablePods)
                {
                    if (!pod.IsInRoom) continue;

                    var room = rooms.SafeGet(pod.Point.X, Array.Empty<Pod>());
                    if (room.Any(x => x.Point.Y < pod.Point.Y))
                        continue;
                    if (pod.IsInTargetX && room.Where(x => x.Point.Y > pod.Point.Y).All(x => x.IsInTargetX))
                        continue;

                    for (var x = pod.Point.X + 1; !podByPoints.ContainsKey((0, x)) && x <= 10; ++x)
                    {
                        if (denyZeroYXs.Contains(x)) continue;
                        yield return getNewMap(pod, 0, x);
                    }

                    for (var x = pod.Point.X - 1; !podByPoints.ContainsKey((0, x)) && x >= 0; --x)
                    {
                        if (denyZeroYXs.Contains(x)) continue;
                        yield return getNewMap(pod, 0, x);
                    }
                }
            }

            private bool IsMovable(Pod pod)
            {
                var room = rooms.SafeGet(pod.Point.X, Array.Empty<Pod>());
                if (room.Any(x => x.Point.Y < pod.Point.Y))
                    return false;

                if (pod.IsInTargetX && room.Where(x => x.Point.Y > pod.Point.Y).All(x => x.IsInTargetX))
                    return false;

                return true;
            }

            private (int Y, int X)? GetSureMove(Pod pod)
            {
                if (!RoomIsFree(pod.TargetX)) return null;

                var dx = Math.Sign(pod.TargetX - pod.Point.X);
                for (var x = pod.Point.X + dx;; x += dx)
                {
                    if (podByPoints.ContainsKey((0, x))) return null;
                    if (x == pod.TargetX) break;
                }

                if (pod.IsInHall)
                {
                    return (
                        rooms.SafeGet(pod.TargetX)?.Select(x => x.Point.Y).Min() - 1 ?? maxY,
                        pod.TargetX
                    );
                }

                return (0, pod.Point.X + dx);
            }
        }

        [DebuggerDisplay("{Letter} {Point}")]
        private struct Pod
        {
            public char Letter;
            public (int Y, int X) Point;

            private int Cost =>
                Letter switch
                {
                    'A' => 1,
                    'B' => 10,
                    'C' => 100,
                    'D' => 1000,
                    _ => throw new ArgumentOutOfRangeException(nameof(Letter))
                };

            public bool IsInRoom => Point.Y >= 1;
            public bool IsInHall => !IsInRoom;
            public bool IsInTargetX => Point.X == TargetX;
            public bool IsInTarget => IsInTargetX && IsInRoom;


            public int GetPathCost((int Y, int X) newPoint)
            {
                return (Math.Abs(newPoint.Y - Point.Y) + Math.Abs(newPoint.X - Point.X)) * Cost;
            }

            public int TargetX => Letter switch
            {
                'A' => 2,
                'B' => 4,
                'C' => 6,
                'D' => 8,
                _ => throw new ArgumentOutOfRangeException(nameof(Letter))
            };
        }

        [Test]
        [TestCaseSource(nameof(GenerateTestCasesMoves))]
        public void TestGetMoves(string input, int expected, int? expectedCost = null)
        {
            var map = ParseMap(input);
            var possibleMoves = map.GetPossibleMoves().ToArray();
            possibleMoves.Length.Should().Be(expected);

            if (expectedCost != null)
            {
                possibleMoves.Sum(x => x.Cost).Should().Be(expectedCost.Value);
            }
        }

        private static IEnumerable<TestCaseData> GenerateTestCasesMoves()
        {
            yield return new TestCaseData(@"#############
#...........#
###B#C#A#D###
  #########  ", 21, 3370);

            yield return new TestCaseData(@"#############
#...........#
###A#C#B#D###
  #########  ", 14, null);

            yield return new TestCaseData(@"#############
#...........#
###A#B#C#D###
  #########  ", 0, null);

            yield return new TestCaseData(@"#############
#A..........#
###.#B#C#D###
  #########  ", 1, 3);

            yield return new TestCaseData(@"#############
#A....B.....#
###.#.#C#D###
  #########  ", 1, null);

            yield return new TestCaseData(@"#############
#A....B.....#
###.#C#.#D###
  #########  ", 1, 3);

            yield return new TestCaseData(@"#############
#A.........B#
###.#C#.#D###
  #########  ", 1, 3);

            yield return new TestCaseData(@"#############
#C..........#
###.#A#B#D###
  #########  ", 1, 2);

            yield return new TestCaseData(@"#############
#...C.......#
###.#A#B#D###
  #########  ", 4 * 2, null);
            yield return new TestCaseData(@"#############
#.......C...#
###.#D#B#A###
  #########  ", 10, null);
            yield return new TestCaseData(@"#############
#...........#
###B#C#B#D###
  #A#D#C#A#  
  #########  ", 7 + 7 + 7 + 7
                , 20 + 30 + 20 + 40 + 60 + 80 + 90 + 200 + 400 + 500 + 200 + 400 + 600 + 700 + 20 + 40 + 60 + 70 + 20 +
                  40 +
                  50 + 2000 + 3000 + 2000 + 4000 + 6000 + 8000 + 9000);
            yield return new TestCaseData(@"#############
#...B...B.D.#
###.#C#.#.###
  #A#D#C#A#  
  #########  ", 1, 200);
            yield return new TestCaseData(@"#############
#.B.C...B.D.#
###.#.#.#.###
  #A#D#C#A#  
  #########  ", 1, 400);
            yield return new TestCaseData(@"#############
#AB.C...B.AD#
###.#.#.#.###
###A#D#C#D###
  #A#B#C#A#  
  #########  ", 1, 400);
            yield return new TestCaseData(@"#############
#AB.D...B.AD#
###.#.#.#.###
###A#C#C#D###
  #A#B#C#A#  
  #########  ", 1, 300);
            yield return new TestCaseData(@"#############
#.......C..A#
###.#D#B#.###
  #########  ", 8, 20 + 40 + 60 + 70 + 2000 + 2000 + 4000 + 5000);
        }


        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"#############
#...........#
###B#C#A#D###
  #########  ", 448);
            yield return new TestCaseData(@"#############
#...........#
###B#C#B#D###
  #A#D#C#A#  
  #########  ", 12521);
            yield return new TestCaseData(@"#############
#...........#
###B#B#D#D###
  #C#C#A#A#  
  #########  ", 10411);
            yield return new TestCaseData(@"#############
#...........#
###B#C#B#D###
  #D#C#B#A#  
  #D#B#A#C#  
  #A#D#C#A#  
  #########  ", 44169);
            yield return new TestCaseData(@"#############
#...........#
###B#B#D#D###
  #D#C#B#A#  
  #D#B#A#C#  
  #C#C#A#A#  
  #########   ", 46721);
        }
    }
}