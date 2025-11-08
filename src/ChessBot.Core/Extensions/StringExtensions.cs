using System.Text.RegularExpressions;

namespace ChessBot.Core.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex("^(?<file>[a-h])(?<rank>[1-8])$", RegexOptions.ExplicitCapture | RegexOptions.Compiled, 500, "en-AU")]
    private static partial Regex PositionRegex();

    public static byte GetIndexByPosition(this string position)
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
}