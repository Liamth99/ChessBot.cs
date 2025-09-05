using System.Text.RegularExpressions;
using ChessBot.Core.Enums;

namespace ChessBot.Core;

public static partial class BoardUtils
{
    public static Piece[] GenerateFromFenString(string fenString = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")
    {
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

        string fenBoard = fenString.Split(' ')[0];
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

        return squares;
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

        int rank = index / 8;

        int file = index % 8;

        char rankChar = (char)('a' + rank);
        
        char fileChar = (char)('1' + file);

        return $"{rankChar}{fileChar}";
    }

    [GeneratedRegex("^(?<file>[a-h])(?<rank>[1-8])$", RegexOptions.ExplicitCapture | RegexOptions.Compiled, 500, "en-AU")]
    private static partial Regex PositionRegex();
}