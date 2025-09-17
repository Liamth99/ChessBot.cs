IPlayer whitePlayer = new HumanPlayer();
IPlayer blackPlayer = new HumanPlayer();

var board = new Board(BoardUtils.GenerateFromFenString());
var consoleBoard = new ConsoleBoard(board);

consoleBoard.ShowSquareIndexes = true;
consoleBoard.ShowValidMoves = true;
consoleBoard.ShowCastleSquares = true;

consoleBoard.WriteToConsole();

while (true)
{
    var move = whitePlayer.GetNextMove(board);
    board.MakeMove(move);
    
    consoleBoard.WriteToConsole();
    
    if(board.IsMate)
        break;

    move = blackPlayer.GetNextMove(board);
    board.MakeMove(move);
    
    consoleBoard.WriteToConsole();
    
    if(board.IsMate)
        break;
}

Console.WriteLine($"{board.ColorToMove.ToColor().ToColor()} Wins!!");
Console.WriteLine("Press any key to exit.");
Console.ReadKey();