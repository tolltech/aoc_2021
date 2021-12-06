using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task06
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int days, int expected)
        {
            var fishes = input.Split(",").Select(int.Parse).ToList();

            for (var day = 0; day < days; ++day)
            {
                var fishesCount = fishes.Count;
                for (var i = 0; i < fishesCount; ++i)
                {
                    if (fishes[i] == 0)
                    {
                        fishes.Add(8);
                        fishes[i] = 6;
                    }
                    else
                    {
                        --fishes[i];
                    }
                }

                var s = string.Join(",", fishes);
            }

            fishes.Count.Should().Be(expected);
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"3,4,3,1,2", 18, 26);
            yield return new TestCaseData(@"3,4,3,1,2", 80, 5934);
            yield return new TestCaseData(
                @"1,2,5,1,1,4,1,5,5,5,3,4,1,2,2,5,3,5,1,3,4,1,5,2,5,1,4,1,2,2,1,5,1,1,1,2,4,3,4,2,2,4,5,4,1,2,3,5,3,4,1,1,2,2,1,3,3,2,3,2,1,2,2,3,1,1,2,5,1,2,1,1,3,1,1,5,5,4,1,1,5,1,4,3,5,1,3,3,1,1,5,2,1,2,4,4,5,5,4,4,5,4,3,5,5,1,3,5,2,4,1,1,2,2,2,4,1,2,1,5,1,3,1,1,1,2,1,2,2,1,3,3,5,3,4,2,1,5,2,1,4,1,1,5,1,1,5,4,4,1,4,2,3,5,2,5,5,2,2,4,4,1,1,1,4,4,1,3,5,4,2,5,5,4,4,2,2,3,2,1,3,4,1,5,1,4,5,2,4,5,1,3,4,1,4,3,3,1,1,3,2,1,5,5,3,1,1,2,4,5,3,1,1,1,2,5,2,4,5,1,3,2,4,5,5,1,2,3,4,4,1,4,1,1,3,3,5,1,2,5,1,2,5,4,1,1,3,2,1,1,1,3,5,1,3,2,4,3,5,4,1,1,5,3,4,2,3,1,1,4,2,1,2,2,1,1,4,3,1,1,3,5,2,1,3,2,1,1,1,2,1,1,5,1,1,2,5,1,1,4",
                80, 356190);
        }
    }
}