using System;
using System.Collections.Generic;
using static Unit;
using static Unit.Goblin;


public class Program
{
    private static Player player;
    private static DungeonRoom currentRoom;
    private static bool isRunning = true;

    public static void Main(string[] args)
    {
        Console.WriteLine("Добро пожаловать в подземелье!");
        Console.Write("Введите имя героя: ");
        string playerName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Без имени";

        player = new Player(playerName);

        Console.WriteLine($"\n{player.Name}, вы готовы к испытанию?");
        Console.WriteLine("Выберите уровень сложности: 1 — Лёгкий, 2 — Сложный");

        string input = Console.ReadLine();
        DungeonFactory factory = input == "2"
            ? new HardDungeonFactory()
            : new EasyDungeonFactory();

        currentRoom = factory.CreateDungeon(input == "2" ? Difficulty.Hard : Difficulty.Easy);
        Console.WriteLine($"\nВы вошли в подземелье. Первая комната: {currentRoom.Title}\n");

        // Основной игровой цикл
        while (isRunning)
        {
            RenderRoom();
            Console.Write("> ");
            string command = Console.ReadLine()?.Trim().ToLower();

            HandleCommand(command);
        }

        Console.WriteLine("Игра завершена. До свидания!");
    }

    private static void RenderRoom()
    {
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Комната: {currentRoom.Title}");

        if (currentRoom.Loot != null)
            Console.WriteLine($"На полу лежит: {currentRoom.Loot.Name} (x{currentRoom.Loot.Quantity})");

        if (currentRoom.Enemy != null)
            Console.WriteLine($"В комнате враг: {currentRoom.Enemy.Name} (Здоровье: {currentRoom.Enemy.CurrentHealth})");

        Console.WriteLine($"Ваше здоровье: {player.CurrentHealth}/{player.MaxHealth}");
        Console.WriteLine("Команды: go [направление], fight [драться], take [поднять], inventory [инвентарь], equip [экипировать], quit [покинуть]");
        Console.WriteLine(new string('-', 40));
    }

    private static void HandleCommand(string command)
    {
        if (string.IsNullOrEmpty(command))
            return;

        var parts = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        string action = parts[0];

        switch (action)
        {
            case "quit":
                isRunning = false;
                break;

            case "go":
                if (parts.Length < 2)
                {
                    Console.WriteLine("Укажите направление (число): go -1, go 0, go 1");
                    return;
                }
                int dir;
                if (!int.TryParse(parts[1], out dir) || !currentRoom.Exits.ContainsKey(dir))
                {
                    Console.WriteLine("Неверное направление или выход отсутствует.");
                    return;
                }
                currentRoom = currentRoom.Exits[dir];
                Console.WriteLine($"Вы перешли в: {currentRoom.Title}");
                if (currentRoom.IsFinal)
                {
                    Console.WriteLine("ПОБЕДА! Вы достигли конечной комнаты.");
                    isRunning = false;
                }
                break;

            case "fight":
                if (currentRoom.Enemy == null)
                {
                    Console.WriteLine("Здесь нет врага для боя.");
                    return;
                }
                Fight(currentRoom.Enemy);
                break;

            case "inventory":
                ShowInventory();
                break;

            case "take":
                TakeLoot();
                break;

            case "equip":
                if (parts.Length < 2)
                {
                    Console.WriteLine("Укажите предмет из инвентаря (по номеру).");
                    return;
                }
                EquipItem(parts[1]);
                break;

            default:
                Console.WriteLine("Неизвестная команда. Попробуйте: go, fight, inventory, equip, quit");
                break;
        }
    }
    private static void TakeLoot()
    {
        if (currentRoom.Loot == null)
        {
            Console.WriteLine("В комнате нет ничего, что можно взять.");
            return;
        }

        // Проверяем, поместится ли предмет в инвентарь
        if (!player.AddToInventory(currentRoom.Loot))
        {
            Console.WriteLine("Инвентарь полон! Нельзя подобрать предмет.");
            return;
        }

        Console.WriteLine($"Вы подобрали: {currentRoom.Loot.Name} (x{currentRoom.Loot.Quantity})");
        currentRoom.Loot = null;  // Убираем лут из комнаты
    }


    private static void Fight(Unit enemy)
    {
        Console.WriteLine($"Начинается бой с {enemy.Name}!");

        while (player.CurrentHealth > 0 && enemy.CurrentHealth > 0)
        {
            // Игрок атакует
            int playerDamage = player.GetTotalDamage();
            enemy.TakeDamage(playerDamage);
            Console.WriteLine($"Вы нанесли {playerDamage} урона. Здоровье врага: {enemy.CurrentHealth}");

            if (enemy.CurrentHealth <= 0)
            {
                Console.WriteLine($"Вы победили {enemy.Name}!");
                currentRoom.Enemy = null;
                return;
            }

            // Враг атакует
            int enemyDamage = enemy.BaseDamage;
            player.TakeDamage(enemyDamage);
            Console.WriteLine($"{enemy.Name} нанес вам {enemyDamage} урона. Ваше здоровье: {player.CurrentHealth}");

            if (player.CurrentHealth <= 0)
            {
                Console.WriteLine("Вы погибли. Игра окончена.");
                isRunning = false;
                return;
            }
        }
    }

    private static void ShowInventory()
    {
        Console.WriteLine("\nВаш инвентарь:");
        for (int i = 0; i < player.Inventory.Count; i++)
        {
            var item = player.Inventory[i];
            Console.WriteLine($"{i + 1}. {item.Name} (x{item.Quantity})");
        }
        Console.WriteLine();
    }

    private static void EquipItem(string itemIndex)
    {
        int index;
        if (!int.TryParse(itemIndex, out index) || index < 1 || index > player.Inventory.Count)
        {
            Console.WriteLine("Неверный номер предмета.");
            return;
        }

        var item = player.Inventory[index - 1];
        SlotType slot;

        switch (item.Type)
        {
            case ItemType.Weapon:
                slot = SlotType.MainHand;
                break;
            case ItemType.Armour:
                slot = SlotType.ArmourSlot;
                break;
            default:
                Console.WriteLine("Этот предмет нельзя экипировать.");
                return;
        }

        player.EquipItem(item, slot);
    }
}


// Основные перечисления и константы
public enum ItemType { Weapon, Armour, Gold, HealthPotion, SharpeningStone }
public enum SlotType { MainHand, OffHand, ArmourSlot, RangeWeapon, Helmet }
public enum Difficulty { Easy, Hard }

public static class GameConstants
{
    public const int MaxInventorySize = 3;
    public const int PlayerMaxHealth = 30;
    public const int GoblinMaxHealth = 18;
    public const int PlayerBaseDamage = 6;
    public const int GoblinBaseDamage = 2;
    public const int DefaultWeaponDamage = 10;
    public const int DefaultWeaponMaxDurability = 15;
    public const int DefaultArmourProtection = 5;
    public const int MaxArmourProtection = 50;
    public const int HealthPotionHeal = 7;
    public const int SharpeningStoneRepair = 4;
}

// Класс предмета
public class  Item
{
    public ItemType Type { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsStackable { get; set; }

    // Для экипируемых
    public int Damage { get; set; }
    public int CurrentDurability { get; set; }
    public int MaxDurability { get; set; }
    public int Protection { get; set; } // только для брони


    public Item(ItemType type, string name)
    {
        Type = type;
        Name = name;
        IsStackable = type == ItemType.Gold;
    }
}
// Класс юнита (игрок и NPC)
public abstract class Unit
{
    public string Name { get; set; }
    protected Unit (string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя не может быть пустым", nameof(name));
        Name = name;
    }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int BaseDamage { get; set; }

    protected Dictionary<SlotType, Item> Equipment { get; } = new();
    public List<Item> Inventory { get; } = new();

    public virtual void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) CurrentHealth = 0;
    }

    public void Heal(int amount)
    {
        CurrentHealth = Math.Min(CurrentHealth + amount, MaxHealth);
    }

    public int GetTotalDamage()
    {
        int total = BaseDamage;
        if (Equipment.ContainsKey(SlotType.MainHand) && Equipment[SlotType.MainHand] != null)
            total += Equipment[SlotType.MainHand].Damage;
        return total;
    }

    public int GetTotalProtection()
    {
        int total = 0;
        if (Equipment.ContainsKey(SlotType.ArmourSlot) && Equipment[SlotType.ArmourSlot] != null)
            total += Equipment[SlotType.ArmourSlot].Protection;
        return Math.Min(total, GameConstants.MaxArmourProtection);
    }

    public bool AddToInventory(Item item)
    {
        if (Inventory.Count >= GameConstants.MaxInventorySize) return false;

        if (item.IsStackable)
        {
            var existing = Inventory.Find(i => i.Type == item.Type);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                return true;
            }
        }
        Inventory.Add(item);
        return true;
    }
    public void RemoveFromInventory(Item item)
    {
        Inventory.Remove(item);
    }
    // Класс игрока (наследник Unit)
    public class Player : Unit
    {
        public Player(string name) : base(name)  // Передаём имя в Unit
        {
            MaxHealth = GameConstants.PlayerMaxHealth;
            CurrentHealth = MaxHealth;
            BaseDamage = GameConstants.PlayerBaseDamage;
        }

        public void EquipItem(Item item, SlotType slot)
        {
            if (item == null) return;

            if (Equipment.ContainsKey(slot))
            {
                var oldItem = Equipment[slot];
                if (oldItem != null)
                    Console.WriteLine($"Заменено: {oldItem.Name}");
            }

            Equipment[slot] = item;
            Console.WriteLine($"Экипировано: {item.Name} в слот {slot}");
        }

        public void RepairWeapon(SlotType slot)
        {
            if (!Equipment.ContainsKey(slot) || Equipment[slot] == null) return;
            var weapon = Equipment[slot];
            if (weapon.CurrentDurability < weapon.MaxDurability)
            {
                weapon.CurrentDurability = Math.Min(weapon.CurrentDurability + GameConstants.SharpeningStoneRepair, weapon.MaxDurability);
                Console.WriteLine($"{weapon.Name} отремонтирован. Текущая прочность: {weapon.CurrentDurability}");
            }
        }
    }

    // Класс Goblin (наследник Unit)
    public class Goblin : Unit
    {
        public Goblin() : base("Goblin")  // Фиксированное имя
        {
            MaxHealth = GameConstants.GoblinMaxHealth;
            CurrentHealth = MaxHealth;
            BaseDamage = GameConstants.GoblinBaseDamage;
        }
    }
}

        // Класс комнаты подземелья
        public class DungeonRoom
        {
            public string Title { get; set; }
            public Dictionary<int, DungeonRoom> Exits { get; } = new(); // -1, 0, 1
            public Item? Loot { get; set; }
            public Unit? Enemy { get; set; }
            public bool IsFinal { get; set; }
        public DungeonRoom(string title, bool isFinal = false)
        {
            Title = title;
            IsFinal = isFinal;
        }
    }

// Фабрика подземелий (абстрактный класс)
public abstract class DungeonFactory
{
    public abstract DungeonRoom CreateDungeon(Difficulty diff);
}

// Реализация фабрики для лёгкого уровня
public class EasyDungeonFactory : DungeonFactory
{
    public override DungeonRoom CreateDungeon(Difficulty diff)
    {
        var start = new DungeonRoom("Вход в подземелье");
        var mid1 = new DungeonRoom("Перекрёсток");
        var mid2 = new DungeonRoom("Тёмный коридор");
        var final = new DungeonRoom("Сокровищница", true);

        start.Exits[0] = mid1;
        mid1.Exits[-1] = mid2;
        mid2.Exits[1] = final;

        // Добавляем лут и врагов
        mid1.Loot = new Item(ItemType.Gold, "Золото") { Quantity = 10 };
        mid2.Enemy = new Goblin();

        return start;
    }
}

// Реализация фабрики для сложного уровня
public class HardDungeonFactory : DungeonFactory
{
    public override DungeonRoom CreateDungeon(Difficulty diff)
    {
        var start = new DungeonRoom("Мрачная пещера");
        var fork1 = new DungeonRoom("Развилка 1");
        var fork2 = new DungeonRoom("Развилка 2");
        var trap = new DungeonRoom("Ловушка");
        var bossRoom = new DungeonRoom("Зал босса");
        var final = new DungeonRoom("Древнее святилище", true);

        start.Exits[0] = fork1;
        fork1.Exits[-1] = trap;
        fork1.Exits[1] = fork2;
        fork2.Exits[0] = bossRoom;
        bossRoom.Exits[0] = final;

        trap.Loot = new Item(ItemType.HealthPotion, "Зелье здоровья");
        bossRoom.Enemy = new Goblin() { Name = "Вождь гоблинов", MaxHealth = 30, CurrentHealth = 30 };

        final.Loot = new Item(ItemType.Weapon, "Легендарный меч")
        {
            Damage = 15,
            CurrentDurability = 20,
            MaxDurability = 20
        };

        return start;
    }
}

