using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task18
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int expected)
        {
            var lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            var trees = lines.Select(line => Parse(line)).ToArray();

            var currentTree = trees.First();
            foreach (var tree in trees.Skip(1))
            {
                currentTree = Add(currentTree, tree);

                while (true)
                {
                    if (!Reduce(currentTree)) break;
                }
            }

            Magnitude(currentTree).Should().Be(expected);
        }

        private bool Reduce(Node tree)
        {
            if (Explode(tree)) return true;
            if (Split(tree)) return true;

            return false;
        }

        private static bool Explode(Node tree)
        {
            var allNodes = SelectAll(tree).ToArray();
            var mostLeft = allNodes.FirstOrDefault(x => x.Level == 4 && x.Node.IsRegular).Node;
            if (mostLeft == null)
            {
                return false;
            }

            var leftRegular = allNodes.TakeWhile(x => x.Node != mostLeft.Left).LastOrDefault(x => x.Node.Value.HasValue);
            if (leftRegular.Node != null) leftRegular.Node.Value += mostLeft.Left.Value;

            var rightRegular = allNodes.SkipWhile(x => x.Node != mostLeft.Right).Skip(1)
                .FirstOrDefault(x => x.Node.Value.HasValue);
            if (rightRegular.Node != null) rightRegular.Node.Value += mostLeft.Right.Value;

            mostLeft.Left = null;
            mostLeft.Right = null;
            mostLeft.Value = 0;

            return true;
        }

        private bool Split(Node tree)
        {
            var splitNode = SelectAll(tree).FirstOrDefault(x => x.Node.Value.HasValue && x.Node.Value.Value >= 10).Node;
            if (splitNode == null)
                return false;

            var leftValue = splitNode.Value / 2;
            var rightValue = splitNode.Value % 2 == 0 ? leftValue : leftValue + 1;

            splitNode.Left = new Node { Value = leftValue, Parent = splitNode };
            splitNode.Right = new Node { Value = rightValue, Parent = splitNode };
            splitNode.Value = null;

            return true;
        }

        private static IEnumerable<(Node Node, int Level)> SelectAll(Node node, int level = 0)
        {
            if (node.Left != null)
                foreach (var n in SelectAll(node.Left, level + 1))
                {
                    yield return n;
                }

            yield return (node, level);

            if (node.Right != null)
                foreach (var n in SelectAll(node.Right, level + 1))
                {
                    yield return n;
                }
        }

        private static Node Add(Node left, Node right)
        {
            var node = new Node { Left = left, Right = right };

            left.Parent = node;
            right.Parent = node;

            return node;
        }

        private static Node Parse(string line, Node parent = null)
        {
            var node = new Node
            {
                Parent = parent
            };

            if (int.TryParse(line, out var val))
            {
                node.Value = val;
                return node;
            }

            var newLine = line.Substring(1, line.Length - 2);

            var level = 0;
            var commaZeroLevelIndex = -1;
            for (var i = 0; i < newLine.Length; ++i)
            {
                var c = newLine[i];
                if (c == '[') ++level;
                if (c == ']') --level;

                if (c == ',' && level == 0)
                {
                    commaZeroLevelIndex = i;
                    break;
                }
            }

            var left = newLine.Substring(0, commaZeroLevelIndex);
            var right = newLine.Substring(commaZeroLevelIndex + 1);

            node.Left = Parse(left, node);
            node.Right = Parse(right, node);

            return node;
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]", 4140);
            yield return new TestCaseData(@"[[0,6],[[[4,0],[6,6]],[[2,2],9]]]
[[9,[[1,6],[6,0]]],[[1,[0,8]],[[0,8],[9,8]]]]
[[[0,[2,1]],3],[[[2,4],[1,2]],[7,5]]]
[[[[8,3],[8,5]],[[7,8],[5,5]]],[9,2]]
[[8,[1,9]],[[[9,9],[9,2]],1]]
[[[[3,7],[2,1]],[0,9]],4]
[[[[3,8],[6,0]],[0,7]],[[[6,3],[2,0]],9]]
[[[9,[7,0]],[8,[9,6]]],[[5,6],4]]
[[[[3,6],[3,6]],[0,2]],[[[8,3],9],[[3,4],8]]]
[[7,[8,4]],1]
[6,[[3,[5,6]],[0,6]]]
[[[7,[4,7]],[[4,5],[4,3]]],[[5,5],[0,[4,2]]]]
[[[0,[2,9]],[[2,4],[4,8]]],[[8,[9,5]],[[9,6],0]]]
[[[[2,0],[9,7]],[[3,2],0]],[7,7]]
[[5,[2,1]],[[3,[5,1]],[[8,5],[1,8]]]]
[[[[9,7],6],[[7,8],7]],[[[6,8],9],[[9,5],7]]]
[[4,2],[[[0,1],[7,2]],[[0,2],[5,5]]]]
[[1,8],[[5,[7,9]],[[3,1],[7,1]]]]
[[[4,[4,6]],6],5]
[[[5,[3,6]],6],[[[8,0],[8,6]],[[3,3],[0,1]]]]
[[4,[[2,6],[0,9]]],[[0,6],[4,2]]]
[[[[9,4],[6,5]],7],[[[1,5],[0,9]],[4,[4,2]]]]
[[7,[[6,5],8]],[[[5,6],0],[6,[3,5]]]]
[[[5,[6,4]],[8,[0,4]]],[[3,[9,3]],4]]
[[[[4,0],6],[6,[6,5]]],[[9,[6,3]],[[9,6],7]]]
[[[[2,2],4],[8,[7,2]]],[2,1]]
[5,[9,[[5,9],4]]]
[[[1,[7,7]],[[2,2],8]],[[[9,7],5],[4,3]]]
[[[[6,8],1],1],[1,[[2,0],6]]]
[[[[0,5],8],[[8,9],[9,3]]],[[[5,5],[4,2]],2]]
[[[9,[2,5]],[6,[1,7]]],[5,[3,[2,2]]]]
[[[7,6],8],[[[1,9],3],[5,2]]]
[8,[[2,[0,7]],8]]
[[[[8,1],[0,0]],5],1]
[[1,[[4,8],0]],[[9,[7,8]],5]]
[[[[1,3],1],[[9,8],[6,6]]],5]
[[[3,2],[[0,5],[0,1]]],[[9,[9,3]],[4,9]]]
[[[0,[2,4]],[[3,3],[6,5]]],[[1,[2,1]],[[3,4],9]]]
[[2,[3,[7,6]]],[5,5]]
[[[[8,2],0],[[9,6],[9,0]]],[[[6,2],[5,0]],9]]
[7,[9,7]]
[3,[[[5,5],1],[8,5]]]
[[[5,5],[5,6]],[9,5]]
[[[9,7],[1,2]],[8,[5,[7,0]]]]
[[[1,[5,2]],[7,[8,9]]],[2,[[4,5],[2,3]]]]
[[[4,[2,2]],[5,[4,7]]],[[[0,3],2],[5,[2,6]]]]
[[0,[[6,5],5]],[[7,[7,2]],3]]
[[[4,[9,4]],[1,9]],[7,[[7,1],[6,1]]]]
[1,[0,2]]
[[[[5,1],[2,1]],[[7,8],6]],[[3,[4,9]],2]]
[[9,[[4,0],[8,8]]],[[[6,6],[2,8]],[1,[1,5]]]]
[[[1,2],[7,0]],[7,[[3,0],5]]]
[[[6,[0,8]],3],[[3,7],1]]
[[[[6,1],[1,0]],9],[[4,8],[3,[0,8]]]]
[[6,[3,[5,8]]],9]
[[[[5,0],[7,7]],[[3,1],[4,8]]],5]
[[[3,7],[9,0]],[[[0,2],7],0]]
[8,9]
[[8,[[0,8],4]],[1,[[4,6],2]]]
[[[5,5],3],[[6,6],[0,[6,3]]]]
[[[7,[3,7]],[[6,1],[9,4]]],[[[8,9],1],[[8,7],6]]]
[[6,[[0,9],[2,3]]],[[1,[5,3]],[8,4]]]
[[[3,5],8],[[[2,4],[7,5]],5]]
[[0,[[7,0],[9,4]]],[[[0,0],[6,7]],[6,5]]]
[[[[1,9],[6,4]],0],[6,[3,[4,8]]]]
[[[[1,6],[0,4]],8],[[8,8],6]]
[[[[7,4],[9,6]],7],[[1,6],[1,0]]]
[1,[[[6,8],5],5]]
[8,4]
[9,[[9,[3,9]],0]]
[5,[[[4,9],7],[[1,0],0]]]
[[[6,1],[0,[2,3]]],[[[7,8],[5,9]],9]]
[3,[[3,[3,4]],[6,[7,8]]]]
[[[7,[7,1]],[4,[2,0]]],[6,[7,3]]]
[[6,9],[[3,[4,7]],3]]
[1,[[9,[5,1]],[7,[7,5]]]]
[[3,2],[[9,[6,8]],[[1,0],2]]]
[[[[3,2],8],[7,6]],9]
[[3,[[9,5],6]],[5,9]]
[[[3,[6,3]],[[7,0],[5,7]]],[[3,3],[[4,9],[4,8]]]]
[[[0,[4,3]],2],[3,[0,[1,3]]]]
[[[7,[3,4]],[7,[3,1]]],[[0,[4,7]],6]]
[[[1,[7,4]],[[8,7],3]],4]
[[[5,5],[[0,3],2]],[1,[[9,4],6]]]
[[[[6,0],[8,8]],[6,[6,0]]],[5,6]]
[[[1,[5,4]],[[5,9],[1,7]]],[[5,[4,7]],[4,[4,4]]]]
[[0,[[2,6],0]],[[6,[4,3]],5]]
[[[1,[5,3]],[9,[1,2]]],[[[4,8],[5,6]],0]]
[[0,7],[1,[7,7]]]
[4,[[7,[7,2]],[[9,1],7]]]
[2,[[1,6],[6,9]]]
[[[4,[4,5]],9],[[[1,7],6],[3,[7,3]]]]
[[6,[[1,1],[7,8]]],[[[5,2],[8,1]],5]]
[[[5,5],[[4,1],[1,2]]],[[3,8],[3,4]]]
[[[[1,9],[0,3]],[4,[0,9]]],4]
[[[4,9],0],[[9,0],[8,[7,5]]]]
[[6,[5,3]],[[[6,6],4],[[6,8],4]]]
[[[[1,1],2],1],[1,[[6,4],2]]]
[[[[6,3],[1,5]],[6,[7,7]]],[6,6]]
[[[[3,0],[5,6]],1],[[[9,3],[1,7]],[[3,4],[2,7]]]]", 4088);
        }

        public class Node
        {
            [JsonIgnore]
            public Node Parent { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public int? Value { get; set; }

            public bool IsRegular => !Value.HasValue && Left.Value.HasValue && Right.Value.HasValue;
        }

        [Test]
        [TestCase(@"[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384)]
        [TestCase(@"[[[[1,1],[2,2]],[3,3]],[4,4]]", 445)]
        [TestCase(@"[[[[3,0],[5,3]],[4,4]],[5,5]]", 791)]
        [TestCase(@"[[[[5,0],[7,4]],[5,5]],[6,6]]", 1137)]
        [TestCase(@"[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488)]
        [TestCase(@"[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]", 4140)]
        public void TestMagnitude(string line, int expected)
        {
            var tree = Parse(line);
            Magnitude(tree).Should().Be(expected);
        }

        [Test]
        [TestCase(@"[[[[[9,8],1],2],3],4]", @"[[[[0,9],2],3],4]")]
        [TestCase(@"[7,[6,[5,[4,[3,2]]]]]", @"[7,[6,[5,[7,0]]]]")]
        [TestCase(@"[[6,[5,[4,[3,2]]]],1]", @"[[6,[5,[7,0]]],3]")]
        [TestCase(@"[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", @"[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]")]
        [TestCase(@"[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", @"[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
        public void TestExplode(string line, string expected)
        {
            var tree = Parse(line);

            Explode(tree);

            var expectedTree = Parse(expected);

            JsonConvert.SerializeObject(tree).Should().Be(JsonConvert.SerializeObject(expectedTree));
        }

        [Test]
        [TestCase(@"[[[[0,7],4],[15,[0,13]]],[1,1]]", @"[[[[0,7],4],[[7,8],[0,13]]],[1,1]]")]
        [TestCase(@"[[[[0,7],4],[[7,8],[0,13]]],[1,1]]", @"[[[[0,7],4],[[7,8],[0,[6,7]]]],[1,1]]")]
        public void TestSplit(string line, string expected)
        {
            var tree = Parse(line);

            Split(tree);

            var expectedTree = Parse(expected);

            JsonConvert.SerializeObject(tree).Should().Be(JsonConvert.SerializeObject(expectedTree));
        }

        private static int Magnitude(Node node)
        {
            if (node.Value.HasValue)
                return node.Value.Value;

            return 3 * Magnitude(node.Left) + 2 * Magnitude(node.Right);
        }
    }
}