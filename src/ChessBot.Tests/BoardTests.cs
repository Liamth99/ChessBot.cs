namespace ChessBot.Tests;

public class BoardTests
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
    
    [Fact]
    public void Checkmate_FoolsMate_IsDetected()
    {
        // Position after: 1. f3 e5 2. g4 Qh4#
        var fen = "rnb1kbnr/pppp1ppp/8/4p3/6Pq/5P2/PPPPP2P/RNBQKBNR w KQkq - 1 3";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeTrue();
        board.IsMate.ShouldBeTrue();
        board.IsDraw.ShouldBeFalse();
    }
    
    [Fact]
    public void Check_ButNotMate_IsDetected()
    {
        // Black king on e8 in check from white queen on e4; black has escapes.
        var fen = "4k3/8/8/8/4Q3/8/8/4K3 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeTrue();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeFalse();
    }
    
    [Fact]
    public void NoCheck_NoMate_InStartPosition()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeFalse();
        board.IsMate.ShouldBeFalse();
    }

    [Fact]
    public void Checkmate_QueenAndKing_VsLoneKing_IsDetected()
    {
        // Simple boxed mate: Black king a8; White queen b7, White king c7. Black to move and checkmated.
        var fen = "k7/1QK5/8/8/8/8/8/8 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeTrue();
        board.IsMate.ShouldBeTrue();
        board.IsDraw.ShouldBeFalse();
    }

    [Fact]
    public void Check_DoubleCheck_NotMate_WhenKingHasEscape()
    {
        // Double check on black king e8 from Re1 and Bb5; f8 is a legal escape, so not mate.
        var fen = "4k3/8/8/1B6/8/8/8/4R3 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeTrue();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeFalse();
    }

    [Fact]
    public void Checkmate_SmotheredMate_KnightF7_IsDetected()
    {
        // Classic smothered mate pattern: Black king h8 trapped by own pieces; White knight on f7 gives mate.
        // Pieces: Black king h8, rook g8, pawns g7,h7; White knight f7, White king a1 (far away).
        var fen = "6rk/5Npp/8/8/8/8/8/K7 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeTrue();
        board.IsMate.ShouldBeTrue();
        board.IsDraw.ShouldBeFalse();
    }

    [Fact]
    public void Check_RookAlignedButBlocked_NoCheck()
    {
        // Black king e8, White rook e1 but a White pawn on e7 blocks the check; should not be check.
        var fen = "4k3/4P3/8/8/8/8/8/4R3 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeFalse();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeFalse();
    }

    [Fact]
    public void Check_BishopLongDiagonal_WithEscape_NotMate()
    {
        // White bishop a4 checking black king e8 along diagonal; Black can move Kd8, so not mate.
        var fen = "4k3/8/8/8/B7/8/8/4K3 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeTrue();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeFalse();
    }

    [Fact]
    public void Checkmate_BackRank_WithRook_IsDetected()
    {
        // Back-rank mate against black.
        var fen = "4R1k1/6pp/8/8/8/8/8/5R2 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeTrue();
        board.IsMate.ShouldBeTrue();
        board.IsDraw.ShouldBeFalse();
    }

    [Fact]
    public void Stalemate_Classic_KingCorner_BoxedByKingAndQueen()
    {
        // Black to move, not in check, but has no legal moves.
        var fen = "7k/8/5KQ1/8/8/8/8/8 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeFalse();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeTrue();
    }

    [Fact]
    public void Stalemate_KingBlocked_ByKingAndPawn()
    {
        // Another stalemate motif:
        var fen = "1k6/1P6/1K6/8/8/8/8/8 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeFalse();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeTrue();
    }

    [Fact]
    public void Pinned_Rook_OnFile_CannotMoveSideways()
    {
        // Setup:
        // White king e1 (4), white rook e2 (12), black rook e8 (60).
        // Rook on e2 is pinned along the e-file; sideways moves must be illegal.
        var board = new Board();
        board[4]  = Piece.White | Piece.King;  // e1
        board[12] = Piece.White | Piece.Rook;  // e2
        board[60] = Piece.Black | Piece.Rook;  // e8

        board.GenerateLegalMoves();

        // Illegal: e2 -> d2 (sideways while pinned)
        var illegalSideways = new Move(12, 11);
        board.LegalMoves.FriendlyMoves.ShouldNotContain(illegalSideways);

        // Legal: e2 -> e3 (still blocking the e-file)
        var legalAlongFile = new Move(12, 20);
        board.LegalMoves.FriendlyMoves.ShouldContain(legalAlongFile);

        // Legal: e2 -> e8 (capturing the attacker)
        var legalCaptureAttacker = new Move(12, 60);
        board.LegalMoves.FriendlyMoves.ShouldContain(legalCaptureAttacker);
    }

    [Fact]
    public void Pinned_Knight_OnFile_HasNoLegalMoves()
    {
        // Setup:
        // White king e1 (4), white knight e2 (12), black rook e8 (60).
        // Knight is pinned; any knight move would expose the king.
        var board = new Board();
        board[4]  = Piece.White | Piece.King;    // e1
        board[12] = Piece.White | Piece.Knight;  // e2
        board[60] = Piece.Black | Piece.Rook;    // e8

        board.GenerateLegalMoves();

        var knightMoves = board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 12).ToList();
        knightMoves.Count.ShouldBe(0);
    }


    [Fact]
    public void Pinned_Pawn_EnPassant_Capture_IsIllegal_IfItExposesKing()
    {
        // Classic EP pin motif:
        // White king e1 (4), black rook e8 (60) along the open e-file.
        // White pawn e5 (36), black pawn d7 (51) will double-push to d5 (35) enabling EP (d6 = 43).
        // EP move e5xd6 EP would remove the e5 pawn from the e-file and expose the white king to the rook on e8.
        // Therefore, the EP capture must be excluded from legal moves.

        var board = new Board();
        board[4]  = Piece.White | Piece.King; // e1
        board[60] = Piece.Black | Piece.Rook; // e8
        board[36] = Piece.White | Piece.Pawn; // e5
        board[51] = Piece.Black | Piece.Pawn; // d7

        // Switch to black, make the double push d7 -> d5
        board.MakeMove(new Move());
        board.MakeMove(new Move(51, 35));

        // EP capture from e5 to d6 is (36 -> 43). This should be disallowed due to pin.
        var illegalEpCapture = new Move(36, 43);

        board.LegalMoves.FriendlyMoves.ShouldNotContain(illegalEpCapture);

        // Normal forward move e5 -> e6 (36 -> 44) should still be legal.
        var legalForward = new Move(36, 44);
        board.LegalMoves.FriendlyMoves.ShouldContain(legalForward);
    }

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