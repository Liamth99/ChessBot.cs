using System.Diagnostics;

namespace ChessBot.Core.Models;

[DebuggerDisplay("{DebugString}")]
public partial class Board
{
    private string DebugString => BoardUtils.GenerateFenString(this);
    
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

        if (ColorToMove == Piece.White)
            FullMoveCount++;

        HalfMoveClock++;

        var movedPiece = _squares[move.StartSquare];
        var pieceOnTargetBeforeMove = _squares[move.TargetSquare];
        bool isCapture = pieceOnTargetBeforeMove is not Piece.None;

        if (isCapture)
            HalfMoveClock = 0;
        
        // Clear castling rights if a rook is captured on its original corner
        if (isCapture && pieceOnTargetBeforeMove.IsType(Piece.Rook))
        {
            switch (move.TargetSquare)
            {
                case 07: ValidCastleBits &= ~WhiteKingCastle; break;
                case 00: ValidCastleBits &= ~WhiteQueenCastle; break;
                case 63: ValidCastleBits &= ~BlackKingCastle; break;
                case 56: ValidCastleBits &= ~BlackQueenCastle; break;
            }
        }
        
        _squares[move.TargetSquare] = movedPiece;
        _squares[move.StartSquare] = Piece.None;

        if (movedPiece.IsType(Piece.Pawn))
        {
            HalfMoveClock = 0;
            
            if (move.TargetSquare.Rank() is 0 or 7)
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

            var dif = move.TargetSquare - move.StartSquare;
            
            switch (dif)
            {
                case 9 or -7:
                    _squares[move.StartSquare + 1] = Piece.None;
                    break;
                
                case 7 or -9:
                    _squares[move.StartSquare - 1] = Piece.None;
                    break;
            }
        }

        if (movedPiece.IsType(Piece.King))
        {
            if (movedPiece.IsType(Piece.White))
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

        if (movedPiece.IsType(Piece.Rook))
        {
            if (movedPiece.IsType(Piece.White))
            {
                if(move.StartSquare is 07)
                    ValidCastleBits &= ~WhiteKingCastle;
                else if(move.StartSquare is 0)
                    ValidCastleBits &= ~WhiteQueenCastle;
            }
            else
            {
                if(move.StartSquare is 63)
                    ValidCastleBits &= ~BlackKingCastle;
                else if (move.StartSquare is 56)
                    ValidCastleBits &= ~BlackQueenCastle;
            }
        }

        GenerateLegalMoves(skipCheckAndLegalMoves);
    }
    
    private bool HasInsufficientMaterial()
    {
        var wPawns   = _squares.Count(x => x.IsType(Piece.White | Piece.Pawn));
        var wRooks   = _squares.Count(x => x.IsType(Piece.White | Piece.Rook));
        var wQueens  = _squares.Count(x => x.IsType(Piece.White | Piece.Queen));
        var wBishops = _squares.Count(x => x.IsType(Piece.White | Piece.Bishop));
        var wKnights = _squares.Count(x => x.IsType(Piece.White | Piece.Knight));
        
        var bPawns   = _squares.Count(x => x.IsType(Piece.Black | Piece.Pawn));
        var bRooks   = _squares.Count(x => x.IsType(Piece.Black | Piece.Rook));
        var bQueens  = _squares.Count(x => x.IsType(Piece.Black | Piece.Queen));
        var bBishops = _squares.Count(x => x.IsType(Piece.Black | Piece.Bishop));
        var bKnights = _squares.Count(x => x.IsType(Piece.Black | Piece.Knight));

        if (wPawns + bPawns > 0) 
            return false;
        
        if (wRooks + bRooks > 0) 
            return false;
        
        if (wQueens + bQueens > 0) 
            return false;

        int wMinors = wBishops + wKnights;
        int bMinors = bBishops + bKnights;
        int totalMinors = wMinors + bMinors;

        if (totalMinors == 0) 
            return true;

        if (totalMinors == 1) 
            return true;

        if (totalMinors == 2)
        {
            if (wMinors == 2)
            {
                if (wBishops >= 2 || (wBishops >= 1 && wKnights >= 1)) 
                    return false;
                
                return true;
            }
            if (bMinors == 2)
            {
                if (bBishops >= 2 || (bBishops >= 1 && bKnights >= 1)) 
                    return false;
            }
            
            return true;
        }

        return false;
    }
}