namespace TYB_AMI{
    public enum StroopColor{
        Blue,
        Green,
        Red,
        Yellow
    }

    public class StroopWord{
        private static readonly Random rng = new();
        private static readonly List<string> Words = ["Red", "Blue", "Green", "Yellow"];

        public string Text { get; }
        public StroopColor Color { get; }

        public StroopWord()
        {
            Text = Words[rng.Next(Words.Count)];
            Color = (StroopColor)rng.Next(4);
        }
    }
}