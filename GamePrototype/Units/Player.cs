using GamePrototype.Items.EconomicItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Utils;
using System.Text;

namespace GamePrototype.Units
{
    public sealed class Player : Unit
    {
        private readonly Dictionary<EquipSlot, EquipItem> _equipment = new();

        public Player(string name, uint health, uint maxHealth, uint baseDamage) : base(name, health, maxHealth, baseDamage)
        {            
        }

        public override void AddItemToInventory(Item item)
        {
            if (item is EquipItem equipItem)
            {
                if (_equipment.TryGetValue(equipItem.Slot, out var existingItem))
                {
                    // Если слот уже занят — заменяем
                    Console.WriteLine($"{Name} replaced {existingItem.Name} with {equipItem.Name}!");
                    Inventory.TryAdd(existingItem); // Возвращаем старую вещь в инвентарь
                }
                _equipment[equipItem.Slot] = equipItem; // Экипируем новую вещь
                return;
            }
            base.AddItemToInventory(item);
        }
        public void DisplayEquipment()
        {
            Console.WriteLine("\n--- EQUIPMENT ---");
            foreach (var slot in _equipment)
            {
                Console.WriteLine($"{slot.Key}: {slot.Value.Name} ({slot.Value.CurrentDurability}/{slot.Value.Durability})");
            }
            Console.WriteLine("--- END ---\n");
        }

        public bool UnequipItem(EquipSlot slot)
        {
            if (_equipment.ContainsKey(slot))
            {
                var item = _equipment[slot];
                _equipment.Remove(slot);
                Inventory.TryAdd(item);
                Console.WriteLine($"{Name} removed {item.Name} from {slot} slot.");
                return true;
            }
            return false;
        }
        public override uint GetUnitDamage()
        {
            if (_equipment.TryGetValue(EquipSlot.Weapon, out var item) && item is Weapon weapon) 
            {
                return BaseDamage + weapon.Damage;
            }
            return BaseDamage;
        }

        public override void HandleCombatComplete()
        {
            var items = Inventory.Items;
            for (int i = 0; i < items.Count; i++) 
            {
                if (items[i] is EconomicItem economicItem) 
                {
                    if (economicItem is HealthPotion healthPotion)
                    {
                        Health += healthPotion.HealthRestore;
                        Inventory.TryRemove(items[i]);
                    }
                    else if (economicItem is Grindstone grindstone)
                    {
                        // Пытаемся заточить оружие
                        if (_equipment.TryGetValue(EquipSlot.Weapon, out var equipItem) && equipItem is Weapon weapon)
                        {
                            if (grindstone.ApplyToWeapon(weapon))
                            {
                                Console.WriteLine($"Weapon '{weapon.Name}' repaired. Current durability: {weapon.CurrentDurability}/{weapon.Durability}");
                                Inventory.TryRemove(items[i]); // Удаляем точильный камень из инвентаря
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void UseEconomicItem(EconomicItem economicItem)
        {
            if (economicItem is HealthPotion healthPotion) 
            {
                Health += healthPotion.HealthRestore;
            }
        }

        protected override uint CalculateAppliedDamage(uint damage)
        {
            if (_equipment.TryGetValue(EquipSlot.Armour, out var item) && item is Armour armour) 
            {
                damage -= (uint)(damage * (armour.Defence / 100f));
                armour.TakeDamage(); // Броня теряет 1 прочность при каждом получении урона
            }
            if (_equipment.TryGetValue(EquipSlot.Helmet, out var helmetItem) && helmetItem is Helmet helmet)
    {
        damage -= (uint)(damage * (helmet.Defence / 100f));
        helmet.TakeDamage();
    }

    return damage;
}

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Name);
            builder.AppendLine($"Health {Health}/{MaxHealth}");
            builder.AppendLine("Loot:");
            var items = Inventory.Items;
            for (int i = 0; i < items.Count; i++) 
            {
                builder.AppendLine($"[{items[i].Name}] : {items[i].Amount}");
            }
            return builder.ToString();
        }
    }
}


