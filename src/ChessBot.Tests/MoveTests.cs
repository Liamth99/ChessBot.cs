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

    [Theory]
    // start, end, promotion, castling
    [InlineData(0b00000000, 0b00000000, PromotionFlag.None, 0b00000000)]
    [InlineData(0b00000001, 0b00000010, PromotionFlag.None, 0b00000000)]
    [InlineData(0b00000100, 0b00000110, PromotionFlag.None, 0b00000111)]     // e1g1 castle rook at h1
    [InlineData(0b00111111, 0b00001010, PromotionFlag.Knight, 0b00000000)]   // 63 -> 10, promote Knight
    [InlineData(0b00110100, 0b00101100, PromotionFlag.Queen, 0b00000000)]
    public void GetHashCode_MatchesPackedBits_ByteCtor(byte start, byte end, PromotionFlag promotion, byte castling)
    {
        var move = new Move(start, end, promotion, castling);

        // Expected layout: [castling (8)] [promotion (8)] [start (8)] [end (8)]
        int expected = (castling << 24) | ((int)promotion << 16) | (start << 8) | end;

        move.GetHashCode().ShouldBe(expected);
    }

    [Theory]
    // start, end, promotion, castling
    [InlineData(0b00000000, 0b00000000, PromotionFlag.None, 0b00000000)]
    [InlineData(0b00001000, 0b00010000, PromotionFlag.None, 0b00000000)]
    [InlineData(0b00111100, 0b00101100, PromotionFlag.Bishop, 0b00000000)]
    [InlineData(0b00111100, 0b01000000, PromotionFlag.None, 0b00111111)]     // e8g8 castle rook at h8
    [InlineData(0b00000111, 0b00000000, PromotionFlag.Queen, 0b00000111)]    // a-side castle encoding example
    public void GetHashCode_MatchesPackedBits_IntCtor(int start, int end, PromotionFlag promotion, int castling)
    {
        var move = new Move(start, end, promotion, castling);

        int expected = (castling << 24) | ((int)promotion << 16) | (start << 8) | (end & 0xFF);

        move.GetHashCode().ShouldBe(expected);
    }
}