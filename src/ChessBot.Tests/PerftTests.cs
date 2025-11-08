using System.Diagnostics;
using System.Numerics;
using Xunit.Abstractions;

namespace ChessBot.Tests;

public class PerftTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PerftTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory] // https://www.chessprogramming.org/Perft_Results#cite_note-4
    //Initial Position
    //[InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 1, 20)]
    //[InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 2, 400)]
    //[InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 3, 8902)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 4, 197_281)]
    //[InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 5, 4_865_609)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 6, 119_060_324)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 7, 3_195_901_860)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 84_998_978_956)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 9, 2_439_530_234_167)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 10, 69_352_859_712_417)]
    
    // Position 2
    //[InlineData("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 1, 44)]
    //[InlineData("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 2, 1486)]
    [InlineData("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 3, 62_379)]
    //[InlineData("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 4, 2_103_487)]
    //[InlineData("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 5, 898_941_194)]
    
    // Position 3
    //[InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 1, 48)]
    //[InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 2, 2039)]
    [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 3, 97_862)]
    // [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 4, 4_085_603)]
    // [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 5, 193_690_690)]
    // [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -", 6, 8_031_647_685)]
    public void PerftTest(string fen, int depth, BigInteger expectedNodes)
    {
        var board = new Board(BoardUtils.GenerateFromFenString(fen));

        //List<Move> Moves = [];
        Stopwatch sw = new();
        
        sw.Restart();
        var data = Perft(board.Clone(), depth);
        sw.Stop();

        _testOutputHelper.WriteLine($"Perft depth {depth}:");
        _testOutputHelper.WriteLine($"  Nodes      : {data.Nodes:N0}");
        _testOutputHelper.WriteLine($"  Captures   : {data.Captures:N0}");
        _testOutputHelper.WriteLine($"  EnPassants : {data.EnPassants:N0}");
        _testOutputHelper.WriteLine($"  Promotions : {data.Promotions:N0}");
        _testOutputHelper.WriteLine($"  Checks     : {data.Checks:N0}");
        _testOutputHelper.WriteLine($"  CheckMates : {data.CheckMates:N0}");
        _testOutputHelper.WriteLine($"  Time       : {sw.ElapsedMilliseconds:N0} ms");
        _testOutputHelper.WriteLine("");

        Assert.Equal(expectedNodes, data.Nodes);
    }

    private static PerftResult Perft(Board board, int depth)
    {
        var result = new PerftResult();

        if (depth == 0)
        {
            result.Nodes = 1;
            return result;
        }

        foreach (var move in board.LegalMoves.FriendlyMoves)
        {
            var newBoard = board.Clone();
            newBoard.MakeMove(move);
            
            var isEnpassant = board[move.StartSquare].IsType(Piece.Pawn) && 
                              board[move.TargetSquare] is Piece.None &&
                              move.StartSquare - move.TargetSquare is -7 or 7 or -9 or 9 &&
                              (move.TargetSquare.ToIntBit() & board.EnPassantBits) > 0;

            if (board[move.TargetSquare] is not Piece.None || isEnpassant)
                result.Captures++;
            
            if (isEnpassant)
                result.EnPassants++;
            
            if (move.Promotion is not PromotionFlag.None)
                result.Promotions++;
            
            if (newBoard.IsCheck)
                result.Checks++;
            
            if (newBoard.IsMate)
                result.CheckMates++;

            var childResult = Perft(newBoard, depth - 1);
            
            result.Nodes += childResult.Nodes;
            result.Captures += childResult.Captures;
            result.EnPassants += childResult.EnPassants;
            result.Promotions += childResult.Promotions;
            result.Checks += childResult.Checks;
            result.CheckMates += childResult.CheckMates;
        }

        return result;
    }

    private sealed class PerftResult
    {
        public BigInteger Nodes { get; set; }
        public BigInteger Captures { get; set; }
        public BigInteger EnPassants { get; set; }
        public BigInteger Promotions { get; set; }
        public BigInteger Checks { get; set; }
        public BigInteger CheckMates { get; set; }
    }
}