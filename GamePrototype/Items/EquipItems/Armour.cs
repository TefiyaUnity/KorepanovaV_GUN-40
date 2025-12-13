using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems
{
    public sealed class Armour : EquipItem
    {
        public Armour(uint defence, uint durability, string name) : base(durability, name) => Defence = defence;

        public uint Defence { get; }
        public uint CurrentDurability { get; private set; }
        public override EquipSlot Slot => EquipSlot.Armour;
        
        // Метод, вызываемый при получении урона броней
        public void TakeDamage()
        {
            if (CurrentDurability > 0)
            {
                CurrentDurability--;
            }
        }
    }
}
