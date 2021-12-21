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

        private static (long Win1, long Win2) Turn(Game game, int winScore,
            Dictionary<Game, (long Win1, long Win2)> cache = null)
        {
            cache ??= new Dictionary<Game, (long Win1, long Win2)>();

            if (cache.TryGetValue(game, out var v))
            {
                return v;
            }

            if (game.Player1Score >= winScore)
            {
                return (1, 0);
            }

            if (game.Player2Score >= winScore)
            {
                return (0, 1);
            }

            var nextGames = new List<Game>(27);
            for (var i = 1; i <= 3; ++i)
            for (var j = 1; j <= 3; ++j)
            for (var k = 1; k <= 3; ++k)
            {
                var newGame = game;
                var die = i + j + k;

                if (game.TurnIsPlayer1)
                {
                    newGame.Player1Position = (game.Player1Position - 1 + die) % 10 + 1;
                    newGame.Player1Score += newGame.Player1Position;
                }
                else
                {
                    newGame.Player2Position = (game.Player2Position - 1 + die) % 10 + 1;
                    newGame.Player2Score += newGame.Player2Position;
                }

                newGame.TurnIsPlayer1 = !game.TurnIsPlayer1;

                nextGames.Add(newGame);
            }

            var win1 = 0L;
            var win2 = 0L;
            foreach (var nextGame in nextGames.OrderByDescending(x => Math.Max(x.Player1Score, x.Player2Score)))
            {
                var turn = Turn(nextGame, winScore, cache);
                cache[nextGame] = turn;
                win1 += turn.Win1;
                win2 += turn.Win2;
            }

            return (win1, win2);
        }

        struct Game
        {
            public int Player1Position;
            public int Player2Position;
            public long Player1Score;
            public long Player2Score;
            public bool TurnIsPlayer1;
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(0, 0, 2, 5L);
            yield return new TestCaseData(0, 0, 3, 14L);
            yield return new TestCaseData(4, 8, 21, 444356092776315L);
            yield return new TestCaseData(7, 5, 21, 0L);
        }
    }
}