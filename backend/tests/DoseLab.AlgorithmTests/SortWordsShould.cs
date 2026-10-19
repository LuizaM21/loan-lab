using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class SortWordsShould
{
    [Fact]
    public void CheckWordsOrder() {
    
        var randomWords = new [] { "cat", "people", "cloud", "apple" };
        var results = FilterWords.SortWords(randomWords, "cp");
        var expected = new[] { "cat", "people", "cloud", "apple" };
        
        Assert.Equal(expected, results);
    }

    
}