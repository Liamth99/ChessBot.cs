namespace ChessBot;

public class ConsoleBoard
{
    public Board Board { get; set; }

    public ConsoleBoard(Board board)
    {
        Board = board;
    }
    
    public bool ShowSquareIndexes { get; set; } = false;
    public bool ShowValidMoves { get; set; } = false;
    public bool RotateBoard { get; set; } = false;
    public bool RotateBoardEachMove { get; set; } = false;
    public bool ShowEnpassantSquares { get; set; } = false;

    private static readonly Lock _lock = new Lock();
    
    public void WriteToConsole()
    {
        lock (_lock)
        {
            if (RotateBoardEachMove)
                RotateBoard = Board.ColorToMove.IsType(Piece.Black);
            
            Board.GenerateLegalMoves();
            
            Console.ResetColor();

            bool isAlt = false;

            // Determine row iteration based on rotation setting
            int startRow = RotateBoard ? 0 : 7;
            int endRow = RotateBoard ? 7 : 0;
            int rowStep = RotateBoard ? 1 : -1;

            for (int i = startRow; i != endRow + rowStep; i += rowStep)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Write($" {i + 1} ");

                // Determine column iteration based on rotation setting
                int startCol = RotateBoard ? 7 : 0;
                int endCol = RotateBoard ? 0 : 7;
                int colStep = RotateBoard ? -1 : 1;

                for (int j = startCol; j != endCol + colStep; j += colStep)
                {
                    Console.BackgroundColor = GetSquareColor(isAlt);
                    Console.Write(" ");

                    var piece = Board[i * 8 + j];

                    if ((piece & Piece.White) == Piece.White)
                        Console.ForegroundColor = ConsoleColor.White;

                    else if ((piece & Piece.Black) == Piece.Black)
                        Console.ForegroundColor = ConsoleColor.Black;
                    
                    if (ShowValidMoves && Board.LegalMoves.Any(x => x.TargetSquare == i * 8 + j))
                        Console.BackgroundColor = ConsoleColor.Green;
                    
                    
                    if(ShowEnpassantSquares && (Board.EnpassantBits & (0b1L << (i * 8 + j))) > 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("é");
                    }                
                    else    
                        Console.Write(piece.ToPieceCharacter());

                    Console.ResetColor();
                    Console.BackgroundColor = GetSquareColor(isAlt);
                    Console.Write(" ");
                    isAlt = !isAlt;
                }

                if (ShowSquareIndexes)
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(" | ");
                    Console.ResetColor();

                    // Determine column iteration based on rotation setting for index display
                    int startIdxCol = RotateBoard ? 7 : 0;
                    int endIdxCol = RotateBoard ? 0 : 7;
                    int idxColStep = RotateBoard ? -1 : 1;

                    for (int j = startIdxCol; j != endIdxCol + idxColStep; j += idxColStep)
                    {
                        var piece = Board[i * 8 + j];

                        if ((piece & Piece.White) == Piece.White)
                            Console.ForegroundColor = ConsoleColor.White;

                        else if ((piece & Piece.Black) == Piece.Black)
                            Console.ForegroundColor = ConsoleColor.Black;

                        else
                            Console.ForegroundColor = ConsoleColor.DarkGray;

                        if (ShowValidMoves)
                        {
                            var prevColor = Console.ForegroundColor;
                            
                            if(Board.LegalMoves.Any(x => x.TargetSquare == (byte)(i * 8 + j)))
                                Console.ForegroundColor = ConsoleColor.Green;
                            
                            Console.Write("[");
                            Console.ForegroundColor = prevColor;
                        }
                        
                        Console.Write($"{i * 8 + j:00}:");
                        if(ShowEnpassantSquares && (Board.EnpassantBits & (0b1L << (i * 8 + j))) > 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("é");
                        }
                        else
                        {
                            Console.Write(piece.ToPieceCharacter());
                        }

                        if (ShowValidMoves)
                        {
                            if(Board.LegalMoves.Any(x => x.TargetSquare == (byte)(i * 8 + j)))
                                Console.ForegroundColor = ConsoleColor.Green;
                            
                            Console.Write("]");
                        }
                        
                        Console.ResetColor();
                    }
                }

                isAlt = !isAlt;
                Console.WriteLine();
            }

            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            if (RotateBoard)
                Console.WriteLine("    h  g  f  e  d  c  b  a  ");
            else
                Console.WriteLine("    a  b  c  d  e  f  g  h  ");
        }
    }

    private ConsoleColor GetSquareColor(bool alt) => alt ? ConsoleColor.DarkGray : ConsoleColor.Gray;
}