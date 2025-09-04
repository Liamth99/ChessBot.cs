namespace ChessBot.Tests;

public class MoveTests
{
    [Theory]
    [InlineData(00, 00, PromotionFlag.None)]
    [InlineData(01, 00, PromotionFlag.None)]
    [InlineData(00, 01, PromotionFlag.None)]
    [InlineData(01, 01, PromotionFlag.None)]
    [InlineData(00, 00, PromotionFlag.Rook)]
    [InlineData(63, 10, PromotionFlag.Knight)]
    public void MoveArguments_byte_EqualProperties(byte start, byte end, PromotionFlag promotion)
    {
        var move = new Move(start, end, promotion);
        
        move.StartSquare.ShouldBe(start);
        move.TargetSquare.ShouldBe(end);
        move.Promotion.ShouldBe(promotion);
    }
    
    [Theory]
    [InlineData(00, 00, PromotionFlag.None)]
    [InlineData(01, 00, PromotionFlag.None)]
    [InlineData(00, 01, PromotionFlag.None)]
    [InlineData(01, 01, PromotionFlag.None)]
    [InlineData(00, 00, PromotionFlag.Rook)]
    [InlineData(63, 10, PromotionFlag.Knight)]
    public void MoveArguments_Int_EqualProperties(int start, int end, PromotionFlag promotion)
    {
        var move = new Move(start, end, promotion);
        
        move.StartSquare.ShouldBe((byte)start);
        move.TargetSquare.ShouldBe((byte)end);
        move.Promotion.ShouldBe(promotion);
    }
}