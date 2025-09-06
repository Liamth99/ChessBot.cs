IPlayer whitePlayer = new HumanPlayer();
IPlayer blackPlayer = new RandomMovePlayer();

var board = new Board(BoardUtils.GenerateFromFenString());
var consoleBoard = new ConsoleBoard(board);

while (true)
{
    consoleBoard.WriteToConsole();

    var move = whitePlayer.GetNextMove(board);
    board.MakeMove(move);
    
    consoleBoard.WriteToConsole();

    move = blackPlayer.GetNextMove(board);
    board.MakeMove(move);
}