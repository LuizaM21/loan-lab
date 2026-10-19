using System.Globalization;

namespace DoseLab.Algorithms;

public static class FilterWords
{
    public static string[] SortWords(string[] words, string letters)
    {
        var results = new List<string>();

        foreach (var word in words)
        {
            foreach (char letter in letters)
            {
                if (word.Contains(letter))
                {
                    results.Add(word);
                    break;
                }
            }
        }
        Console.WriteLine(string.Join(" ", results));
        return results.ToArray();
    }
    
}