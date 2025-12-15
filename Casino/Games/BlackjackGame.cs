using System;
using System.Collections.Generic;
using Cards;
using Games;

namespace Games
{
    public class BlackjackGame : CasinoGameBase
    {
        private Queue<Card> _deck;
        private List<Card> _playerHand;
        private List<Card> _dealerHand;

        public BlackjackGame(int cardsCount)
        {
            if (cardsCount < 2)
                throw new ArgumentException("Количество карт должно быть не менее 2");

            FactoryMethod();
            _playerHand = new List<Card>();
            _dealerHand = new List<Card>();
        }

        protected override void FactoryMethod()
        {
            var cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards.Add(new Card(suit, rank));
                }
            }

            _deck = new Queue<Card>(Shuffle(cards));
        }

        private List<Card> Shuffle(List<Card> cards)
        {
            var random = new Random();
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
            return cards;
        }

        private int CalculateHandValue(List<Card> hand)
        {
            int value = 0;
            int aces = 0;

            foreach (var card in hand)
            {
                value += (int)card.Rank;
                if (card.Rank == Rank.Ace) aces++;
            }

            while (value > 21 && aces > 0)
            {
                value -= 10;
                aces--;
            }

            return value;
        }

        public override void PlayGame()
        {
            // Раздача начальных карт
            _playerHand.Add(_deck.Dequeue());
            _playerHand.Add(_deck.Dequeue());

            _dealerHand.Add(_deck.Dequeue());
            _dealerHand.Add(_deck.Dequeue());

            int playerValue = CalculateHandValue(_playerHand);
            int dealerValue = CalculateHandValue(_dealerHand);

            Console.WriteLine($"Ваши карты: {string.Join(", ", _playerHand)} (Сумма: {playerValue})");
            Console.WriteLine($"Карты дилера: {_dealerHand[0]}, ???");

            // Логика игры
            while (true)
            {
                if (playerValue == 21 && dealerValue != 21)
                {
                    Console.WriteLine("Блэкджек! Вы победили!");
                    OnWinInvoke();
                    return;
                }
                else if (dealerValue == 21 && playerValue != 21)
                {
                    Console.WriteLine("Дилер собрал блэкджек! Вы проиграли.");
                    OnLooseInvoke();
                    return;
                }

                Console.Write("Хотите взять ещё карту? (да/нет): ");
                string input = Console.ReadLine().ToLower();

                if (input == "да")
                {
                    var newCard = _deck.Dequeue();
                    _playerHand.Add(newCard);
                    playerValue = CalculateHandValue(_playerHand);
                    Console.WriteLine($"Вы взяли: {newCard} (Сумма: {playerValue})");

                    if (playerValue > 21)
                    {
                        Console.WriteLine("Перебор! Вы проиграли.");
                        OnLooseInvoke();
                        return;
                    }
                }
                else if (input == "нет")
                {
                    // Ход дилера
                    while (dealerValue < 17)
                    {
                        var newCard = _deck.Dequeue();
                        _dealerHand.Add(new Card());
                        dealerValue = CalculateHandValue(_dealerHand);
                        Console.WriteLine($"Дилер взял карту: {newCard}");
                    }

                    Console.WriteLine($"Итоговые карты дилера: {string.Join(", ", _dealerHand)} (Сумма: {dealerValue})");

                    if (dealerValue > 21)
                    {
                        Console.WriteLine("Дилер перебрал! Вы победили!");
                        OnWinInvoke();
                    }
                    else if (playerValue > dealerValue)
                    {
                        Console.WriteLine("Вы победили!");
                        OnWinInvoke();
                    }
                    else if (dealerValue > playerValue)
                    {
                        Console.WriteLine("Вы проиграли.");
                        OnLooseInvoke();
                    }
                    else
                    {
                        Console.WriteLine("Ничья!");
                        OnDrawInvoke();
                    }
                    return;
                }
                else
                {
                    Console.WriteLine("Введите 'да' или 'нет'.");
                }
            }
        }
    }
}
