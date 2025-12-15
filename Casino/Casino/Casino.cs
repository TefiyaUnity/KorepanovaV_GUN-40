using System;
using Player;
using Services;
using Games;

namespace Casino
{
    public class Casino : IGame
    {
        private readonly IPlayer _player;
        private readonly FileSystemSaveLoadService _saveLoadService;
        private BlackjackGame _blackjackGame;
        private DiceGame _diceGame;

        public Casino(IPlayer player, string savePath)
        {
            _player = player;
            _saveLoadService = new FileSystemSaveLoadService(savePath);
            _blackjackGame = new BlackjackGame(52);
            _diceGame = new DiceGame(2, 1, 6);

            // Подписка на события игр
            _blackjackGame.OnWin += () => _player.Balance += _currentBet;
            _blackjackGame.OnLoose += () => _player.Balance -= _currentBet;
            _blackjackGame.OnDraw += () => { }; // Ничья — баланс не меняется

            _diceGame.OnWin += () => _player.Balance += _currentBet;
            _diceGame.OnLoose += () => _player.Balance -= _currentBet;
            _diceGame.OnDraw += () => { };
        }

        private int _currentBet;

        public void StartGame()
        {
            Console.WriteLine("Добро пожаловать в казино!");

            // Выбор игры
            Console.WriteLine("Выберите игру:");
            Console.WriteLine("1 — Блэкджек");
            Console.WriteLine("2 — Игра в кости");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();

            CasinoGameBase game = null;

            switch (choice)
            {
                case "1":
                    game = _blackjackGame;
                    break;
                case "2":
                    game = _diceGame;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    return;
            }

            // Проверка баланса
            if (_player.Balance <= 0)
            {
                Console.WriteLine("Нет денег? Выгнали!");
                return;
            }

            // Ставка
            Console.Write($"Введите ставку (не более {_player.Balance}): ");
            if (!int.TryParse(Console.ReadLine(), out _currentBet) || _currentBet <= 0 || _currentBet > _player.Balance)
            {
                Console.WriteLine("Некорректная ставка.");
                return;
            }

            // Запуск выбранной игры
            game.PlayGame();

            // Проверка и обработка баланса после игры
            if (_player.Balance > _player.MaxBalance)
            {
                int excess = _player.Balance - _player.MaxBalance;
                _player.Balance = _player.MaxBalance;
                Console.WriteLine($"Вы разорили казино! На его месте построят новое. Ваш баланс: {_player.Balance}");
            }
            else if (_player.Balance > _player.MaxBalance / 2)
            {
                _player.Balance /= 2;
                Console.WriteLine("Вы потратили половину своих банковских денег в баре казино");
            }

            // Сохранение профиля
            string playerData = $"{_player.Name}\n{_player.Balance}";
            _saveLoadService.SaveData(playerData, _player.Name);

            Console.WriteLine($"Спасибо за игру, {_player.Name}! Ваш баланс: {_player.Balance}");
        }
    }
}
