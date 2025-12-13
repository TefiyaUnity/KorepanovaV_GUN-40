using GamePrototype.Items.EquipItems;

namespace GamePrototype.Items.EconomicItems
{
    public sealed class Grindstone : EconomicItem
    {
        public override bool Stackable => false;

        public Grindstone(string name) : base(name) {}
        // Метод для заточки оружия
        public bool ApplyToWeapon(Weapon weapon)
        {
            if (weapon.CurrentDurability < weapon.Durability)
            {
                weapon.Repair(1); // Восстанавливаем 1 единицу прочности
                return true;
            }
            return false;
        }
    }
}
