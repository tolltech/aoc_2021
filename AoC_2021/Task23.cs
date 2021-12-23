using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task23
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int expected)
        {
            var lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            var map = lines.Select(x => x.ToCharArray()).ToArray();


        }

        private struct Amphipod
        {
            public char Letter;
            public (int Y, int X) Point;
            public int Cost =>
                Letter switch
                {
                    'A' => 1,
                    'B' => 10,
                    'C' => 100,
                    'D' => 1000,
                    _ => throw new ArgumentOutOfRangeException(nameof(Letter))
                };

            public bool IsInTargetY => Point.Y >= 2;
            public bool IsInTargetX => Point.X == TargetX;
            public bool IsInTarget => IsInTargetX && IsInTargetY;
            public bool IsInRoom => Point.Y >= 2;


            public int TargetX => Letter switch
            {
                'A' => 3,
                'B' => 5,
                'C' => 7,
                'D' => 9,
                _ => throw new ArgumentOutOfRangeException(nameof(Letter))
            };
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
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
        }

//#############
//#...........#
//###B#B#D#D###
//#C#C#A#A#
//#########
  
//############# 5003
//#.........a.#
//###B#B#D#.###
//#C#C#A#d#
//#########
  
//############# 9006
//#.......a.a.#
//###B#B#.#d###
//#C#C#.#d#
//#########
  
//############# 9656
//#.......a.a.#
//###B#.#.#d###
//#C#b#c#d#
//#########
  
//############# 10396
//#.......a.a.#
//###.#b#c#d###
//#.#b#c#d#
//#########
  
//############# 10411
//#...........#
//###a#b#c#d###
//#a#b#c#d#
//#########
    }
}