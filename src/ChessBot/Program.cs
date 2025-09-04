var board = new Board();

var consoleBoard = new ConsoleBoard(board)
{
    ShowSquareIndexes = true,
    ShowValidMoves = true,
    RotateBoardEachMove = false,
    ShowEnpassantSquares = true
};

//board.Squares = BoardUtils.GenerateFromFenString();



// Set up scenario: White pawn on e5 with black pawns that moved to both adjacent files
board[36] = Piece.White | Piece.Pawn; // e5
board[51] = Piece.Black | Piece.Pawn; // d7
board[53] = Piece.Black | Piece.Pawn; // f7
        
// Make a null move to switch to black
board.MakeMove(new Move());
        
// Black pawn d7 moves to d5
board.MakeMove(new Move(51, 35)); // d7 to d5
        
// White makes any move
board.MakeMove(new Move(36, 44)); // e5 to e6
        
// Black pawn f7 moves to f5 
board.MakeMove(new Move(53, 37)); // f7 to f5
        
board.GenerateLegalMoves();

consoleBoard.WriteToConsole();

// Write legal moves as code

var moveString = string.Join(", ",
    board.LegalMoves.OrderBy(x => x.StartSquare).ThenBy(x => x.TargetSquare)
        .Select(x => $"new({x.StartSquare:00}, {x.TargetSquare:00}, PromotionFlag.{x.Promotion})"));

Console.WriteLine($"board.LegalMoves.OrderBy(x => x.StartSquare).ThenBy(x => x.TargetSquare).ShouldBe([{moveString}]);");
