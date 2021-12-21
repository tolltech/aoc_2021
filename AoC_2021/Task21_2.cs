using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task21_2
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(int start1, int start2, int winScore, long expected)
        {
            var game = new Game
            {
                Player1Position = start1,
                Player2Position = start2,
                Player1Score = 0,
                Player2Score = 0,
                TurnIsPlayer1 = true
            };

            var wins = Turn(game, winScore);
            var max = Math.Max(wins.Win1, wins.Win2);
            max.Should().Be(expected);
        }

        private static (long Win1, long Win2) Turn(Game game, int winScore)
        {
            var cache = new Dictionary<Game, long> { { game, 1 } };

            var nextGames = new PriorityQueue<Game, long>();
            nextGames.Enqueue(game, 0);

            while (nextGames.Count > 0)
            {
                var currentGame = nextGames.Dequeue();
                var currentCached = cache[currentGame];

                for (var i = 1; i <= 3; ++i)
                for (var j = 1; j <= 3; ++j)
                for (var k = 1; k <= 3; ++k)
                {
                    var newGame = currentGame;
                    var die = i + j + k;

                    if (currentGame.TurnIsPlayer1)
                    {
                        newGame.Player1Position = (currentGame.Player1Position - 1 + die) % 10 + 1;
                        newGame.Player1Score += newGame.Player1Position;
                    }
                    else
                    {
                        newGame.Player2Position = (currentGame.Player2Position - 1 + die) % 10 + 1;
                        newGame.Player2Score += newGame.Player2Position;
                    }

                    newGame.TurnIsPlayer1 = !currentGame.TurnIsPlayer1;

                    if (cache.ContainsKey(newGame))
                    {
                        cache[newGame] += currentCached;
                    }
                    else
                    {
                        cache[newGame] = currentCached;

                        if (newGame.MaxScore < winScore)
                            nextGames.Enqueue(newGame, newGame.Player1Score + newGame.Player2Score);
                    }
                }
            }

            return (
                cache.Where(x => x.Key.Player1Score >= winScore).Sum(x => x.Value),
                cache.Where(x => x.Key.Player2Score >= winScore).Sum(x => x.Value)
            );
        }

        struct Game
        {
            public int Player1Position;
            public int Player2Position;
            public long Player1Score;
            public long Player2Score;
            public bool TurnIsPlayer1;

            public long MaxScore => Player1Score > Player2Score ? Player1Score : Player2Score;
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(4, 8, 21, 444356092776315L);
            yield return new TestCaseData(7, 5, 21, 809953813657517L);
        }
    }
}