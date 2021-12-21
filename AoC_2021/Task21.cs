using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task21
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(int start1, int start2, int dieCount, long expected)
        {
            var playerPositions = new[] { start1, start2 };
            var totalScores = new[] { 0L, 0L };
            var playerIndex = 0;

            var die = 0;
            var totalDie = 0L;

            while (true)
            {
                var step = die % dieCount + 1 +
                           (die + 1) % dieCount + 1 +
                           (die + 2) % dieCount + 1;

                var currentScore = (playerPositions[playerIndex] - 1 + step) % 10 + 1;
                playerPositions[playerIndex] = currentScore;
                totalScores[playerIndex] += currentScore;

                totalDie += 3;
                die = (die + 3) % dieCount;
                playerIndex = (playerIndex + 1) % 2;

                if (totalScores.Any(x => x >= 1000))
                {
                    (totalScores[playerIndex] * totalDie).Should().Be(expected);
                    return;
                }
            }
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(4, 8, 100, 739785);
            yield return new TestCaseData(7, 5, 100, 798147);
        }
    }
}