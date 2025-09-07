using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessBot.Core;

public static partial class BoardUtils
{
    public static BoardInitSettings GenerateFromFenString(string fenString = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
    {
        var fenParts = fenString.Split(' ');

        if (fenParts.Length is not 6)
        {
            throw new ArgumentException("Fen string should contain all 6 parts to be valid", nameof(fenString));
        }
        
        Piece[] squares = new Piece[64];
        Dictionary<char, Piece> pieceDic = new()
        {
            {'k', Piece.King},
            {'q', Piece.Queen},
            {'n', Piece.Knight},
            {'b', Piece.Bishop},
            {'r', Piece.Rook},
            {'p', Piece.Pawn},
        };

        string fenBoard = fenParts[0];
        int file = 0;
        int rank = 7;
        
        foreach (var symbol in fenBoard)
        {
            if (symbol is '/')
            {
                file = 0;
                rank--;
            }
            else
            {
                if (char.IsDigit(symbol))
                    file += (int)char.GetNumericValue(symbol);

                else
                {
                    var piece = (char.IsUpper(symbol) ? Piece.White : Piece.Black) | pieceDic[char.ToLowerInvariant(symbol)];
                    squares[rank * 8 + file] = piece;
                    file++;
                }
            }
        }

        ulong castleBits = 0;

        if (fenParts[2].Contains('K'))
            castleBits |= Board.WhiteKingCastle;
        if (fenParts[2].Contains('Q'))
            castleBits |= Board.WhiteQueenCastle;
        if (fenParts[2].Contains('k'))
            castleBits |= Board.BlackKingCastle;
        if (fenParts[2].Contains('q'))
            castleBits |= Board.BlackQueenCastle;

        return new BoardInitSettings()
        {
            Squares = squares,
            ColorToMove = fenParts[1] == "w" ? Piece.White : Piece.Black,
            ValidCastleBits = castleBits,
            EnPassantBits = fenParts[3] == "-" ? 0UL : 1UL << GetIndexByPosition(fenParts[3]),
            HalfMoveClock = int.Parse(fenParts[4]),
            FullMoveCount = int.Parse(fenParts[5])
        };
    }
    
    public static string GenerateFenString(Board board)
    {
        if (board is null) 
            throw new ArgumentNullException(nameof(board));

        var parts = new List<string>(6);
        var rows = new List<string>(8);
        
        for (int rank = 7; rank >= 0; rank--)
        {
            int emptyCount = 0;
            StringBuilder row = new(8);

            for (int file = 0; file < 8; file++)
            {
                int index = rank * 8 + file;
                var piece = board[index];

                if (piece == Piece.None)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        row.Append(emptyCount);
                        emptyCount = 0;
                    }

                    char symbol = GetPieceChar(piece);
                    row.Append(symbol);
                }
            }

            if (emptyCount > 0)
                row.Append(emptyCount);

            rows.Add(row.ToString());
        }
        
        parts.Add(string.Join('/', rows));

        parts.Add(board.ColorToMove.IsType(Piece.White) ? "w" : "b");

        StringBuilder castle = new(4);
        
        if ((board.ValidCastleBits & Board.WhiteKingCastle)  > 0) 
            castle.Append('K');
        if ((board.ValidCastleBits & Board.WhiteQueenCastle) > 0) 
            castle.Append('Q');
        if ((board.ValidCastleBits & Board.BlackKingCastle)  > 0) 
            castle.Append('k');
        if ((board.ValidCastleBits & Board.BlackQueenCastle) > 0) 
            castle.Append('q');
        
        parts.Add(castle.Length == 0 ? "-" : castle.ToString());

        if (board.EnPassantBits == 0)
        {
            parts.Add("-");
        }
        else
        {
            int epIndex = BitOperations.TrailingZeroCount(board.EnPassantBits);
            parts.Add(GetPositionByIndex((byte)epIndex));
        }

        parts.Add(board.HalfMoveClock.ToString());
        parts.Add(board.FullMoveCount.ToString());

        return string.Join(' ', parts);

        static char GetPieceChar(Piece piece)
        {
            char c = piece switch
            {
                _ when piece.IsType(Piece.King)   => 'k',
                _ when piece.IsType(Piece.Queen)  => 'q',
                _ when piece.IsType(Piece.Rook)   => 'r',
                _ when piece.IsType(Piece.Bishop) => 'b',
                _ when piece.IsType(Piece.Knight) => 'n',
                _ when piece.IsType(Piece.Pawn)   => 'p',
                _                                  => ' ',
            };

            return piece.IsType(Piece.White) ? char.ToUpperInvariant(c) : c;
        }
    }

    
    public static byte GetIndexByPosition(string position)
    {
        var regex = PositionRegex().Match(position.ToLowerInvariant());
        
        if(!regex.Success)
            throw new ArgumentOutOfRangeException(nameof(position), "Position should match the following regex `^[a-hA-H][1-8]$`");

        byte index = 0;

        switch (regex.Groups["rank"].Value[0])
        {
            case '1':
                break;
            case '2':
                index += 8;
                break;
            case '3':
                index += 16;
                break;
            case '4':
                index += 24;
                break;
            case '5':
                index += 32;
                break;
            case '6':
                index += 40;
                break;
            case '7':
                index += 48;
                break;
            case '8':
                index += 56;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(index), $"Rank '{regex.Groups["rank"].Value[0]}' is not valid");
        }

        switch (regex.Groups["file"].Value[0])
        {
            case 'a':
                break;
            case 'b':
                index += 1;
                break;
            case 'c':
                index += 2;
                break;
            case 'd':
                index += 3;
                break;
            case 'e':
                index += 4;
                break;
            case 'f':
                index += 5;
                break;
            case 'g':
                index += 6;
                break;
            case 'h':
                index += 7;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(index), $"File '{regex.Groups["file"].Value[0]}' is not valid");
        }

        return index;
    }

    public static string GetPositionByIndex(byte index)
    {
        if (index > 63)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 63 inclusive.");

        int rank = index % 8;

        int file = index / 8;

        char rankChar = (char)('a' + rank);
        
        char fileChar = (char)('1' + file);

        return $"{rankChar}{fileChar}";
    }

    [GeneratedRegex("^(?<file>[a-h])(?<rank>[1-8])$", RegexOptions.ExplicitCapture | RegexOptions.Compiled, 500, "en-AU")]
    private static partial Regex PositionRegex();
}