IPlayer whitePlayer = new HumanPlayer();
IPlayer blackPlayer = new HumanPlayer();

var board = new Board(BoardUtils.GenerateFromFenString("r3k2r/p1ppPpb1/bn2pnp1/4N3/4P3/1pN2Q1p/PPPBBPPP/R3K2R b KQkq - 0 2"));

var consoleBoard = new ConsoleBoard(board);

consoleBoard.ShowSquareIndexes    = true;
consoleBoard.ShowValidMoves       = true;
consoleBoard.ShowCastleSquares    = true;
consoleBoard.ShowEnpassantSquares = true;

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