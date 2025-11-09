using System.Numerics;

namespace ChessBot.Tests;

public partial class PerftTests
{
    // Values created by stockfish CLI

    public static TheoryData<string, int, Dictionary<Move, BigInteger>> DefaultDivideData = new()
    {
        // {
        //     "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 2, new Dictionary<Move, BigInteger>()
        //     {
        //         { new Move("a2", "a3"), new(20) }, { new Move("b2", "b3"), new(20) }, { new Move("c2", "c3"), new(20) }, { new Move("d2", "d3"), new(20) },
        //         { new Move("e2", "e3"), new(20) }, { new Move("f2", "f3"), new(20) }, { new Move("g2", "g3"), new(20) }, { new Move("h2", "h3"), new(20) },
        //         { new Move("a2", "a4"), new(20) }, { new Move("b2", "b4"), new(20) }, { new Move("c2", "c4"), new(20) }, { new Move("d2", "d4"), new(20) },
        //         { new Move("e2", "e4"), new(20) }, { new Move("f2", "f4"), new(20) }, { new Move("g2", "g4"), new(20) }, { new Move("h2", "h4"), new(20) },
        //         { new Move("b1", "a3"), new(20) }, { new Move("b1", "c3"), new(20) }, { new Move("g1", "f3"), new(20) }, { new Move("g1", "h3"), new(20) },
        //     }
        // },
        // {
        //     "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 3, new Dictionary<Move, BigInteger>()
        //     {
        //         { new Move("a2", "a3"), new(380) }, { new Move("b2", "b3"), new(420) }, { new Move("c2", "c3"), new(420) }, { new Move("d2", "d3"), new(539) },
        //         { new Move("e2", "e3"), new(599) }, { new Move("f2", "f3"), new(380) }, { new Move("g2", "g3"), new(420) }, { new Move("h2", "h3"), new(380) },
        //         { new Move("a2", "a4"), new(420) }, { new Move("b2", "b4"), new(421) }, { new Move("c2", "c4"), new(441) }, { new Move("d2", "d4"), new(560) },
        //         { new Move("e2", "e4"), new(600) }, { new Move("f2", "f4"), new(401) }, { new Move("g2", "g4"), new(421) }, { new Move("h2", "h4"), new(420) },
        //         { new Move("b1", "a3"), new(400) }, { new Move("b1", "c3"), new(440) }, { new Move("g1", "f3"), new(440) }, { new Move("g1", "h3"), new(400) },
        //     }
        // },
        // {
        //     "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 4, new Dictionary<Move, BigInteger>()
        //     {
        //         { new Move("a2", "a3"), new(8457) }, { new Move("b2",  "b3"), new(9345) }, { new Move("c2", "c3"), new(9272) }, { new Move("d2", "d3"), new(11959) },
        //         { new Move("e2", "e3"), new(13134) }, { new Move("f2", "f3"), new(8457) }, { new Move("g2", "g3"), new(9345) }, { new Move("h2", "h3"), new(8457) },
        //         { new Move("a2", "a4"), new(9329) }, { new Move("b2",  "b4"), new(9332) }, { new Move("c2", "c4"), new(9744) }, { new Move("d2", "d4"), new(12435) },
        //         { new Move("e2", "e4"), new(13160) }, { new Move("f2", "f4"), new(8929) }, { new Move("g2", "g4"), new(9328) }, { new Move("h2", "h4"), new(9329) },
        //         { new Move("b1", "a3"), new(8885) }, { new Move("b1",  "c3"), new(9755) }, { new Move("g1", "f3"), new(9748) }, { new Move("g1", "h3"), new(8881) },
        //     }
        // },
        {
            "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 5, new Dictionary<Move, BigInteger>()
            {
                { new Move("a2", "a3"), new(181046) }, { new Move("b2", "b3"), new(215255) }, { new Move("c2", "c3"), new(222861) }, { new Move("d2", "d3"), new(328511) },
                { new Move("e2", "e3"), new(402988) }, { new Move("f2", "f3"), new(178889) }, { new Move("g2", "g3"), new(217210) }, { new Move("h2", "h3"), new(181044) },
                { new Move("a2", "a4"), new(217832) }, { new Move("b2", "b4"), new(216145) }, { new Move("c2", "c4"), new(240082) }, { new Move("d2", "d4"), new(361790) },
                { new Move("e2", "e4"), new(405385) }, { new Move("f2", "f4"), new(198473) }, { new Move("g2", "g4"), new(214048) }, { new Move("h2", "h4"), new(218829) },
                { new Move("b1", "a3"), new(198572) }, { new Move("b1", "c3"), new(234656) }, { new Move("g1", "f3"), new(233491) }, { new Move("g1", "h3"), new(198502) },
            }
        },
    };

    public static TheoryData<string, int, Dictionary<Move, BigInteger>> BugIn_DefaultDivideData = new()
    {
        {
            "rnbqkbnr/pppppppp/8/8/8/P7/1PPPPPPP/RNBQKBNR b KQkq - 0 1", 3 , new Dictionary<Move, BigInteger>()
            {
                { new Move("a7", "a6"), 361 }, { new Move("b7", "b6"), 399 }, { new Move("c7", "c6"), 399 }, { new Move("d7", "d6"), 512 },
                { new Move("e7", "e6"), 569 }, { new Move("f7", "f6"), 361 }, { new Move("g7", "g6"), 399 }, { new Move("h7", "h6"), 361 },
                { new Move("a7", "a5"), 399 }, { new Move("b7", "b5"), 400 }, { new Move("c7", "c5"), 419 }, { new Move("d7", "d5"), 532 },
                { new Move("e7", "e5"), 570 }, { new Move("f7", "f5"), 381 }, { new Move("g7", "g5"), 400 }, { new Move("h7", "h5"), 399 },
                { new Move("b8", "a6"), 380 }, { new Move("b8", "c6"), 418 }, { new Move("g8", "f6"), 418 }, { new Move("g8", "h6"), 380 },
            }
        },
        {
            "rnbqkbnr/1ppppppp/8/p7/8/P7/1PPPPPPP/RNBQKBNR w KQkq a6 0 2", 2 , new Dictionary<Move, BigInteger>()
            {
                { new Move("b2", "b3"), 21 }, { new Move("c2", "c3"), 21 }, { new Move("d2", "d3"), 21 }, { new Move("e2", "e3"), 21 },
                { new Move("f2", "f3"), 21 }, { new Move("g2", "g3"), 21 }, { new Move("h2", "h3"), 21 }, { new Move("a3", "a4"), 20 },
                { new Move("b2", "b4"), 22 }, { new Move("c2", "c4"), 21 }, { new Move("d2", "d4"), 21 }, { new Move("e2", "e4"), 21 },
                { new Move("f2", "f4"), 21 }, { new Move("g2", "g4"), 21 }, { new Move("h2", "h4"), 21 }, { new Move("b1", "c3"), 21 },
                { new Move("g1", "f3"), 21 }, { new Move("g1", "h3"), 21 }, { new Move("a1", "a2"), 21 },
            }
        },
    };

    // https://www.chessprogramming.org/Perft_Results#cite_note-4
    public static TheoryData<string, int, Dictionary<Move, BigInteger>> Position2 = new()
    {
        // {
        //     "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 1, new Dictionary<Move, BigInteger>()
        //     {
        //         { new Move("a2", "a3") , 1 }, { new Move("g2", "g3") , 1 }, { new Move("b2", "b3") , 1 }, { new Move("c2", "c3") , 1 },
        //         { new Move("h2", "h3") , 1 }, { new Move("a2", "a4") , 1 }, { new Move("b2", "b4") , 1 }, { new Move("g2", "g4") , 1 },
        //         { new Move("h2", "h4") , 1 }, { new Move("d7", "c8", PromotionFlag.Queen) , 1 }, { new Move("d7", "c8", PromotionFlag.Rook) , 1 },   { new Move("d7", "c8", PromotionFlag.Bishop) , 1 },
        //         { new Move("d7", "c8", PromotionFlag.Knight) , 1 }, { new Move("b1", "d2") , 1 }, { new Move("b1", "a3") , 1 }, { new Move("b1", "c3") , 1 },
        //         { new Move("e2", "g1") , 1 }, { new Move("e2", "c3") , 1 }, { new Move("e2", "g3") , 1 }, { new Move("e2", "d4") , 1 },
        //         { new Move("e2", "f4") , 1 }, { new Move("c1", "d2") , 1 }, { new Move("c1", "e3") , 1 }, { new Move("c1", "f4") , 1 },
        //         { new Move("c1", "g5") , 1 }, { new Move("c1", "h6") , 1 }, { new Move("c4", "b3") , 1 }, { new Move("c4", "d3") , 1 },
        //         { new Move("c4", "b5") , 1 }, { new Move("c4", "d5") , 1 }, { new Move("c4", "a6") , 1 }, { new Move("c4", "e6") , 1 },
        //         { new Move("c4", "f7") , 1 }, { new Move("h1", "f1") , 1 }, { new Move("h1", "g1") , 1 }, { new Move("d1", "d2") , 1 },
        //         { new Move("d1", "d3") , 1 }, { new Move("d1", "d4") , 1 }, { new Move("d1", "d5") , 1 }, { new Move("d1", "d6") , 1 },
        //         { new Move("e1", "f1") , 1 }, { new Move("e1", "d2") , 1 }, { new Move("e1", "f2") , 1 }, { new Move("e1", "g1", castlingSquare: "h1") , 1 },
        //     }
        // },
        {
            "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 4, new Dictionary<Move, BigInteger>()
            {
                { new Move("a2", "a3") , 46833 }, { new Move("b2", "b3") , 46497 }, { new Move("c2", "c3") , 49406 }, { new Move("g2", "g3") , 44509 },
                { new Move("h2", "h3") , 46762 }, { new Move("a2", "a4") , 48882 }, { new Move("b2", "b4") , 46696 }, { new Move("g2", "g4") , 45506 },
                { new Move("h2", "h4") , 47811 }, { new Move("d7", "c8", PromotionFlag.Queen) , 44226 }, { new Move("d7", "c8",  PromotionFlag.Rook) , 38077 }, { new Move("d7",   "c8", PromotionFlag.Bishop) , 65053 },
                { new Move("d7", "c8", PromotionFlag.Knight) , 62009 }, { new Move("b1", "d2") , 40560 }, { new Move("b1", "a3") , 44378 }, { new Move("b1", "c3") , 50303 },
                { new Move("e2", "g1") , 48844 }, { new Move("e2", "c3") , 54792 }, { new Move("e2", "g3") , 51892 }, { new Move("e2", "d4") , 52109 },
                { new Move("e2", "f4") , 51127 }, { new Move("c1", "d2") , 46881 }, { new Move("c1", "e3") , 53637 }, { new Move("c1", "f4") , 52350 },
                { new Move("c1", "g5") , 45601 }, { new Move("c1", "h6") , 40913 }, { new Move("c4", "b3") , 43453 }, { new Move("c4", "d3") , 43565 },
                { new Move("c4", "b5") , 45559 }, { new Move("c4", "d5") , 48002 }, { new Move("c4", "a6") , 41884 }, { new Move("c4", "e6") , 49872 },
                { new Move("c4", "f7") , 43289 }, { new Move("h1", "f1") , 46101 }, { new Move("h1", "g1") , 44668 }, { new Move("d1", "d2") , 48843 },
                { new Move("d1", "d3") , 57153 }, { new Move("d1", "d4") , 57744 }, { new Move("d1", "d5") , 56899 }, { new Move("d1", "d6") , 43766 },
                { new Move("e1", "f1") , 49775 }, { new Move("e1", "d2") , 33423 }, { new Move("e1", "f2") , 36783 }, { new Move("e1", "g1", castlingSquare: "h1") , 47054 },
            }
        },
    };

    public static TheoryData<string, int, Dictionary<Move, BigInteger>> BugIn_Position2 = new()
    {
        {
            "rnbq1k1r/pp1Pbppp/2p5/8/2B5/P7/1PP1NnPP/RNBQK2R b KQ - 0 8", 3, new Dictionary<Move, BigInteger>()
            {
                { new Move("c6", "c5"),  1366 }, { new Move("a7", "a6"), 1446 }, { new Move("b7", "b6"), 1437 }, { new Move("f7", "f6"), 1309 },
                { new Move("g7", "g6"), 1455 }, { new Move("h7",  "h6"), 1489 }, { new Move("a7", "a5"), 1490 }, { new Move("b7", "b5"), 1524 },
                { new Move("f7", "f5"), 1438 }, { new Move("g7",  "g5"), 1374 }, { new Move("h7", "h5"), 1532 }, { new Move("f2", "d1"), 1243 },
                { new Move("f2", "h1"), 1196 }, { new Move("f2",  "d3"), 156 }, { new Move("f2",  "h3"), 1304 }, { new Move("f2", "e4"), 1508 },
                { new Move("f2", "g4"), 1424 }, { new Move("b8",  "a6"), 1532 }, { new Move("b8", "d7"), 1494 }, { new Move("e7", "a3"), 1639 },
                { new Move("e7", "b4"), 337 }, { new Move("e7",   "h4"), 1432 }, { new Move("e7", "c5"), 1589 }, { new Move("e7", "g5"), 1498 },
                { new Move("e7", "d6"), 1733 }, { new Move("e7",  "f6"), 1456 }, { new Move("c8", "d7"), 1571 }, { new Move("h8", "g8"), 1443 },
                { new Move("d8", "a5"), 395 }, { new Move("d8",   "b6"), 1674 }, { new Move("d8", "c7"), 1709 }, { new Move("d8", "d7"), 1618 },
                { new Move("d8", "e8"), 1602 }, { new Move("f8",  "g8"), 1420 },
            }
        },
        {
            "rnb2k1r/ppqPbppp/2p5/8/2B5/P7/1PP1NnPP/RNBQK2R w KQ - 1 9", 2, new Dictionary<Move, BigInteger>()
            {
                { new Move("b2", "b3"), 39}, { new Move("c2", "c3"), 39}, { new Move("g2", "g3"), 38}, { new Move("h2", "h3"), 39},
                { new Move("a3", "a4"), 39}, { new Move("b2", "b4"), 38}, { new Move("g2", "g4"), 39}, { new Move("h2", "h4"), 39},
                { new Move("d7", "c8", PromotionFlag.Queen), 3}, { new Move("d7", "c8", PromotionFlag.Rook), 3},
                { new Move("d7", "c8", PromotionFlag.Bishop), 40}, { new Move("d7", "c8", PromotionFlag.Knight), 40},
                { new Move("d7", "d8", PromotionFlag.Queen), 2}, { new Move("d7", "d8", PromotionFlag.Rook), 2},
                { new Move("d7", "d8", PromotionFlag.Bishop), 44}, { new Move("d7", "d8", PromotionFlag.Knight), 44},
                { new Move("b1", "d2"), 39}, { new Move("b1", "c3"), 39}, { new Move("e2", "g1"), 39}, { new Move("e2", "c3"), 39},
                { new Move("e2", "g3"), 38}, { new Move("e2", "d4"), 39}, { new Move("e2", "f4"), 37}, { new Move("c1", "d2"), 39},
                { new Move("c1", "e3"), 39}, { new Move("c1", "f4"), 37}, { new Move("c1", "g5"), 37}, { new Move("c1", "h6"), 36},
                { new Move("c4", "a2"), 39}, { new Move("c4", "b3"), 39}, { new Move("c4", "d3"), 39}, { new Move("c4", "b5"), 39},
                { new Move("c4", "d5"), 40}, { new Move("c4", "a6"), 38}, { new Move("c4", "e6"), 40}, { new Move("c4", "f7"), 37},
                { new Move("a1", "a2"), 39}, { new Move("h1", "f1"), 39}, { new Move("h1", "g1"), 39}, { new Move("d1", "d2"), 39},
                { new Move("d1", "d3"), 39}, { new Move("d1", "d4"), 39}, { new Move("d1", "d5"), 40}, { new Move("d1", "d6"), 28},
                { new Move("e1", "f1"), 39}, { new Move("e1", "d2"), 39}, { new Move("e1", "f2"), 33}, { new Move("e1", "g1", castlingSquare: "h1"), 39},
            }
        }
    };

    // https://www.chessprogramming.org/Perft_Results#cite_note-4
    public static TheoryData<string, int, Dictionary<Move, BigInteger>> Position3 = new()
    {
        {
            "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 4, new Dictionary<Move, BigInteger>()
            {
                { new Move("a2", "a3"), 94405 }, { new Move("b2", "b3"), 81066 }, { new Move("g2",  "g3"), 77468 }, { new Move("d5", "d6"), 79551 },
                { new Move("a2", "a4"), 90978 }, { new Move("g2", "g4"), 75677 }, { new Move("g2",  "h3"), 82759 }, { new Move("d5", "e6"), 97464 },
                { new Move("c3", "b1"), 84773 }, { new Move("c3", "d1"), 84782 }, { new Move("c3",  "a4"), 91447 }, { new Move("c3", "b5"), 81498 },
                { new Move("e5", "d3"), 77431 }, { new Move("e5", "c4"), 77752 }, { new Move("e5",  "g4"), 79912 }, { new Move("e5", "c6"), 83885 },
                { new Move("e5", "g6"), 83866 }, { new Move("e5", "d7"), 93913 }, { new Move("e5",  "f7"), 88799 }, { new Move("d2", "c1"), 83037 },
                { new Move("d2", "e3"), 90274 }, { new Move("d2", "f4"), 84869 }, { new Move("d2",  "g5"), 87951 }, { new Move("d2", "h6"), 82323 },
                { new Move("e2", "d1"), 74963 }, { new Move("e2", "f1"), 88728 }, { new Move("e2",  "d3"), 85119 }, { new Move("e2", "c4"), 84835 },
                { new Move("e2", "b5"), 79739 }, { new Move("e2", "a6"), 69334 }, { new Move("a1",  "b1"), 83348 }, { new Move("a1", "c1"), 83263 },
                { new Move("a1", "d1"), 79695 }, { new Move("h1", "f1"), 81563 }, { new Move("h1",  "g1"), 84876 }, { new Move("f3", "d3"), 83727 },
                { new Move("f3", "e3"), 92505 }, { new Move("f3", "g3"), 94461 }, { new Move("f3",  "h3"), 98524 }, { new Move("f3", "f4"), 90488 },
                { new Move("f3", "g4"), 92037 }, { new Move("f3", "f5"), 104992 }, { new Move("f3", "h5"), 95034 }, { new Move("f3", "f6"), 77838 },
                { new Move("e1", "d1"), 79989 }, { new Move("e1", "f1"), 77887 }, { new Move("e1",  "g1"), 86975 }, { new Move("e1", "c1"), 7980 },
            }
        }
    };

    public static TheoryData<string, int, Dictionary<Move, BigInteger>> BugsIn_Position3 = new()
    {
        {
            "r3k2r/p1ppqpb1/bn1Ppnp1/4N3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1", 3, new Dictionary<Move, BigInteger>()
            {
                { new Move("b4", "b3"), 2019 }, { new Move("g6", "g5"), 1937 }, { new Move("c7", "c6"), 1848 }, { new Move("c7", "c5"), 1844 },
                { new Move("h3", "g2"), 2301 }, { new Move("b4", "c3"), 1932 }, { new Move("c7", "d6"), 1803 }, { new Move("b6", "a4"), 1931 },
                { new Move("b6", "c4"), 1946 }, { new Move("b6", "d5"), 1965 }, { new Move("b6", "c8"), 1690 }, { new Move("f6", "e4"), 2508 },
                { new Move("f6", "g4"), 2220 }, { new Move("f6", "d5"), 2220 }, { new Move("f6", "h5"), 2081 }, { new Move("f6", "h7"), 1985 },
                { new Move("f6", "g8"), 1988 }, { new Move("a6", "e2"), 1821 }, { new Move("a6", "d3"), 1983 }, { new Move("a6", "c4"), 1993 },
                { new Move("a6", "b5"), 2035 }, { new Move("a6", "b7"), 2040 }, { new Move("a6", "c8"), 1705 }, { new Move("g7", "h6"), 2012 },
                { new Move("g7", "f8"), 1789 }, { new Move("a8", "b8"), 2032 }, { new Move("a8", "c8"), 1887 }, { new Move("a8", "d8"), 1891 },
                { new Move("h8", "h4"), 2020 }, { new Move("h8", "h5"), 2002 }, { new Move("h8", "h6"), 1834 }, { new Move("h8", "h7"), 1835 },
                { new Move("h8", "f8"), 1644 }, { new Move("h8", "g8"), 1738 }, { new Move("e7", "d6"), 2197 }, { new Move("e7", "d8"), 1805 },
                { new Move("e7", "f8"), 1751 }, { new Move("e8", "d8"), 1789 }, { new Move("e8", "f8"), 1783 },
                { new Move("e8", "g8", castlingSquare: "h8"), 1840 }, { new Move("e8", "c8", castlingSquare: "a8"), 1907 },
            }
        },
        {
            "r3k2r/p1ppqpb1/bn1Ppnp1/4N3/4P3/1pN2Q1p/PPPBBPPP/R3K2R w KQkq - 0 2", 2, new Dictionary<Move, BigInteger>()
            {
                { new Move("a2", "a3"), 40 }, { new Move("g2", "g3"), 40 }, { new Move("a2", "a4"), 40 }, { new Move("g2", "g4"), 40 },
                { new Move("a2", "b3"), 39 }, { new Move("g2", "h3"), 41 }, { new Move("d6", "e7"), 36 }, { new Move("c2", "b3"), 39 },
                { new Move("d6", "c7"), 41 }, { new Move("c3", "b1"), 41 }, { new Move("c3", "d1"), 41 }, { new Move("c3", "a4"), 41 },
                { new Move("c3", "b5"), 38 }, { new Move("c3", "d5"), 42 }, { new Move("e5", "d3"), 41 }, { new Move("e5", "c4"), 40 },
                { new Move("e5", "g4"), 42 }, { new Move("e5", "c6"), 39 }, { new Move("e5", "g6"), 40 }, { new Move("e5", "d7"), 44 },
                { new Move("e5", "f7"), 42 }, { new Move("d2", "c1"), 41 }, { new Move("d2", "e3"), 41 }, { new Move("d2", "f4"), 41 },
                { new Move("d2", "g5"), 40 }, { new Move("d2", "h6"), 39 }, { new Move("e2", "d1"), 42 }, { new Move("e2", "f1"), 42 },
                { new Move("e2", "d3"), 40 }, { new Move("e2", "c4"), 39 }, { new Move("e2", "b5"), 38 }, { new Move("e2", "a6"), 34 },
                { new Move("a1", "b1"), 41 }, { new Move("a1", "c1"), 41 }, { new Move("a1", "d1"), 41 }, { new Move("h1", "f1"), 41 },
                { new Move("h1", "g1"), 41 }, { new Move("f3", "d3"), 40 }, { new Move("f3", "e3"), 41 }, { new Move("f3", "g3"), 41 },
                { new Move("f3", "h3"), 41 }, { new Move("f3", "f4"), 41 }, { new Move("f3", "g4"), 41 }, { new Move("f3", "f5"), 43 },
                { new Move("f3", "h5"), 41 }, { new Move("f3", "f6"), 37 }, { new Move("e1", "d1"), 41 }, { new Move("e1", "f1"), 41 },
                { new Move("e1", "g1", castlingSquare: "h1"), 41 }, { new Move("e1", "c1", castlingSquare: "a1"), 41 },
            }
        }
    };
}