namespace TYB_AMI{
    public class StroopGame{
        public const int TotalQuestions = 50;
        public int CurrentQuestion { get; private set; }
        public int Score { get; private set; }
        public StroopWord CurrentWord { get; private set; } = new();
        public bool Finished => CurrentQuestion >= TotalQuestions;

        public void SubmitAnswer(StroopColor answer){
            if (Finished)
                return;
            
            if (answer == CurrentWord.Color)
                Score++;
            
            CurrentQuestion++;

            if (!Finished)
                CurrentWord = new StroopWord();
        }

        public void Reset(){
            Score = 0;
            CurrentQuestion = 0;
            CurrentWord = new StroopWord();
        }
    }
}