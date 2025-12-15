using Player;

namespace Player
{
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public int Balance { get; set; }
        public int MaxBalance => 10000;

        public Player(string name, int balance = 1000)
        {
            Name = name;
            Balance = balance;
        }
    }
}