using System;

namespace TYB_AMI//was initially TrainYourBrainApp
{
    enum ProblemType
    {
        Addition = 0,
        Subtraction = 1,
        Multiplication = 2
    }

    public class MathProblem
    {
        // Fields
        ProblemType type;
        Random rng;
        int num1;
        int num2;

        // Properties
        public string Problem { get; set; }
        public int Solution { get; set; }
        //public Vector2 Position { get; set; }
        public bool Correct { get; set; }
        public int? UserAnswer { get; set; }

        // Constructor
        public MathProblem(/*Vector2 position*/)
        {
            //Position = position;
            Correct = false;
            Problem = "";

            rng = new Random();
            type = (ProblemType)rng.Next(3);

            if (type == ProblemType.Addition)
            {
                GenerateAdditionProblem();
            }
            else if (type == ProblemType.Subtraction)
            {
                GenerateSubtractionProblem();
            }
            else if (type == ProblemType.Multiplication)
            {
                GenerateMultiplicationProblem();
            }
        }

        // Methods
        public void GenerateAdditionProblem()
        {
            num1 = rng.Next(10);
            num2 = rng.Next(10);

            Problem = $"{num1} + {num2} =";
            Solution = num1 + num2;
        }

        public void GenerateSubtractionProblem()
        {
            num1 = rng.Next(2, 19);
            num2 = 20;
            while (num2 > num1)
                num2 = rng.Next(9);

            Problem = $"{num1} - {num2} =";
            Solution = num1 - num2;
        }

        public void GenerateMultiplicationProblem()
        {
            num1 = rng.Next(10);
            num2 = rng.Next(10);

            Problem = $"{num1} x {num2} =";
            Solution = num1 * num2;
        }
        
        /*public void Draw(SpriteBatch sb, SpriteFont font)
        {
            sb.DrawString(font, Problem, Position, Color.Black);
        }*/
    }
}
