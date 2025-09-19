namespace ChessBot.Tests;

public partial class BoardTests
{
    [Fact]
    public void Stalemate_Classic_KingCorner_BoxedByKingAndQueen()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("7k/8/5KQ1/8/8/8/8/8 b - - 0 1"));

        board.IsCheck.ShouldBeFalse();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeTrue();
    }

    [Fact]
    public void Stalemate_KingBlocked_ByKingAndPawn()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("1k6/1P6/1K6/8/8/8/8/8 b - - 0 1"));

        board.IsCheck.ShouldBeFalse();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeTrue();
    }

    [Fact]
    public void Pinned_Rook_OnFile_CannotMoveSideways()
    {
        // White king e1, white rook e2, black rook e8. White to move.
        var board = new Board(BoardUtils.GenerateFromFenString("4r3/8/8/8/8/8/4R3/4K3 w - - 0 1"));

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
        // White king e1, white knight e2, black rook e8. White to move.
        var board = new Board(BoardUtils.GenerateFromFenString("4r3/8/8/8/8/8/4N3/4K3 w - - 0 1"));

        var knightMoves = board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 12).ToList();
        knightMoves.Count.ShouldBe(0);
    }

    [Fact]
    public void Pinned_Pawn_EnPassant_Capture_IsIllegal_IfItExposesKing()
    {
        // White king e1, black rook e8, white pawn e5, black pawn d7. Black to move.
        var board = new Board(BoardUtils.GenerateFromFenString("4r3/3p4/8/4P3/8/8/8/4K3 b - - 0 1"));

        // Black double-push d7 -> d5
        board.MakeMove(new Move(51, 35));

        // EP capture from e5 to d6 is (36 -> 43). This should be disallowed due to pin.
        var illegalEpCapture = new Move(36, 43);

        board.LegalMoves.FriendlyMoves.ShouldNotContain(illegalEpCapture);

        // Normal forward move e5 -> e6 (36 -> 44) should still be legal.
        var legalForward = new Move(36, 44);
        board.LegalMoves.FriendlyMoves.ShouldContain(legalForward);
    }
}