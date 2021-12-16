using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task16_2
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int expected)
        {
            var binaryString = input.ToBinFromHex();

            var packet = ReadPacket(binaryString, 0);

            var actual = Calculate(packet.Packet);
            actual.Should().Be(expected);
        }

        private long Calculate(Packet packet)
        {
            return packet.TypeId switch
            {
                0 => packet.Packets.Select(Calculate).Sum(),
                1 => packet.Packets.Select(Calculate).Aggregate((a, b) => a * b),
                2 => packet.Packets.Select(Calculate).Min(),
                3 => packet.Packets.Select(Calculate).Max(),
                4 => packet.Literal,
                5 => Calculate(packet.Packets[0]) > Calculate(packet.Packets[1]) ? 1 : 0,
                6 => Calculate(packet.Packets[0]) < Calculate(packet.Packets[1]) ? 1 : 0,
                7 => Calculate(packet.Packets[0]) == Calculate(packet.Packets[1]) ? 1 : 0,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"C200B40A82", 3);
            yield return new TestCaseData(@"04005AC33890", 54);
            yield return new TestCaseData(@"880086C3E88112", 7);
            yield return new TestCaseData(@"CE00C43D881120", 9);
            yield return new TestCaseData(@"D8005AC2A8F0", 1);
            yield return new TestCaseData(@"F600BC2D8F", 0);
            yield return new TestCaseData(@"9C005AC2F8F0", 0);
            yield return new TestCaseData(@"9C0141080250320F1802104A08", 1);
            yield return new TestCaseData(
                @"620D7B005DF2549DF6D51466E599E2BF1F60016A3F980293FFC16E802B325332544017CC951E3801A19A3C98A7FF5141004A48627F21A400C0C9344EBA4D9345D987A8393D43D000172329802F3F5DE753D56AB76009080350CE3B44810076274468946009E002AD2D9936CF8C4E2C472400732699E531EDDE0A4401F9CB9D7802F339DE253B00CCE77894EC084027D4EFFD006C00D50001098B708A340803118E0CC5C200A1E691E691E78EA719A642D400A913120098216D80292B08104DE598803A3B00465E7B8738812F3DC39C46965F82E60087802335CF4BFE219BA34CEC56C002EB9695BDAE573C1C87F2B49A3380273709D5327A1498C4F6968004EC3007E1844289240086C4D8D5174C01B8271CA2A880473F19F5024A5F1892EF4AA279007332980CA0090252919DEFA52978ED80804CA100622D463860200FC608FB0BC401888B09E2A1005B725FA25580213C392D69F9A1401891CD617EAF4A65F27BC5E6008580233405340D2BBD7B66A60094A2FE004D93F99B0CF5A52FF3994630086609A75B271DA457080307B005A68A6665F3F92E7A17010011966A350C92AA1CBD52A4E996293BEF5CBFC3480085994095007009A6A76DF136194E27CE34980212B0E6B3940B004C0010A8631E20D0803F0F21AC2020085B401694DA4491840D201237C0130004222BC3F8C2200DC7686B14A67432E0063801AC05C3080146005101362E0071010EC403E19801000340A801A002A118012C0200B006AC4015088018000B398019399C2FC432013E3003551006E304108AA000844718B51165F35FA89802F22D3801374CE3C3B2B8B5B7DDC11CC011C0090A6E92BF5226E92BF5327F3FD48750036898CC7DD9A14238DD373C6E70FBCA0096536673DC9C7770D98EE19A67308154B7BB799FC8CE61EE017CFDE2933FBE954C69864D1E5A1BCEC38C0179E5E98280"
                , 1675198555015L);
        }

        private static IEnumerable<Packet> Enroll(Packet root)
        {
            yield return root;
            foreach (var packet in root.Packets.SelectMany(Enroll))
            {
                yield return packet;
            }
        }

        private static (Packet Packet, int Next) ReadPacket(string src, int index)
        {
            var version = GetVersion(src, index);
            var typeId = GetTypeId(src, version.Next);

            switch (typeId.TypeId)
            {
                case 4:
                    var readLiterals = GetLiterals(src, typeId.Next);
                    return (new Packet
                    {
                        Version = version.Version,
                        TypeId = typeId.TypeId,
                        Literal = readLiterals.Literals.ToLongFromBin()
                    }, readLiterals.Next);
                default:
                    var lengthTypeId = GetLengthTypeId(src, typeId.Next);

                    var packets = new List<Packet>();
                    var nextIndex = lengthTypeId.Next;
                    if (lengthTypeId.LengthTypeId == '0')
                    {
                        var subPacketBitsCount = src.Substring(nextIndex, 15).ToIntFromBin();
                        nextIndex += 15;

                        var backUpNextIndex = nextIndex;
                        while (true)
                        {
                            var newPacket = ReadPacket(src, backUpNextIndex);

                            packets.Add(newPacket.Packet);

                            if (newPacket.Next >= nextIndex + subPacketBitsCount)
                                break;

                            backUpNextIndex = newPacket.Next;
                        }

                        nextIndex += subPacketBitsCount;
                    }
                    else if (lengthTypeId.LengthTypeId == '1')
                    {
                        var subPacketCount = src.Substring(nextIndex, 11).ToIntFromBin();
                        nextIndex += 11;

                        for (var i = 0; i < subPacketCount; ++i)
                        {
                            var packet = ReadPacket(src, nextIndex);
                            packets.Add(packet.Packet);
                            nextIndex = packet.Next;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("ltypeid");
                    }

                    return (new Packet
                    {
                        Version = version.Version,
                        TypeId = typeId.TypeId,
                        Packets = packets
                    }, nextIndex);
            }
        }

        private static (int Version, int Next) GetVersion(string src, int index)
        {
            return (src.Substring(index, 3).ToIntFromBin(), index + 3);
        }

        private static (int TypeId, int Next) GetTypeId(string src, int index)
        {
            return (src.Substring(index, 3).ToIntFromBin(), index + 3);
        }

        private static (string Literals, int Next) GetLiterals(string src, int index)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var flag = src[index];
                sb.Append(src.Substring(index + 1, 4));
                index += 5;


                if (flag == '0')
                {
                    return (sb.ToString(), index);
                }
            }
        }

        private static (char LengthTypeId, int Next) GetLengthTypeId(string src, int index)
        {
            return (src[index], index + 1);
        }


        private class Packet
        {
            public Packet()
            {
                Packets = new List<Packet>();
            }

            public int Version { get; set; }
            public int TypeId { get; set; }
            public long Literal { get; set; }
            public List<Packet> Packets { get; set; }
        }
    }
}