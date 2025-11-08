using System.Collections.Immutable;

namespace ChessBot.Core.Models;

public partial class Board
{
    private static readonly ImmutableArray<short> SlidingDirectionOffsets = [8, -8, -1, 1, 7, -7, 9, -9];
    private static readonly short[][] NumSquaresToEdge = new short[64][];

    public const ulong WhiteKingCastle  = 0b1110_0000UL;
    public const ulong WhiteQueenCastle = 0b0000_1111UL;
    public const ulong BlackKingCastle  = WhiteKingCastle << 56;
    public const ulong BlackQueenCastle = WhiteQueenCastle << 56;

    public const ulong WhiteKingCastleEmpty  = 0b0110_0000UL;
    public const ulong WhiteQueenCastleEmpty = 0b0000_1110UL;
    public const ulong BlackKingCastleEmpty  = WhiteKingCastleEmpty << 56;
    public const ulong BlackQueenCastleEmpty = WhiteQueenCastleEmpty << 56;

    public const ulong WhiteKingCastleSafety  = 0b0110_0000UL;
    public const ulong WhiteQueenCastleSafety = 0b0000_1100UL;
    public const ulong BlackKingCastleSafety  = WhiteKingCastleSafety << 56;
    public const ulong BlackQueenCastleSafety = WhiteQueenCastleSafety << 56;

    public const ulong AllCastleBits = BlackKingCastle | BlackQueenCastle | WhiteKingCastle | WhiteQueenCastle;

    public void GenerateLegalMoves(bool skipCheckAndLegalMoves = false)
    {
        LegalMoves.Clear();
        
        IsCheck = false;
        IsMate  = false;
        IsDraw  = false;

        short friendlyKingPosIndex = 0;
        
        WhitePieces = 0;
        BlackPieces = 0;

        for (byte startSquare = 0; startSquare < 64; startSquare++)
        {
            var piece = _squares[startSquare];

            if (piece.ToColor() is not Piece.None)
            {
                if (piece.IsType(Piece.White))
                    WhitePieces |= startSquare.ToIntBit();
                else
                    BlackPieces |= startSquare.ToIntBit();
                
                if (piece.ContainsAnyType(Piece.SlidingPiece))
                    AddSlidingLegalMoves(startSquare, piece);

                if (piece.IsType(Piece.Knight))
                    AddKnightLegalMoves(startSquare, piece);

                if (piece.IsType(Piece.King))
                {
                    AddKingLegalMoves(startSquare, piece);
                    
                    if (piece.IsType(ColorToMove))
                        friendlyKingPosIndex = startSquare;
                }

                if (piece.IsType(Piece.Pawn))
                    AddPawnLegalMoves(startSquare, piece);
            }
        }
        
        if (ColorToMove is Piece.White)
        {
            IsCheck = ((1UL << friendlyKingPosIndex) & LegalMoves.BlackAttackBits) > 0;

            if (!IsCheck)
            {
                if ((ValidCastleBits & WhiteKingCastle) is not 0 && ((WhitePieces | BlackPieces) & WhiteKingCastleEmpty) is 0 && (WhiteKingCastleSafety & LegalMoves.BlackAttackBits) is 0)
                        LegalMoves.Add(new(04, 06, PromotionFlag.None, 07));

                if ((ValidCastleBits & WhiteQueenCastle) is not 0 && ((WhitePieces | BlackPieces) & WhiteQueenCastleEmpty) is 0 && (WhiteQueenCastleSafety & LegalMoves.BlackAttackBits) is 0)
                        LegalMoves.Add(new(04, 02, PromotionFlag.None, 00));
            }
        }
        else
        {
            IsCheck = ((1UL << friendlyKingPosIndex) & LegalMoves.WhiteAttackBits) > 0;

            if (!IsCheck)
            {
                if ((ValidCastleBits & BlackKingCastle) is not 0 && ((WhitePieces | BlackPieces) & BlackKingCastleEmpty) is 0 && (BlackKingCastleSafety & LegalMoves.WhiteAttackBits) is 0)
                {
                    LegalMoves.Add(new(60, 62, PromotionFlag.None, 63));
                }

                if ((ValidCastleBits & BlackQueenCastle) is not 0 && ((WhitePieces | BlackPieces) & BlackQueenCastleEmpty) is 0 && (BlackQueenCastleSafety & LegalMoves.WhiteAttackBits) is 0)
                {
                    LegalMoves.Add(new(60, 58, PromotionFlag.None, 56));
                }
            }
        }
        
        if(skipCheckAndLegalMoves)
            return;

        List<Move> actuallyValidMoves = new(256);
        Move[] enemyMoves = LegalMoves.EnemyMoves.ToArray();
        
        foreach (var move in LegalMoves.FriendlyMoves)
        {
            var futureBoard = Clone();
            
            futureBoard.MakeMove(move, true);

            bool nextTurnCheck;
            short nextKingPos = friendlyKingPosIndex == move.StartSquare ? move.TargetSquare : friendlyKingPosIndex;
            
            if (ColorToMove is Piece.White)
                nextTurnCheck = ((1UL << nextKingPos) & futureBoard.LegalMoves.BlackAttackBits) > 0;
            else
                nextTurnCheck = ((1UL << nextKingPos) & futureBoard.LegalMoves.WhiteAttackBits) > 0;
            
            if(!nextTurnCheck)
                actuallyValidMoves.Add(move);
        }
        
        LegalMoves.Clear();
        LegalMoves.AddRange(enemyMoves.Concat(actuallyValidMoves));

        if (!LegalMoves.FriendlyMoves.Any())
        {
            if (IsCheck)
                IsMate = true;
            else
                IsDraw = true;

            return;
        }

        if (HalfMoveClock >= 50)
            IsDraw = true;

        if(!IsDraw)
            IsDraw = HasInsufficientMaterial();

        if (!IsDraw)
        {
            // TODO: this is gross i know. Ill fix it later with actual hashing.
            string fen          = BoardUtils.GenerateFenString(this);
            var    fenParts     = fen.Split();
            string positionHash = string.Join(" ", fenParts[0], fenParts[1], fenParts[2], fenParts[3]);

            _positionHistory.Enqueue(positionHash);

            if(_positionHistory.Count(x => x == positionHash) >= 3)
                IsDraw = true;
        }
    }

    private void AddSlidingLegalMoves(byte startSquare, Piece piece)
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

                LegalMoves.Add(new(startSquare, targetSquare));

                if (pieceOnTarget is not Piece.None && piece.ToColor() != pieceOnTarget.ToColor())
                    break;
            }
        }
    }

    private void AddKnightLegalMoves( byte startSquare, Piece piece)
    {
        if (startSquare.File() < 7)
        {
            if (startSquare.Rank() < 6 && !_squares[startSquare + 17].IsType(piece.ToColor()))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 17)));

            if (startSquare.Rank() > 1 && !_squares[startSquare - 15].IsType(piece.ToColor()))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare - 15)));
        }

        if (startSquare.File() < 6)
        {
            if (startSquare.Rank() < 7 && !_squares[startSquare + 10].IsType(piece.ToColor()))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 10)));

            if (startSquare.Rank() > 0 && !_squares[startSquare - 6].IsType(piece.ToColor()))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare - 6)));
        }

        if (startSquare.File() > 1)
        {
            if (startSquare.Rank() < 7 && !_squares[startSquare + 6].IsType(piece.ToColor()))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 6)));

            if (startSquare.Rank() > 0 && !_squares[startSquare - 10].IsType(piece.ToColor()))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare - 10)));
        }

        if (startSquare.File() > 0)
        {
            if (startSquare.Rank() < 6 && !_squares[startSquare + 15].IsType(piece.ToColor()))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 15)));

            if (startSquare.Rank() > 1 && !_squares[startSquare - 17].IsType(piece.ToColor()))
                LegalMoves.Add(new Move(startSquare, (byte)(startSquare - 17)));
        }
    }

    private void AddKingLegalMoves(byte startSquare, Piece piece)
    {
        foreach (var directionOffset in SlidingDirectionOffsets)
        {
            int targetSquare = startSquare + directionOffset;

            if (IsValidKingMove(startSquare, targetSquare, piece))
            {
                LegalMoves.Add(new Move(startSquare, (byte)targetSquare));
            }
        }
    }

    private bool IsValidKingMove(byte startSquare, int targetSquare, Piece piece)
    {
        if (targetSquare is < 0 or >= 64)
            return false;

        if (_squares[targetSquare].IsType(piece.ToColor()))
            return false;

        int horizontalDistance = Math.Abs(startSquare % 8 - targetSquare % 8);
        return horizontalDistance <= 1;
    }

    private void AddPawnLegalMoves(byte startSquare, Piece piece)
    {
        short directionMultiplier = (short)(piece.ToColor() is Piece.White ? 1 : -1);
        int targetSquare = startSquare + 8 * directionMultiplier;

        if (targetSquare is < 0 or > 63)
            return;

        byte targetByte = (byte)targetSquare;

        // Generate diagonal attacks
        AddPawnAttacks(startSquare, directionMultiplier, piece);

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

                if (targetSquare is < 8 or > 55)
                    return;

                bool isStartingRank = (piece.ToColor() == Piece.White && startSquare.Rank() == 1) ||
                                      (piece.ToColor() == Piece.Black && startSquare.Rank() == 6);

                if (isStartingRank && _squares[startSquare + 16 * directionMultiplier] == Piece.None)
                {
                    LegalMoves.Add(new Move(startSquare, (byte)(startSquare + 16 * directionMultiplier)));
                }
            }
        }
    }

    private void AddPawnAttacks(byte startSquare, short directionMultiplier, Piece piece)
    {
        // Left diagonal attack
        if (startSquare.File() > 0)
        {
            int leftAttackSquare = startSquare + (7 * directionMultiplier);
            if (leftAttackSquare is >= 0 and <= 63)
            {
                var pieceOnLeftAttack = _squares[leftAttackSquare];
                if (pieceOnLeftAttack != Piece.None && pieceOnLeftAttack.ToColor() != piece.ToColor() ||
                    (EnPassantBits & (0b1UL << leftAttackSquare)) > 0)
                {
                    byte leftAttackByte = (byte)leftAttackSquare;
                    
                    if(startSquare.File() - leftAttackByte.File() is > 1 or < -1)
                        return;

                    // Check for promotion on attack
                    if (leftAttackByte.Rank() is 0 or 7)
                    {
                        LegalMoves.Add(new Move(startSquare, leftAttackByte, PromotionFlag.Queen));
                        LegalMoves.Add(new Move(startSquare, leftAttackByte, PromotionFlag.Rook));
                        LegalMoves.Add(new Move(startSquare, leftAttackByte, PromotionFlag.Bishop));
                        LegalMoves.Add(new Move(startSquare, leftAttackByte, PromotionFlag.Knight));
                    }
                    else
                        LegalMoves.Add(new Move(startSquare, leftAttackByte));
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
                if (pieceOnRightAttack != Piece.None && pieceOnRightAttack.ToColor() != piece.ToColor() ||
                    (EnPassantBits & (0b1UL << rightAttackSquare)) > 0)
                {
                    byte rightAttackByte = (byte)rightAttackSquare;
                    
                    if(startSquare.File() - rightAttackByte.File() is > 1 or < -1)
                        return;
                    
                    if (rightAttackByte.Rank() is 0 or 7)
                    {
                        LegalMoves.Add(new Move(startSquare, rightAttackByte, PromotionFlag.Queen));
                        LegalMoves.Add(new Move(startSquare, rightAttackByte, PromotionFlag.Rook));
                        LegalMoves.Add(new Move(startSquare, rightAttackByte, PromotionFlag.Bishop));
                        LegalMoves.Add(new Move(startSquare, rightAttackByte, PromotionFlag.Knight));
                    }
                    else
                        LegalMoves.Add(new Move(startSquare, rightAttackByte));
                }
            }
        }
    }
}