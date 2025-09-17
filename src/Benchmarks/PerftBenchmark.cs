using System.Collections.Concurrent;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class PerftBenchmark
{
    [Params(values: [1, 2, 3])]
    public int Depth { get; set; }

    private static readonly ConcurrentDictionary<int, BigInteger> LastResults = [];

    [Benchmark]
    public BigInteger PerftBenchMark() => Perft(new Board(BoardUtils.GenerateFromFenString()), Depth);
    
    private BigInteger Perft(Board board, int depth)
    {
        if (depth == 0)
            return 1;

        board.GenerateLegalMoves();

        if (depth == 1)
            return board.LegalMoves.FriendlyMoves.Count();

        BigInteger nodes = 0;

        foreach (var move in board.LegalMoves.FriendlyMoves)
        {
            var next = board.Clone();
            next.MakeMove(move, true);
            nodes += Perft(next, depth - 1);
        }

        return nodes;
    }
}