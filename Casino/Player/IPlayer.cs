namespace Player
{
    public interface IPlayer
    {
        string Name { get; set; }
        int Balance { get; set; }
        int MaxBalance { get; }
    }
}