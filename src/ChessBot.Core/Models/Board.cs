using System.Collections.Immutable;

namespace ChessBot.Core.Models;

public class Board
{
    public List<Move> LegalMoves = [];
    public Piece ColorToMove => _colorToMove;
    public long EnpassantBits => _enpassantBits;

    
    private readonly Piece[] _squares = new Piece[64];
    private Piece _colorToMove = Piece.White;

    private long _enpassantBits;
    
    public static readonly ImmutableArray<short> SlidingDirectionOffsets = [8, -8, -1, 1, 7, -7, 9, -9];
    
    public static readonly short[][] NumSquaresToEdge = new short[64][];

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
            return;
        
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
        
    }

    public void GenerateLegalMoves()
    {
        LegalMoves.Clear();

        for (byte startSquare = 0; startSquare < 64; startSquare++)
        {
            var piece = _squares[startSquare];

            if (piece.ToColor() == _colorToMove)
            {
                if (piece.ContainsAnyType(Piece.SlidingPiece))
                    GenerateSlidingMoves(startSquare, piece);
                
                if (piece.IsType(Piece.Knight))
                    GenerateKnightMoves(startSquare, piece);
                
                if (piece.IsType(Piece.King))
                    GenerateKingMoves(startSquare);
                
                if(piece.IsType(Piece.Pawn))
                    GeneratePawnMoves(startSquare);
            }
        }
    }

    private void GenerateSlidingMoves(byte startSquare, Piece piece)
    {
        byte startDirIndex = (byte)(piece.IsType(Piece.Bishop) ? 4 : 0);
        byte endDirIndex = (byte)(piece.IsType(Piece.Rook) ? 4 : 8);
        
        for (int directionIndex = startDirIndex; directionIndex < endDirIndex; directionIndex++)
        {
            for (int n = 0; n < NumSquaresToEdge[startSquare][directionIndex]; n++)
            {
                byte targetSquare = (byte)(startSquare + SlidingDirectionOffsets[directionIndex] * (n + 1));
                var pieceOnTarget = _squares[targetSquare];
                
                if (piece.ToColor() == pieceOnTarget.ToColor())
                    break;
                
                LegalMoves.Add(new (startSquare, targetSquare));

                if (pieceOnTarget is not Piece.None && piece.ToColor() != pieceOnTarget.ToColor())
                    break;
            }
        }
    }
    
    private void GenerateKnightMoves(byte startSquare, Piece piece)
    {
        if (startSquare.File() < 7)
        {
            if(startSquare.Rank() < 6 && !_squares[startSquare + 17].IsType(_colorToMove)) // something along the lines of (this doesnt work) -> && ((_squares[startSquare + 8] | _squares[startSquare + 16]) & (piece & (Piece.Black | Piece.White)).ToggleColor()) == 0
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 17)));
            
            if(startSquare.Rank() > 0 && !_squares[startSquare - 15].IsType(_colorToMove))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare - 15)));
        }
        
        if (startSquare.File() < 6)
        {
            if(startSquare.Rank() < 7 && !_squares[startSquare + 10].IsType(_colorToMove))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 10)));
            
            if(startSquare.Rank() > 1 && !_squares[startSquare - 6].IsType(_colorToMove))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare - 6)));
        }

        if (startSquare.File() > 1)
        {
            if(startSquare.Rank() < 7 && !_squares[startSquare + 6].IsType(_colorToMove))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 6)));
            
            if(startSquare.Rank() > 1 && !_squares[startSquare -10].IsType(_colorToMove))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare - 10)));
        }
        
        if (startSquare.File() > 0)
        {
            if(startSquare.Rank() < 6 && !_squares[startSquare + 15].IsType(_colorToMove))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 15)));
            
            if(startSquare.Rank() > 0 && !_squares[startSquare - 17].IsType(_colorToMove))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare - 17)));
        }
    }

    private void GenerateKingMoves(byte startSquare)
    {
        foreach (var directionOffset in SlidingDirectionOffsets)
        {
            int targetSquare = startSquare + directionOffset;
        
            if (IsValidKingMove(startSquare, targetSquare))
            {
                LegalMoves.Add(new Move(startSquare, (byte)targetSquare));
            }
        }
    }

    private bool IsValidKingMove(byte startSquare, int targetSquare)
    {
        if (targetSquare is < 0 or >= 64)
            return false;
    
        if (_squares[targetSquare].IsType(_colorToMove))
            return false;
    
        int horizontalDistance = Math.Abs(startSquare % 8 - targetSquare % 8);
        return horizontalDistance <= 1;
    }
    
    private void GeneratePawnMoves(byte startSquare)
    {
        short directionMultiplier = (short)(_colorToMove is Piece.White ? 1 : -1);
        int targetSquare = startSquare + 8 * directionMultiplier;
        
        if(targetSquare is < 0 or > 63)
            return;

        byte targetByte = (byte)targetSquare;

        // Generate diagonal attacks
        GeneratePawnAttacks(startSquare, directionMultiplier);

        if (_squares[targetByte] is Piece.None)
        {
            if (targetByte.Rank() is 0 or 7)
            {
                LegalMoves.Add(new Move(startSquare, targetByte, PromotionFlag.Queen));
                LegalMoves.Add(new Move(startSquare, targetByte, PromotionFlag.Rook));
                LegalMoves.Add(new Move(startSquare, targetByte, PromotionFlag.Bishop));
                LegalMoves.Add(new Move(startSquare, targetByte, PromotionFlag.Knight));
            }
            else
            {
                LegalMoves.Add(new Move(startSquare, targetByte));
            
                if(targetSquare is < 8 or > 55)
                    return;

                bool isStartingRank = (_colorToMove == Piece.White && startSquare.Rank() == 1) || (_colorToMove == Piece.Black && startSquare.Rank() == 6);

                if (isStartingRank && _squares[startSquare + 16 * directionMultiplier] == Piece.None)
                {
                    LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 16 * directionMultiplier)));
                }
            }
        }
    }

    private void GeneratePawnAttacks(byte startSquare, short directionMultiplier)
    {
        // Left diagonal attack
        if (startSquare.File() > 0)
        {
            int leftAttackSquare = startSquare + (7 * directionMultiplier);
            if (leftAttackSquare is >= 0 and <= 63)
            {
                var pieceOnLeftAttack = _squares[leftAttackSquare];
                if (pieceOnLeftAttack != Piece.None && pieceOnLeftAttack.ToColor() != _colorToMove || (_enpassantBits & (0b1L << leftAttackSquare)) > 0)
                {
                    byte leftAttackByte = (byte)leftAttackSquare;
                
                    // Check for promotion on attack
                    if (leftAttackByte.Rank() is 0 or 7)
                    {
                        LegalMoves.Add(new Move(startSquare, leftAttackByte, PromotionFlag.Queen));
                        LegalMoves.Add(new Move(startSquare, leftAttackByte, PromotionFlag.Rook));
                        LegalMoves.Add(new Move(startSquare, leftAttackByte, PromotionFlag.Bishop));
                        LegalMoves.Add(new Move(startSquare, leftAttackByte, PromotionFlag.Knight));
                    }
                    else
                    {
                        LegalMoves.Add(new Move(startSquare, leftAttackByte));
                    }
                }
            }
        }
    
        // Right diagonal attack
        if (startSquare.File() < 7)
        {
            int rightAttackSquare = startSquare + (9 * directionMultiplier);
            if (rightAttackSquare is >= 0 and <= 63)
            {
                var pieceOnRightAttack = _squares[rightAttackSquare];
                if (pieceOnRightAttack != Piece.None && pieceOnRightAttack.ToColor() != _colorToMove || (_enpassantBits & (0b1L << rightAttackSquare)) > 0)
                {
                    byte rightAttackByte = (byte)rightAttackSquare;
                
                    // Check for promotion on attack
                    if (rightAttackByte.Rank() is 0 or 7)
                    {
                        LegalMoves.Add(new Move(startSquare, rightAttackByte, PromotionFlag.Queen));
                        LegalMoves.Add(new Move(startSquare, rightAttackByte, PromotionFlag.Rook));
                        LegalMoves.Add(new Move(startSquare, rightAttackByte, PromotionFlag.Bishop));
                        LegalMoves.Add(new Move(startSquare, rightAttackByte, PromotionFlag.Knight));
                    }
                    else
                    {
                        LegalMoves.Add(new Move(startSquare, rightAttackByte));
                    }
                }
            }
        }
    }
}