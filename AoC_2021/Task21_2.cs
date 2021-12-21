using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
                NextTurnIsPlayer1 = true
            };
            
            var wins = Turn(game, winScore);
            var max = Math.Max(wins.Win1, wins.Win2);
            max.Should().Be(expected);
        }

        private static (long Win1, long Win2) Turn(Game game, int winScore)
        {
            if (game.Player1Score >= winScore)
            {
                return (1, 0);
            }

            if (game.Player2Score >= winScore)
            {
                return (0, 1);
            }

            var turns = new List<(long Win1, long Win2)>(3);
            for (var die = 1; die <= 3; ++die)
            {
                var newGame = game;
                newGame.NextTurnIsPlayer1 = !newGame.NextTurnIsPlayer1;
                if (game.NextTurnIsPlayer1)
                {
                    newGame.Player1Position = (game.Player1Position - 1 + die) % 10 + 1;
                    newGame.Player1Score += newGame.Player1Position;
                }
                else
                {
                    newGame.Player2Position = (game.Player2Position - 1 + die) % 10 + 1;
                    newGame.Player2Score += newGame.Player2Position;
                }

                var turn = Turn(newGame, winScore);
                turns.Add(turn);
            }

            return (turns.Select(x => x.Win1).Sum(), turns.Select(x => x.Win2).Sum());
        }

        struct Game
        {
            public int Player1Position;
            public int Player2Position;
            public long Player1Score;
            public long Player2Score;
            public bool NextTurnIsPlayer1;
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