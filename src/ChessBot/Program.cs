IPlayer whitePlayer = new HumanPlayer();
IPlayer blackPlayer = new HumanPlayer();

var board = new Board(BoardUtils.GenerateFromFenString());
var consoleBoard = new ConsoleBoard(board);

consoleBoard.ShowSquareIndexes = true;
consoleBoard.ShowValidMoves = true;

while (true)
{
    consoleBoard.WriteToConsole();

    var move = whitePlayer.GetNextMove(board);
    board.MakeMove(move);
    
    consoleBoard.WriteToConsole();

    move = blackPlayer.GetNextMove(board);
    board.MakeMove(move);
}