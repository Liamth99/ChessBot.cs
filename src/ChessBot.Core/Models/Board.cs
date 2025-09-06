namespace ChessBot.Core.Models;

public partial class Board
{
    public readonly LegalMoveCollection LegalMoves;
    public Piece ColorToMove => _colorToMove;
    public long EnpassantBits => _enpassantBits;
    
    private readonly Piece[] _squares = new Piece[64];
    private Piece _colorToMove = Piece.White;

    private long _enpassantBits;

    static Board()
    {
        for (short file = 0; file < 8; file++)
        {
            for (short rank = 0; rank < 8; rank++)
            {
                short numNorth = (short)(7 - rank);
                short numSouth = rank;
                short numWest  = file;
                short numEast  = (short)(7 - file);

                int squareIndex = rank * 8 + file;

                NumSquaresToEdge[squareIndex] =
                [
                    numNorth,
                    numSouth,
                    numWest,
                    numEast,
                    short.Min(numNorth, numWest),
                    short.Min(numSouth, numEast),
                    short.Min(numNorth, numEast),
                    short.Min(numSouth, numWest),
                ];
            }
        }
    }

    public Board(Piece[]? board = null, Piece colorToMove = Piece.White)
    {
        LegalMoves = new(this);
        board ??= new Piece[64];
        
        if (board.Length is not 64)
            throw new IndexOutOfRangeException("Board must contain 64 squares");

        _squares = board;
        _colorToMove = colorToMove & (Piece.Black | Piece.White);

        GenerateLegalMoves();
    }

    public Piece this[int index]
    {
        get => _squares[index];
        set => _squares[index] = value;
    }

    public void MakeMove(Move move)
    {
        _colorToMove = _colorToMove.ToggleColor();
        _enpassantBits = 0;
        
        if(move == new Move())
        {
            GenerateLegalMoves();
            return;
        }
        
        _squares[move.TargetSquare] = _squares[move.StartSquare];
        _squares[move.StartSquare] = Piece.None;

        if (_squares[move.TargetSquare].IsType(Piece.Pawn))
        {
            
            if (move.TargetSquare.Rank() is 0 or 8)
            {
                // Toggle the bits from the promotion flag and the pown flag to change the piece type
                _squares[move.TargetSquare] ^= (Piece)((byte)move.Promotion | (byte)Piece.Pawn);
            }
            else
            {
                int movediff = move.TargetSquare - move.StartSquare;
                if (movediff is -16 or 16)
                {
                    _enpassantBits = 0b1L << (move.TargetSquare - (movediff >> 1));
                }
            }
        }
        
        GenerateLegalMoves();
    }
}