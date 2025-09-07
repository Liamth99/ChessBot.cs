namespace ChessBot.Core.Models;

public partial class Board
{
    public readonly LegalMoveCollection LegalMoves;
    
    public Piece ColorToMove { get; private set; }
    public long EnPassantBits { get; private set; }
    public long ValidCastleBits { get; private set; }
    public int FullMoveCount { get; private set; }
    public int HalfMoveClock { get; private set; }

    private readonly Piece[] _squares;


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

    public Board(BoardInitSettings? settings = null)
    {
        settings ??= BoardInitSettings.Empty;
        
        _squares        = settings.Squares;
        ColorToMove     = settings.ColorToMove;
        EnPassantBits   = settings.EnPassantBits;
        ValidCastleBits = settings.ValidCastleBits;
        FullMoveCount   = settings.FullMoveCount;
        HalfMoveClock   = settings.HalfMoveClock;
        
        LegalMoves = new(this);

        GenerateLegalMoves();
    }

    public Piece this[int index]
    {
        get => _squares[index];
        set => _squares[index] = value;
    }

    public void MakeMove(Move move)
    {
        ColorToMove = ColorToMove.ToggleColor();
        EnPassantBits = 0;
        
        if(move == new Move())
        {
            GenerateLegalMoves();
            return;
        }

        FullMoveCount++;
        HalfMoveClock++;

        bool isCapture = _squares[move.TargetSquare] is not Piece.None;

        if (isCapture)
            HalfMoveClock = 0;
        
        _squares[move.TargetSquare] = _squares[move.StartSquare];
        _squares[move.StartSquare] = Piece.None;

        if (_squares[move.TargetSquare].IsType(Piece.Pawn))
        {
            HalfMoveClock = 0;
            
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
                    EnPassantBits = 0b1L << (move.TargetSquare - (movediff >> 1));
                }
            }
        }
        
        GenerateLegalMoves();
    }
}