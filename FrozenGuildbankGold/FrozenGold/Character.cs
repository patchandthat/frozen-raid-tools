namespace FrozenGold
{
    public class Character
    {
        public Character(string name)
        {
            Name = name;
        }

        public string Name { get; }
        
        public CharacterClass Class { get; set; }
    }
}