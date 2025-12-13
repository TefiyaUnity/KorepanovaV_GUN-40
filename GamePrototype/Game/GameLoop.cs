using GamePrototype.Combat;
using GamePrototype.Dungeon;
using GamePrototype.Units;
using GamePrototype.Utils;

namespace GamePrototype.Game
{
    public sealed class GameLoop
    {
        private Player _player;
        private DungeonRoom _dungeon;
        private readonly CombatManager _combatManager = new CombatManager();
        
        public void StartGame() 
        {
            Initialize();
            Console.WriteLine("Entering the dungeon");
            StartGameLoop();
            var currentRoom = _dungeon;

            while (currentRoom.IsFinal == false)
            {
                StartRoomEncounter(currentRoom, out var success);
                if (!success)
                {
                    Console.WriteLine("Game over!");
                    return;
                }

                DisplayRouteOptions(currentRoom);

                while (true)
                {
                    var input = Console.ReadLine();

                    // Команды управления экипировкой
                    if (input.Equals("eq", StringComparison.OrdinalIgnoreCase))
                    {
                        _player.DisplayEquipment();
                        continue;
                    }
                    else if (input.Equals("unequip", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("Slot to unequip (Weapon/Armour/RangeWeapon/Helmet): ");
                        if (Enum.TryParse<EquipSlot>(Console.ReadLine(), out var slot))
                        {
                            _player.UnequipItem(slot);
                        }
                        continue;
                    }

                    // Навигация
                    if (Enum.TryParse<Direction>(input, out var direction))
                    {
                        currentRoom = currentRoom.Rooms[direction];
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Wrong direction or command! Use: Left/Forward/Right or eq/unequip");
                    }
                }
            }
            Console.WriteLine($"Congratulations, {_player.Name}");
            Console.WriteLine("Result: ");
            Console.WriteLine(_player.ToString());
        }
        

        #region Game Loop

        private void Initialize()
        {
            Console.WriteLine("Welcome, player!");
            _dungeon = DungeonBuilder.BuildDungeon();
            Console.WriteLine("Enter your name");
            string name = Console.ReadLine();
            _player = new Player(name, 30, 30, 6);
            Console.WriteLine($"Hello {_player.Name}");
        }

        private void StartGameLoop()
        {
            var currentRoom = _dungeon;
            
            while (currentRoom.IsFinal == false) 
            {
                StartRoomEncounter(currentRoom, out var success);
                if (!success) 
                {
                    Console.WriteLine("Game over!");
                    return;
                }
                DisplayRouteOptions(currentRoom);
                while (true) 
                {
                    if (Enum.TryParse<Direction>(Console.ReadLine(), out var direction) ) 
                    {
                        currentRoom = currentRoom.Rooms[direction];
                        break;
                    }
                    else 
                    {
                        Console.WriteLine("Wrong direction!");
                    }
                }
            }
            Console.WriteLine($"Congratulations, {_player.Name}");
            Console.WriteLine("Result: ");
            Console.WriteLine(_player.ToString());
        }

        private void StartRoomEncounter(DungeonRoom currentRoom, out bool success)
        {
            success = true;
            if (currentRoom.Loot != null) 
            {
                _player.AddItemToInventory(currentRoom.Loot);
            }
            if (currentRoom.Enemy != null) 
            {
                if (_combatManager.StartCombat(_player, currentRoom.Enemy) == _player)
                {
                    _player.HandleCombatComplete();
                    LootEnemy(currentRoom.Enemy);
                }
                else 
                {
                    success = false;
                }
            }

            void LootEnemy(Unit enemy)
            {
                _player.AddItemsFromUnitToInventory(enemy);
            }
        }

        private void DisplayRouteOptions(DungeonRoom currentRoom)
        {
            Console.WriteLine("Where to go?");
            foreach (var room in currentRoom.Rooms)
            {
                Console.Write($"{room.Key} - {(int) room.Key}\t");
            }
        }

        
        #endregion
    }
}
