namespace ChessBot.Tests;

public partial class BoardTests
{
    [Fact]
    public void Checkmate_FoolsMate_IsDetected()
    {
        // Position after: 1. f3 e5 2. g4 Qh4#
        var fen = "rnb1kbnr/pppp1ppp/8/4p3/6Pq/5P2/PPPPP2P/RNBQKBNR w - - 1 3";
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
}