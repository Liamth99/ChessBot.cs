namespace ChessBot.Core.Models;

public partial class Board
{
    public readonly LegalMoveCollection LegalMoves;
    
    public Piece ColorToMove { get; private set; }
    public ulong EnPassantBits { get; private set; }
    public ulong ValidCastleBits { get; private set; }
    public int FullMoveCount { get; private set; }
    public int HalfMoveClock { get; private set; }
    public bool IsCheck { get; private set; }
    public bool IsMate { get; private set; }
    public bool IsDraw { get; private set; }

    private readonly Piece[] _squares;
    public ulong WhitePieces { get; private set; }
    public ulong BlackPieces { get; private set; }
    
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

    public Board(BoardInitSettings? settings = null, bool skipCheckAndLegalMoves = false)
    {
        settings ??= BoardInitSettings.Empty;
        
        _squares        = settings.Squares;
        ColorToMove     = settings.ColorToMove;
        EnPassantBits   = settings.EnPassantBits;
        ValidCastleBits = settings.ValidCastleBits;
        FullMoveCount   = settings.FullMoveCount;
        HalfMoveClock   = settings.HalfMoveClock;
        
        LegalMoves = new(this);

        GenerateLegalMoves(skipCheckAndLegalMoves);
    }

    public Board Clone()
    {
        Piece[] newBoardPieces = new Piece[64];
        
        _squares.CopyTo(newBoardPieces, 0);
        
        return new Board(
            new()
            {
                Squares         = newBoardPieces,
                ColorToMove     = this.ColorToMove,
                EnPassantBits   = this.EnPassantBits,
                FullMoveCount   = this.FullMoveCount,
                HalfMoveClock   = this.HalfMoveClock,
                ValidCastleBits = this.ValidCastleBits
            }, true);
    }

    public Piece this[int index]
    {
        get => _squares[index];
        set => _squares[index] = value;
    }

    public void MakeMove(Move move, bool skipCheckAndLegalMoves = false)
    {
    #if DEBUG
        if (!LegalMoves.FriendlyMoves.Contains(move))
            throw new InvalidOperationException("Move is not a legal move to play");
    #endif
        
        ColorToMove = ColorToMove.ToggleColor();
        EnPassantBits = 0;
        
        if(move == new Move())
        {
            GenerateLegalMoves(skipCheckAndLegalMoves);
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
                // Toggle the bits from the promotion flag and the pawn flag to change the piece type
                _squares[move.TargetSquare] ^= (Piece)((byte)move.Promotion | (byte)Piece.Pawn);
            }
            else
            {
                int movediff = move.TargetSquare - move.StartSquare;
                if (movediff is -16 or 16)
                {
                    EnPassantBits = 0b1UL << (move.TargetSquare - (movediff >> 1));
                }
            }
        }

        if (_squares[move.TargetSquare].IsType(Piece.King))
        {
            if (_squares[move.TargetSquare].IsType(Piece.White))
            {
                ValidCastleBits &= ~(WhiteKingCastle | WhiteQueenCastle);
                
                if (move.CastlingSquare is 07)
                {
                    _squares[07] = Piece.None;
                    _squares[05] = Piece.White | Piece.Rook;
                }
                else if (move.StartSquare is 4 && move.TargetSquare is 2)
                {
                    _squares[0] = Piece.None;
                    _squares[03] = Piece.White | Piece.Rook;
                }
            }
            else
            {
                ValidCastleBits &= ~(BlackKingCastle | BlackQueenCastle);
                
                if (move.CastlingSquare is 63)
                {
                    _squares[63] = Piece.None;
                    _squares[61] = Piece.Black | Piece.Rook;
                }
                else if (move.CastlingSquare is 56)
                {
                    _squares[56] = Piece.None;
                    _squares[59] = Piece.Black | Piece.Rook;
                }
            }
        }

        if (_squares[move.TargetSquare].IsType(Piece.Rook))
        {
            if (_squares[move.TargetSquare].IsType(Piece.White))
            {
                if((move.StartSquare.ToIntBit() & WhiteKingCastle) > 0)
                    ValidCastleBits &= ~WhiteKingCastle;
                else
                    ValidCastleBits &= ~WhiteQueenCastle;
            }
            else
            {
                if((move.StartSquare.ToIntBit() & BlackKingCastle) > 0)
                    ValidCastleBits &= ~BlackKingCastle;
                else
                    ValidCastleBits &= ~BlackQueenCastle;
            }
        }

        GenerateLegalMoves(skipCheckAndLegalMoves);
    }
}