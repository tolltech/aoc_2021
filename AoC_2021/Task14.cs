using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task14
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int step, long expected)
        {
            var lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            var polymerStr = lines[0];

            var polymer = new Dictionary<(char, char), long>();
            for (var i = 0; i < polymerStr.Length - 1; ++i)
            {
                var c1 = polymerStr[i];
                var c2 = polymerStr[i + 1];
                polymer[(c1, c2)] = polymer.TryGetValue((c1, c2), out var v) ? v + 1 : 1;
            }

            var letters = polymerStr.GroupBy(x => x).ToDictionary(x => x.Key, x => (long)x.Count());

            var rules = new Dictionary<(char, char), char>();
            foreach (var line in lines.Skip(1))
            {
                var splits = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
                rules[(splits[0][0], splits[0][1])] = splits[1][0];
            }

            for (var i = 0; i < step; ++i)
            {
                var newPairs = new Dictionary<(char, char), long>();
                var removedPairs = new Dictionary<(char, char), long>();;
                foreach (var rule in rules)
                {
                    var pair = rule.Key;
                    var newChar = rule.Value;

                    if (!polymer.TryGetValue(pair, out var currentCount))
                    {
                        continue;
                    }

                    removedPairs[pair] = removedPairs.TryGetValue(pair,out var rP) ? rP + currentCount : currentCount;

                    var newPair1 = (pair.Item1, newChar);
                    newPairs[newPair1] = newPairs.TryGetValue(newPair1, out var v1) ? v1 + currentCount : currentCount;
                    var newPair2 = (newChar, pair.Item2);
                    newPairs[newPair2] = newPairs.TryGetValue(newPair2, out var v2) ? v2 + currentCount : currentCount;

                    letters[newChar] = letters.TryGetValue(newChar, out var charCount) ? charCount + currentCount : currentCount;
                }

                foreach (var newPair in newPairs)
                {
                    polymer[newPair.Key] = polymer.TryGetValue(newPair.Key, out var v) ? v + newPair.Value : newPair.Value;
                }

                foreach (var pair in removedPairs)
                {
                    polymer[pair.Key] = polymer.TryGetValue(pair.Key, out var p) ? p - pair.Value : 0;
                    if (polymer[pair.Key] <= 0)
                    {
                        polymer.Remove(pair.Key);
                    }
                }
            }

            (letters.Values.OrderByDescending(x => x).First()
             - letters.Values.OrderBy(x => x).First())
                .Should().Be(expected);
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C", 10, 1588);
            yield return new TestCaseData(@"SHHNCOPHONHFBVNKCFFC

HH -> K
PS -> P
BV -> H
HB -> H
CK -> F
FN -> B
PV -> S
KK -> F
OF -> C
SF -> B
KB -> S
HO -> O
NH -> N
ON -> V
VF -> K
VP -> K
PH -> K
FF -> V
OV -> N
BO -> K
PO -> S
CH -> N
FO -> V
FB -> H
FV -> N
FK -> S
VC -> V
CP -> K
CO -> K
SV -> N
PP -> B
BS -> P
VS -> C
HV -> H
NN -> F
NK -> C
PC -> V
HS -> S
FS -> S
OB -> S
VV -> N
VO -> P
KV -> F
SK -> O
BC -> P
BP -> F
NS -> P
SN -> S
NC -> N
FC -> V
CN -> S
OK -> B
SC -> N
HN -> B
HP -> B
KP -> B
CB -> K
KF -> C
OS -> B
BH -> O
PN -> K
VN -> O
KH -> F
BF -> H
HF -> K
HC -> P
NP -> H
KO -> K
CF -> H
BK -> O
OH -> P
SO -> F
BB -> F
VB -> K
SP -> O
SH -> O
PK -> O
HK -> P
CC -> V
NB -> O
NV -> F
OO -> F
VK -> V
FH -> H
SS -> C
NO -> P
CS -> H
BN -> V
FP -> N
OP -> N
PB -> P
OC -> O
SB -> V
VH -> O
KS -> B
PF -> N
KN -> H
NF -> N
CV -> K
KC -> B", 10, 2549L);
        }
    }
}