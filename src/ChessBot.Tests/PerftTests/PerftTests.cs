using System.Diagnostics;
using System.Numerics;
using Xunit.Abstractions;

namespace ChessBot.Tests.PerftTests;

public class PerftTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PerftTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private class PerftData
    {
        public BigInteger Nodes { get; set; } = BigInteger.Zero;
        public long Captures { get; set; }
        public long EnPassants { get; set; }
        public long Promotions { get; set; }
        public long Checks { get; set; }
        public long CheckMates { get; set; }

        public void Add(PerftData other)
        {
            Nodes      += other.Nodes;
            Captures   += other.Captures;
            EnPassants += other.EnPassants;
            Promotions += other.Promotions;
            Checks     += other.Checks;
            CheckMates += other.CheckMates;
        }
    }

    private static PerftData Perft(Board board, int depth)
    {
        var result = new PerftData();

        if (depth == 0)
        {
            result.Nodes = 1;
            return result;
        }

        board.GenerateLegalMoves();

        if (depth == 1)
        {
            foreach (var move in board.LegalMoves.FriendlyMoves)
            {
                // Skip moves if already mate on current board (consistent with original)
                if (board.IsMate)
                    continue;

                var movingPiece = board[move.StartSquare];
                var targetPiece = board[move.TargetSquare];

                bool isPawn = movingPiece.IsType(Piece.Pawn);
                bool isPromotion = isPawn && (move.TargetSquare.Rank() is 0 or 7);

                // En passant: pawn moves diagonally to empty square that matches en-passant bit
                bool isDiagonalPawnMove = isPawn && (Math.Abs(move.TargetSquare - move.StartSquare) % 8 != 0);
                bool isEnPassant = isDiagonalPawnMove && (board.EnPassantBits & (1UL << move.TargetSquare)) > 0;

                bool isCapture = targetPiece != Piece.None || isEnPassant;

                var next = board.Clone();
                next.MakeMove(move, true);

                result.Nodes += 1;
                
                if (isCapture)  
                    result.Captures++;
                
                if (isEnPassant) 
                    result.EnPassants++;
                
                if (isPromotion) 
                    result.Promotions++;
                
                if (next.IsCheck) 
                    result.Checks++;
                
                if (next.IsMate)  
                    result.CheckMates++;
            }

            return result;
        }

        foreach (var move in board.LegalMoves.FriendlyMoves)
        {
            if (board.IsMate)
                continue;

            var next = board.Clone();
            next.MakeMove(move, true);

            var child = Perft(next, depth - 1);
            result.Add(child);
        }

        return result;
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 1, 20)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 2, 400)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 3, 8902)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 4, 197_281)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 5, 4_865_609)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 6, 119_060_324)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 7, 3_195_901_860)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 84_998_978_956)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 9, 2_439_530_234_167)]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 10, 69_352_859_712_417)]
    public void Perft_InitialPosition(string fen, int depth, BigInteger expectedNodes)
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
}