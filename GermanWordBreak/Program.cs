namespace GermanWordBreak
{
  internal class Program
  {
    private static Dictionary<string, List<string>> wordSplitMap { get; set; } = new Dictionary<string, List<string>>();
    private static Dictionary<string, string> dictionary { get; set; } = new Dictionary<string, string>();
    private static List<string> wordList { get; set; } = new List<string>();
    private static string outputPath { get; set; }

    private const string dictionaryPath = "../../../de-dictionary.tsv";
    private const string defaultWordListPath = "../../../de-test-words.tsv";
    private const string defaultOutputPath = "../../../output.tsv";
    static void Main()
    {
      initFields();

      processWordList();

      writeToFile();
    }

    private static void processWordList()
    {
      foreach (var word in wordList)
      {
        var wordToLower = word.ToLower();
        if (!wordSplitMap.ContainsKey(wordToLower))
          wordSplitMap.Add(wordToLower, splitWord(wordToLower));
      }
    }

    private static void writeToFile()
    {
      using (var sw = new StreamWriter(outputPath.Length == 0 ? defaultOutputPath : outputPath, false))
      {
        foreach (var word in wordSplitMap)
          sw.WriteLine($"(in) {word.Key} -> (out) {string.Join(", ", word.Value)}");
      }
      Console.WriteLine("Done! Press any key to exit.");
    }

    private static void initFields()
    {
      var dictionaryList = readLinesFromFile(dictionaryPath);
      if (dictionaryList == null) return;

      Console.WriteLine($"Press Enter for default path: {defaultWordListPath}");
      Console.Write("Enter full path to file with words: ");
      var wordListPath = Console.ReadLine() ;
      wordList = readLinesFromFile(wordListPath.Length == 0 ? defaultWordListPath : wordListPath);
      wordList = wordList?.OrderBy(w => w.Length).ToList(); 
      if (wordList == null) return;

      Console.WriteLine($"Press Enter for default path: {defaultOutputPath}");
      Console.Write("Enter full path to output file: ");
      outputPath = Console.ReadLine();

      dictionary = initDictionary(dictionaryList);
    }

    private static List<string> splitWord(string word)
    {
      var result = trySplit(word);
      if (result.Count == 0)
        result.Add(word);
      return result;
    }

    private static List<string> trySplit(string word)
    {
      if (dictionary.ContainsKey(word))
        return new List<string>() { word };

      var wordSplits = new List<List<string>>();
      var selectedWord = string.Empty;

      for (int i = 0; i < word.Length; i++)
      {
        selectedWord = word.Substring(0, i + 1);
        if (dictionary.ContainsKey(selectedWord))
          addWordToSplit(word, i, wordSplits);
      }

      if (wordSplits.Count == 0)
        return new List<string>();
      return wordSplits.OrderBy(s => s.Count).First();
    }

    private static void addWordToSplit(string word, int index, List<List<string>> wordSplits)
    {
      var selectedWord = word.Substring(0, index + 1);
      var split = new List<string>() { selectedWord };
      var leftOver = word.Substring(index + 1);

      split.AddRange(splitLeftOver(leftOver));

      if (string.Join("", split) == word)
        wordSplits.Add(split);
    }

    private static List<string> splitLeftOver(string leftOver)
    {
      var result = new List<string>();
      if (wordSplitMap.ContainsKey(leftOver))
        result = wordSplitMap[leftOver];
      else
      {
        var leftOverSplit = trySplit(leftOver);
        if (leftOverSplit.Count != 0)
          result = leftOverSplit;
      }
      return result;
    }

    private static Dictionary<string, string> initDictionary(List<string> dictionaryList)
    {
      var result = new Dictionary<string, string>();
      foreach (var entry in dictionaryList)
      {
        var entryToLower = entry.ToLower();
        result[entryToLower] = entryToLower;
      }
      return result;
    }

    private static List<string> readLinesFromFile(string path)
    {
      try
      {
        var content = File.ReadAllLines(path).ToList();
        return content;
      }
      catch (Exception)
      {
        Console.WriteLine($"Cannot find file at {path}");
        return null;
      }
    }
  }
}





