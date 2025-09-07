namespace ChessBot.Tests;

public partial class BoardTests
{
    [Theory]
    [InlineData("a1", 0)]
    [InlineData("a2", 8)]
    [InlineData("c3", 18)]
    [InlineData("h8", 63)]
    [InlineData("b1", 1)]
    [InlineData("d4", 27)]
    [InlineData("e4", 28)]
    [InlineData("a8", 56)]
    [InlineData("h2", 15)]
    [InlineData("g7", 54)]
    [InlineData("f5", 37)]
    [InlineData("e5", 36)]
    [InlineData("b8", 57)]
    [InlineData("c1", 2)]
    public void GetIndexByPosition_ReturnsIndex(string position, byte index)
    {
        BoardUtils.GetIndexByPosition(position).ShouldBe(index);
    }
    
    [Theory]
    [InlineData(0,  "a1")]
    [InlineData(1,  "b1")]
    [InlineData(18, "c3")]
    [InlineData(63, "h8")]
    [InlineData(27, "d4")]
    [InlineData(28, "e4")]
    [InlineData(56, "a8")]
    [InlineData(15, "h2")]
    [InlineData(54, "g7")]
    [InlineData(37, "f5")]
    [InlineData(36, "e5")]
    [InlineData(57, "b8")]
    [InlineData(2,  "c1")]
    public void GetPositionByIndex_ReturnsPosition(byte index, string position)
    {
        BoardUtils.GetPositionByIndex(index).ShouldBe(position);
    }

    [Fact]
    public void DefaultFenGeneration_IsStandard()
    {
        var boardState = BoardUtils.GenerateFromFenString();
        
        boardState.Squares[0].ShouldBe(Piece.White | Piece.Rook);
        boardState.Squares[1].ShouldBe(Piece.White | Piece.Knight);
        boardState.Squares[2].ShouldBe(Piece.White | Piece.Bishop);
        boardState.Squares[3].ShouldBe(Piece.White | Piece.Queen);
        boardState.Squares[4].ShouldBe(Piece.White | Piece.King);
        boardState.Squares[5].ShouldBe(Piece.White | Piece.Bishop);
        boardState.Squares[6].ShouldBe(Piece.White | Piece.Knight);
        boardState.Squares[7].ShouldBe(Piece.White | Piece.Rook);

        for (int i = 0; i < 8; i++)
        {
            boardState.Squares[i + 8].ShouldBe(Piece.White | Piece.Pawn);
        }
        
        for (int i = 0; i < 8; i++)
        {
            boardState.Squares[i + 48].ShouldBe(Piece.Black | Piece.Pawn);
        }
        
        boardState.Squares[56].ShouldBe(Piece.Black | Piece.Rook);
        boardState.Squares[57].ShouldBe(Piece.Black | Piece.Knight);
        boardState.Squares[58].ShouldBe(Piece.Black | Piece.Bishop);
        boardState.Squares[59].ShouldBe(Piece.Black | Piece.Queen);
        boardState.Squares[60].ShouldBe(Piece.Black | Piece.King);
        boardState.Squares[61].ShouldBe(Piece.Black | Piece.Bishop);
        boardState.Squares[62].ShouldBe(Piece.Black | Piece.Knight);
        boardState.Squares[63].ShouldBe(Piece.Black | Piece.Rook);
        
        boardState.EnPassantBits.ShouldBe(0u);
        boardState.ValidCastleBits.ShouldBe(Board.AllCastleBits);
        
        boardState.ColorToMove.ShouldBe(Piece.White);
        boardState.HalfMoveClock.ShouldBe(0);
        boardState.FullMoveCount.ShouldBe(1);
    }

    [Fact]
    public void Fen_WithEnPassant_TargetSquareSet()
    {
        // 8/8/8/8/3pP3/8/8/8 b - e3 0 20
        var fen = "8/8/8/8/3pP3/8/8/8 b - e3 0 20";
        var state = BoardUtils.GenerateFromFenString(fen);

        // Pieces
        state.Squares[27].ShouldBe(Piece.Black | Piece.Pawn);  // d4
        state.Squares[28].ShouldBe(Piece.White | Piece.Pawn);  // e4

        // Side to move
        state.ColorToMove.ShouldBe(Piece.Black);

        // En passant target
        var epIndex = BoardUtils.GetIndexByPosition("e3");
        state.EnPassantBits.ShouldBe(1uL << epIndex);

        // Clocks
        state.HalfMoveClock.ShouldBe(0);
        state.FullMoveCount.ShouldBe(20);
    }
    
    [Theory]
    [InlineData("K",       Board.WhiteKingCastle)]
    [InlineData("Q",       Board.WhiteQueenCastle)]
    [InlineData("k",       Board.BlackKingCastle)]
    [InlineData("q",       Board.BlackQueenCastle)]
    [InlineData("KQkq",    Board.AllCastleBits)]
    [InlineData("-",       0L)]
    public void Fen_CastlingRights_SetCastleBits(string castling, ulong expectedBits)
    {
        var fen = $"8/8/8/8/8/8/8/8 w {castling} - 0 1";
        var state = BoardUtils.GenerateFromFenString(fen);
        state.ValidCastleBits.ShouldBe(expectedBits);
    }
    
    [Fact]
    public void Fen_InvalidParts_ThrowsArgumentException()
    {
        var invalidFen = "8/8/8/8/8/8/8/8 w - - 0"; // only 5 parts
        Should.Throw<ArgumentException>(() => BoardUtils.GenerateFromFenString(invalidFen));
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    [InlineData("4k3/8/8/8/8/8/8/4K3 b - - 0 1")]
    [InlineData("8/8/8/8/3pP3/8/8/8 b - e3 0 20")]
    [InlineData("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 7 42")]
    [InlineData("8/8/8/8/8/8/8/8 w - - 0 1")]
    [InlineData("8/8/8/8/8/8/8/8 b Kq - 12 99")]
    [InlineData("8/8/8/8/8/8/8/8 w - a3 0 1")]
    [InlineData("8/8/8/6Pp/8/8/8/8 b - h6 3 12")]
    [InlineData("3qk3/ppp2ppp/8/2B5/4P3/8/PPP3PP/3QK3 w Q - 4 10")]
    [InlineData("n2qk2n/1ppp1pp1/8/8/4P3/8/1PP3P1/N2QK2N b k - 15 23")]
    public void Fen_ToBoard_ToString_IsValid(string fenString)
    {
        var board = new Board(BoardUtils.GenerateFromFenString(fenString));
        
        BoardUtils.GenerateFenString(board).ShouldBe(fenString);
    }
}