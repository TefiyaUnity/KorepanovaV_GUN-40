using System;
using Player;
using Services;
using Casino;  // Пространство имён


namespace CasinoGame
{
    class Program
    {
        static void Main(string[] args)
        {
            const string savePath = "players";

            Console.Write("Введите имя игрока: ");
            string playerName = Console.ReadLine();

            var saveLoadService = new FileSystemSaveLoadService(savePath);
            string savedData = saveLoadService.LoadData(playerName);


            IPlayer player;

            if (string.IsNullOrEmpty(savedData))
            {
                player = new Player.Player(playerName);
                Console.WriteLine($"Создан новый профиль. Начальный баланс: {player.Balance}");
            }
            else
            {
                string[] lines = savedData.Split('\n');
                player = new Player.Player(lines[0], int.Parse(lines[1]));
                Console.WriteLine($"Загружен профиль. Баланс: {player.Balance}");
            }

            // Используем полное имя: Casino.Casino
            var casino = new Casino.Casino(player, savePath);
            casino.StartGame();

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
