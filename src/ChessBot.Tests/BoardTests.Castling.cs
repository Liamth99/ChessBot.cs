namespace ChessBot.Tests;

public partial class BoardTests
{
    [Fact]
    public void CastlingRights_Disabled_WhenWhiteKingMoves()
    {
        // Position with only kings and rooks, all castling rights available, white to move
        var fen = "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";
        var board = new Board(BoardUtils.GenerateFromFenString(fen));

        // Move white king e1 -> e2
        var from = BoardUtils.GetIndexByPosition("e1");
        var to = BoardUtils.GetIndexByPosition("e2");
        board.MakeMove(new Move((byte)from, (byte)to));

        // Both white castling sides should be cleared
        (board.ValidCastleBits & Board.WhiteKingCastle).ShouldBe(0UL);
        (board.ValidCastleBits & Board.WhiteQueenCastle).ShouldBe(0UL);

        // Black rights should remain untouched
        (board.ValidCastleBits & Board.BlackKingCastle).ShouldBeGreaterThan(0UL);
        (board.ValidCastleBits & Board.BlackQueenCastle).ShouldBeGreaterThan(0UL);
    }

    [Fact]
    public void CastlingRights_Disabled_WhenWhiteRookMoves_KingSide()
    {
        // Position with only kings and rooks, all castling rights available, white to move
        var fen = "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";
        var board = new Board(BoardUtils.GenerateFromFenString(fen));

        // Move rook h1 -> h2
        var from = BoardUtils.GetIndexByPosition("h1");
        var to = BoardUtils.GetIndexByPosition("h2");
        board.MakeMove(new Move(from, to));

        board.ValidCastleBits.ShouldBe(Board.AllCastleBits & ~Board.WhiteKingCastle);
    }

    [Fact]
    public void CastlingRights_Disabled_WhenWhiteRookMoves_QueenSide()
    {
        // Position with only kings and rooks, all castling rights available, white to move
        var fen = "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";
        var board = new Board(BoardUtils.GenerateFromFenString(fen));

        // Move rook a1 -> a2
        var from = BoardUtils.GetIndexByPosition("a1");
        var to = BoardUtils.GetIndexByPosition("a2");
        board.MakeMove(new Move(from, to));

        board.ValidCastleBits.ShouldBe(Board.AllCastleBits & ~Board.WhiteQueenCastle);
    }

    [Fact]
    public void CastlingRights_Disabled_WhenBlackKingMoves()
    {
        // Black to move, all castling rights available
        var fen = "r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1";
        var board = new Board(BoardUtils.GenerateFromFenString(fen));

        // Move black king e8 -> e7
        var from = BoardUtils.GetIndexByPosition("e8");
        var to = BoardUtils.GetIndexByPosition("e7");
        board.MakeMove(new Move(from, to));


        board.ValidCastleBits.ShouldBe(Board.AllCastleBits & ~(Board.BlackKingCastle | Board.BlackQueenCastle));
    }

    [Fact]
    public void CastlingRights_Disabled_WhenBlackRookMoves_KingSide()
    {
        // Black to move, all castling rights available
        var fen = "r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1";
        var board = new Board(BoardUtils.GenerateFromFenString(fen));

        // Move black rook h8 -> h7
        var from = BoardUtils.GetIndexByPosition("h8");
        var to = BoardUtils.GetIndexByPosition("h7");
        board.MakeMove(new Move(from, to));


        board.ValidCastleBits.ShouldBe(Board.AllCastleBits & ~Board.BlackKingCastle);
    }

    [Fact]
    public void CastlingRights_Disabled_WhenBlackRookMoves_QueenSide()
    {
        // Black to move, all castling rights available
        var fen = "r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1";
        var board = new Board(BoardUtils.GenerateFromFenString(fen));

        // Move black rook a8 -> a7
        var from = BoardUtils.GetIndexByPosition("a8");
        var to = BoardUtils.GetIndexByPosition("a7");
        board.MakeMove(new Move(from, to));


        board.ValidCastleBits.ShouldBe(Board.AllCastleBits & ~Board.BlackQueenCastle);
    }
}