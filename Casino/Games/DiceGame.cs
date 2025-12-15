using System;
using System.Collections.Generic;
using DiceImplementation;  // ← теперь ссылаемся на DiceImplementation
using Games;

namespace Games
{
    public class DiceGame : CasinoGameBase
    {
        private readonly int _diceCount;
        private readonly int _min;
        private readonly int _max;

        private List<Dice> _playerDice;
        private List<Dice> _dealerDice;

        public DiceGame(int diceCount, int min, int max)
        {
            if (diceCount <= 0)
                throw new ArgumentException("Количество костей должно быть положительным");


            _diceCount = diceCount;
            _min = min;
            _max = max;
            FactoryMethod();
        }

        protected override void FactoryMethod()
        {
            _playerDice = new List<Dice>();
            _dealerDice = new List<Dice>();

            for (int i = 0; i < _diceCount; i++)
            {
                _playerDice.Add(new Dice(_min, _max));
                _dealerDice.Add(new Dice(_min, _max));
            }
        }

        private int CalculateTotal(List<Dice> diceList)
        {
            return diceList.Sum(d => d.Number);
        }

        public override void PlayGame()
        {
            int playerTotal = CalculateTotal(_playerDice);
            int dealerTotal = CalculateTotal(_dealerDice);

            Console.WriteLine($"Ваши кости: {string.Join(", ", _playerDice.Select(d => d.Number))} (Сумма: {playerTotal})");
            Console.WriteLine($"Кости дилера: {string.Join(", ", _dealerDice.Select(d => d.Number))} (Сумма: {dealerTotal})");

            if (playerTotal > dealerTotal)
            {
                Console.WriteLine("Вы победили!");
                OnWinInvoke();
            }
            else if (dealerTotal > playerTotal)
            {
                Console.WriteLine("Вы проиграли.");
                OnLooseInvoke();
            }
            else
            {
                Console.WriteLine("Ничья!");
                OnDrawInvoke();
            }
        }
    }
}
