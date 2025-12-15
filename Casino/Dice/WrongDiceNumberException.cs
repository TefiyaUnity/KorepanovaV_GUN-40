using System;

namespace DiceImplementation
{
    public class WrongDiceNumberException : Exception
    {
        public WrongDiceNumberException(int value, int min, int max)
            : base($"Недопустимое значение: {value}. Допустимый диапазон: [{min}, {max}]")
        {
        }
    }
}
