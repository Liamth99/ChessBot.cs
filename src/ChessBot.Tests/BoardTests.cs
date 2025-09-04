using ChessBot.Core;
using ChessBot.Core.Enums;
using ChessBot.Core.Models;

namespace ChessBot.Tests;

public class BoardTests
{
    [Theory]
    [InlineData("a1", 0)]
    [InlineData("a2", 1)]
    [InlineData("c3", 18)]
    [InlineData("h8", 63)]
    public void GetIndexByPosition_ReturnsIndex(string position, byte index)
    {
        BoardUtils.GetIndexByPosition(position).ShouldBe(index);
    }
    
    [Theory]
    [InlineData(0,  "a1")]
    [InlineData(1,  "a2")]
    [InlineData(18, "c3")]
    [InlineData(63, "h8")]
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