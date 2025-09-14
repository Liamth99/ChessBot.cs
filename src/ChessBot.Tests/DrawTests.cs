namespace ChessBot.Tests;

public class DrawTests
{
    [Fact]
    public void Draw_Insufficient_K_vs_K()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/4K3 w - - 0 1"));
        Assert.True(board.IsDraw);
    }

    [Fact]
    public void Draw_Insufficient_K_vs_KB()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/2B1K3 w - - 0 1"));
        Assert.True(board.IsDraw);
    }

    [Fact]
    public void Draw_Insufficient_K_vs_KN()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/2N1K3 w - - 0 1"));
        Assert.True(board.IsDraw);
    }

    [Fact]
    public void Draw_Insufficient_KB_vs_KB()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("2b1k3/8/8/8/8/8/8/2B1K3 w - - 0 1"));
        Assert.True(board.IsDraw);
    }

    [Fact]
    public void Draw_Insufficient_KNN_vs_K()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/1NN1K3 w - - 0 1"));
        Assert.True(board.IsDraw);
    }

    [Fact]
    public void NotDraw_Sufficient_KBN_vs_K()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/2NBK3 w - - 0 1"));
        Assert.False(board.IsDraw);
    }

    [Fact]
    public void NotDraw_Sufficient_KP_vs_K()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/P7/4K3 w - - 0 1"));
        Assert.False(board.IsDraw);
    }

    [Fact]
    public void NotDraw_Sufficient_KQ_vs_K()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/3QK3 w - - 0 1"));
        Assert.False(board.IsDraw);
    }

    [Fact]
    public void NotDraw_Sufficient_KR_vs_K()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/3RK3 w - - 0 1"));
        Assert.False(board.IsDraw);
    }

    [Fact]
    public void NotDraw_Sufficient_KBB_vs_K()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/1BB1K3 w - - 0 1"));
        Assert.False(board.IsDraw);
    }

    [Fact]
    public void NotDraw_Sufficient_KNN_vs_KN()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/1NN1K2n w - - 0 1"));
        Assert.False(board.IsDraw);
    }

}