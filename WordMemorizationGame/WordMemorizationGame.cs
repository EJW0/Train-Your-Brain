namespace TYB_AMI{
    public class WordMemorization{
        public List<string> WordBank { get; private set; } = new();
        public List<string> SelectedWords { get; private set; } = new();
        public List<string> UserWords { get; private set; } = new();
        public List<string> ScoredWords { get; private set; } = new();
        public int Score { get; private set; }

        public void LoadWords(IEnumerable<string> words){
            WordBank = words
                .Select(x => x.Trim().ToLower())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        public void SelectRandomWords(int count = 30){
            SelectedWords.Clear();
            Random rng = new();
            var tempBank = WordBank.ToList();

            for (int i = 0; i < count && tempBank.Count > 0; i++){
                int index = rng.Next(tempBank.Count);
                SelectedWords.Add(tempBank[index]);
                tempBank.RemoveAt(index);
            }
        }

        public void AddAnswer(string word){
            word = word.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(word)){
                UserWords.Insert(0, word);
            }
        }

        public void CalculateScore(){
            Score = 0;
            ScoredWords.Clear();
            foreach (var word in UserWords){
                if (SelectedWords.Contains(word) && !ScoredWords.Contains(word)){
                    ScoredWords.Add(word);
                    Score++;
                }
            }
        }
    }
}