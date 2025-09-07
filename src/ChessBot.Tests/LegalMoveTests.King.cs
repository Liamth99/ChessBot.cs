namespace ChessBot.Tests;

public partial class LegalMoveTests
{
    [Fact]
    public void GeneratedMoves_King_AreValid()
    {
        var board = new Board();
        board[35] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(35, 26), new(35, 27), new(35, 28),
            new(35, 34), new(35, 36),
            new(35, 42), new(35, 43), new(35, 44)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_BlockedByFriendly()
    {
        var board = new Board();
        board[35] = Piece.White | Piece.King;
        board[34] = Piece.White | Piece.Pawn;
        board[36] = Piece.White | Piece.Pawn;
        board[43] = Piece.White | Piece.Pawn;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.Where(x => x.StartSquare is 35).OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(35, 26), new(35, 27), new(35, 28),
            new(35, 42), new(35, 44)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_AtTopEdge()
    {
        var board = new Board();
        board[60] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(60, 51), new(60, 52), new(60, 53),
            new(60, 59), new(60, 61)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_AtBottomEdge()
    {
        var board = new Board();
        board[4] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(4, 3), new(4, 5),
            new(4, 11), new(4, 12), new(4, 13)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_AtLeftEdge()
    {
        var board = new Board();
        board[32] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(32, 24), new(32, 25),
            new(32, 33),
            new(32, 40), new(32, 41)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_AtRightEdge()
    {
        var board = new Board();
        board[39] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(39, 30), new(39, 31),
            new(39, 38),
            new(39, 46), new(39, 47)
        ]);
    }
    
    [Fact]
    public void GeneratedMoves_Castling_WhiteKingSide_FromFen()
    {
        // Position: White king on e1, rook on h1, white to move, K castling right
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/4K2R w K - 0 1"));

        // Expect the king-side castle e1 -> g1 (4 -> 6) to be generated
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(4, 6));
    }

    [Fact]
    public void GeneratedMoves_Castling_BlackKingSide_FromFen()
    {
        // Position: Black king on e8, rook on h8, black to move, k castling right
        var board = new Board(BoardUtils.GenerateFromFenString("4k2r/8/8/8/8/8/8/4K3 b k - 0 1"));

        // Expect the king-side castle e8 -> g8 (60 -> 62) to be generated
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(60, 62, PromotionFlag.None, 63));
    }

    [Fact]
    public void MakeMove_Castling_WhiteKingSide_FromFen_RookAndKingPositions()
    {
        // Position: White king on e1, rook on h1, white to move, K castling right
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/4K2R w K - 0 1"));

        // Ensure the castle move exists
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(4, 6));

        // Execute e1 -> g1 with castlingSquare set to h1 (7)
        var castle = new Move(4, 6, PromotionFlag.None, 7);
        board.MakeMove(castle);

        board[4].ShouldBe(Piece.None);                 // e1
        board[7].ShouldBe(Piece.None);                 // h1
        board[6].ShouldBe(Piece.White | Piece.King);   // g1
        board[5].ShouldBe(Piece.White | Piece.Rook);   // f1
    }

}