namespace BasketballTournament.Models
{
    public class Group
    {
        public string Name { get; set; }
        public List<TeamEntity> Teams { get; set; } = new List<TeamEntity>();

        public Group(string name)
        {
            Name = name;
        }
    }
}
