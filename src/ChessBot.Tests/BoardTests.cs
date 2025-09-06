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
        
        boardState[0].ShouldBe(Piece.White | Piece.Rook);
        boardState[1].ShouldBe(Piece.White | Piece.Knight);
        boardState[2].ShouldBe(Piece.White | Piece.Bishop);
        boardState[3].ShouldBe(Piece.White | Piece.Queen);
        boardState[4].ShouldBe(Piece.White | Piece.King);
        boardState[5].ShouldBe(Piece.White | Piece.Bishop);
        boardState[6].ShouldBe(Piece.White | Piece.Knight);
        boardState[7].ShouldBe(Piece.White | Piece.Rook);

        for (int i = 0; i < 8; i++)
        {
            boardState[i + 8].ShouldBe(Piece.White | Piece.Pawn);
        }
        
        for (int i = 0; i < 8; i++)
        {
            boardState[i + 48].ShouldBe(Piece.Black | Piece.Pawn);
        }
        
        boardState[56].ShouldBe(Piece.Black | Piece.Rook);
        boardState[57].ShouldBe(Piece.Black | Piece.Knight);
        boardState[58].ShouldBe(Piece.Black | Piece.Bishop);
        boardState[59].ShouldBe(Piece.Black | Piece.Queen);
        boardState[60].ShouldBe(Piece.Black | Piece.King);
        boardState[61].ShouldBe(Piece.Black | Piece.Bishop);
        boardState[62].ShouldBe(Piece.Black | Piece.Knight);
        boardState[63].ShouldBe(Piece.Black | Piece.Rook);
    }

    

    
}