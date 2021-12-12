using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task12
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int expected)
        {
            var lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            var map = new Dictionary<string, List<string>>();

            foreach (var line in lines)
            {
                var splits = line.Split("-", StringSplitOptions.RemoveEmptyEntries);
                if (splits[0] != "end" && splits[1] != "start")
                {
                    map[splits[0]] = map.TryGetValue(splits[0], out var list)  ? list : new List<string>();
                    map[splits[0]].Add(splits[1]);
                }

                if (splits[1] != "end" && splits[0] != "start")
                {
                    map[splits[1]] = map.TryGetValue(splits[1], out var list)  ? list : new List<string>();
                    map[splits[1]].Add(splits[0]);
                }
            }

            var pathCount = 0;
            Dfs(map, "start", ref pathCount);
            pathCount.Should().Be(expected);
        }

        private void Dfs(Dictionary<string, List<string>> map, string current, ref int pathCount, HashSet<string> visited = null)
        {
            visited ??= new HashSet<string>();

            if (current == "end")
            {
                ++pathCount;
                return;
            }

            var children = map[current];
            foreach (var child in children)
            {
                if (visited.Contains(child))
                    continue;

                if (child.All(char.IsLower)) visited.Add(child);

                Dfs(map, child, ref pathCount, visited);

                if (child.All(char.IsLower)) visited.Remove(child);
            }
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"start-A
start-b
A-c
A-b
b-d
A-end
b-end", 10);
            yield return new TestCaseData(@"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc", 19);
            yield return new TestCaseData(@"fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW", 226);
            yield return new TestCaseData(@"yb-pi
jg-ej
yb-KN
LD-start
end-UF
UF-yb
yb-xd
qx-yb
xd-end
jg-KN
start-qx
start-ej
qx-LD
jg-LD
xd-LD
ej-qx
end-KN
DM-xd
jg-yb
ej-LD
qx-UF
UF-jg
qx-jg
xd-UF", 4011);
        }
    }
}