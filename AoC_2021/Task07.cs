﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2021
{
    [TestFixture]
    public class Task07
    {
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Solve(string input, int expectedPosition, int expectedFuel)
        {
            var ints = input.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var minPosition = -1;
            var minFuel = int.MaxValue;
            foreach (var position in ints)
            {
                var fuel = ints.Sum(x => Math.Abs(x - position));
                if (fuel < minFuel)
                {
                    minFuel = fuel;
                    minPosition = position;
                }
            }

            minFuel.Should().Be(expectedFuel);
            minPosition.Should().Be(expectedPosition);
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            yield return new TestCaseData(@"16,1,2,0,4,2,7,1,2,14", 2, 37);
            yield return new TestCaseData(@"1101,1,29,67,1102,0,1,65,1008,65,35,66,1005,66,28,1,67,65,20,4,0,1001,65,1,65,1106,0,8,99,35,67,101,99,105,32,110,39,101,115,116,32,112,97,115,32,117,110,101,32,105,110,116,99,111,100,101,32,112,114,111,103,114,97,109,10,121,295,1276,124,523,395,987,355,1668,754,139,217,433,1,836,467,1384,66,390,1278,113,114,355,434,396,1050,1552,612,793,532,26,1340,150,455,83,320,587,701,429,596,154,4,1247,55,1376,507,205,1760,774,980,485,10,161,524,213,156,36,56,550,429,337,76,53,276,475,1220,1539,10,53,87,32,1280,757,447,948,983,504,998,1220,27,6,1410,118,172,467,53,351,813,304,708,455,1227,1081,161,1077,36,196,289,371,1354,278,367,992,1,1002,428,535,124,341,318,455,783,916,139,559,71,86,248,71,26,237,99,1064,387,305,454,21,85,202,1156,1410,46,125,217,651,341,15,858,39,248,1677,1792,7,866,781,479,277,1331,92,297,573,112,778,772,388,384,34,335,755,442,1157,537,44,1050,1302,375,136,1017,824,188,362,1359,541,598,43,14,66,1168,43,129,85,71,176,182,1252,336,451,1083,408,761,1327,129,1055,136,770,1489,463,66,685,864,857,150,40,72,807,1017,253,38,659,843,1125,660,891,674,635,113,474,1087,545,196,231,200,865,446,269,739,66,1074,590,5,60,717,170,109,133,243,16,97,393,454,49,597,1355,58,1665,1432,41,251,648,742,87,1172,19,1445,243,728,246,764,206,773,620,866,500,127,91,773,521,47,53,150,828,408,1096,178,567,1356,132,792,6,392,205,894,362,5,554,1287,706,46,800,8,114,152,669,367,640,776,1274,321,1853,308,545,597,28,999,352,29,810,86,106,602,106,121,1467,252,1450,322,267,779,722,57,0,367,39,28,119,99,655,30,723,254,175,921,158,27,879,864,979,436,188,247,96,1109,1033,768,53,175,228,621,134,398,349,270,666,603,818,88,858,310,283,370,517,636,689,114,186,156,306,1044,296,251,1102,1921,34,804,314,298,262,1240,86,227,385,145,54,43,229,15,1034,200,408,1408,13,725,180,597,168,152,961,200,560,288,479,90,157,135,1051,412,1073,132,635,117,597,1448,445,95,278,327,179,1029,18,257,1406,159,1368,359,23,427,225,120,1763,1230,733,110,40,195,870,509,407,881,826,209,106,130,1227,848,336,430,1490,326,377,461,197,70,574,841,497,141,15,83,347,312,555,756,61,1041,171,723,395,632,9,699,365,353,122,620,144,229,12,361,726,0,254,275,28,197,76,387,510,65,771,192,11,345,1317,37,415,286,1,610,378,82,131,94,65,444,455,1146,1319,8,167,79,575,190,351,53,251,352,366,1085,1122,519,5,230,685,388,1476,516,107,365,380,642,1316,400,72,168,178,112,850,264,143,29,605,149,4,216,1327,667,783,40,64,446,580,1280,47,1,322,592,6,560,648,49,116,418,236,620,166,1198,801,362,278,230,417,162,55,535,245,360,73,13,459,505,124,125,325,1126,132,193,374,320,569,842,74,902,210,1199,346,593,912,110,20,482,201,564,248,6,653,208,401,126,248,300,817,581,11,27,924,400,1118,55,1445,6,1024,770,707,510,166,0,435,1188,43,278,338,84,829,1603,55,1255,155,592,420,513,928,1163,51,342,15,499,357,431,1701,54,1521,214,831,199,43,433,1326,1120,803,1154,72,40,181,124,424,464,825,446,500,149,559,355,495,997,87,322,335,701,64,13,978,373,157,1376,1640,255,727,902,86,1230,1517,98,860,797,210,178,1195,493,1007,305,1219,1255,1078,460,712,118,1252,1029,1423,116,41,907,304,651,414,881,162,151,464,265,430,217,1353,315,122,1020,540,8,591,3,50,401,324,1212,143,869,608,706,128,202,25,1118,780,267,305,685,60,92,980,868,1458,446,635,374,565,741,474,367,723,119,70,683,471,651,155,23,158,91,287,8,516,560,434,236,572,320,16,100,140,171,380,672,699,731,621,1346,531,376,182,484,2,198,489,382,388,1200,58,4,885,1575,1320,540,32,1293,217,281,18,212,1240,634,29,153,258,472,501,743,109,55,577,1518,173,308,369,1365,587,260,98,1038,6,1187,278,582,115,1539,282,224,40,306,1447,942,1732,131,320,243,14,559,991,308,7,111,1072,566,42,103,69,1857,1556,229,300,540,183,243,1312,546,19,1287,1597,421,391,169,520,267,591,195,1076,590,917,1018,556,410,498,488,352,590,814,23,299,1254,123,1228,722,745,96,644,288,252,253,1010,26,255,568,783,147,57,63,696,619,1362,1575,1525,696,80,746,294,447,183,320,176,544,278,1757,204,362,13,412,173,438,285,468,3,47,1020,347,640,1442",
                352, 336131);
        }
    }
}