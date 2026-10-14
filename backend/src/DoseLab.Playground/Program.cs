using DoseLab.Algorithms;


Console.WriteLine(BinarySearch.IndexOf(new[] {1,3,6,8,9}, 3)); // 1
Console.WriteLine(BinarySearch.IndexOf(new[] {0, 1, 2, 3, 7}, 4)); // -1

Console.WriteLine(BracketMatcher.IsBalanced("(})")); // False
Console.WriteLine(BracketMatcher.IsBalanced("({[]})")); // True

Console.WriteLine(CoinChange.MinimumCoins(new[] {1,4,10}, 11)); //2
Console.WriteLine(CoinChange.MinimumCoins(new[] {1,4,3}, 11)); //3
Console.WriteLine(CoinChange.MinimumCoins(new[] {2,3}, 21)); //7
Console.WriteLine(CoinChange.MinimumCoins(new[] {2,3}, 0)); //0
Console.WriteLine(CoinChange.MinimumCoins(new[] {2,3}, 10)); //4

Console.WriteLine(MultiplesSum.ComputeMultiplesSum(9)); //21
Console.WriteLine(MultiplesSum.ComputeMultiplesSum(15)); //66
Console.WriteLine(FilterWords.SortWords(new[] {"dog", "cat", "cloud", "pen", "leak"},"ae"));