using GamePrototype.Items.EconomicItems;
using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems
{
    public abstract class EquipItem : Item
    {
        private uint _durability;
        private uint _maxDurability;
        public uint Durability { get => _durability; protected set => _durability = value; }
        public override bool Stackable => false;

        public abstract EquipSlot Slot { get; }
        public uint CurrentDurability { get; private set; }

        protected EquipItem(uint maxDurability, string name) : base(name) => _maxDurability = maxDurability;

        public void ReduceDurability(uint delta) => _durability -= delta;

        public void Repair(uint delta)
        {
            CurrentDurability = Math.Min(CurrentDurability + delta, _maxDurability);
        }
    }
    public sealed class RangeWeapon : EquipItem
    {
        public RangeWeapon(uint damage, uint durability, string name)
            : base(durability, name) => Damage = damage;

        public uint Damage { get; }

        public override EquipSlot Slot => EquipSlot.RangeWeapon;
    }
    public sealed class Helmet : EquipItem
    {
        public Helmet(uint defence, uint durability, string name)
            : base(durability, name) => Defence = defence;

        public uint Defence { get; }  // % снижения урона
        public uint CurrentDurability { get; private set; }
        public override EquipSlot Slot => EquipSlot.Helmet;

        // Метод для обработки урона (аналогично Armour)
        public void TakeDamage()
        {
            if (CurrentDurability > 0)
                CurrentDurability--;
        }
    }
}
