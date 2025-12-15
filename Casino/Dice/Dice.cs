namespace DiceImplementation
{
    public struct Dice
    {
        private readonly int _min;
        private readonly int _max;
        public readonly int Number;

        public Dice(int min, int max)
        {
            if (min < 1 || min > int.MaxValue)
                throw new WrongDiceNumberException(min, 1, int.MaxValue);
            if (max < 1 || max > int.MaxValue)
                throw new WrongDiceNumberException(max, 1, int.MaxValue);
            if (min > max)
                throw new ArgumentException("min не может быть больше max");

            _min = min;
            _max = max;
            var random = new Random();
            Number = random.Next(_min, _max + 1);
        }
    }
}
