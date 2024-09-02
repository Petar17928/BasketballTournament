namespace BasketballTournament.Models
{
    public class TeamEntity
    {
        public string Team { get; set; }
        public string ISOCode { get; set; }
        public int FIBARanking { get; set; }
        public List<Match> Matches { get; set; } = new List<Match>();
        public Match LatestMatch => Matches.LastOrDefault();

        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }
        public int ScoredPoints { get; set; }
        public int ReceivedPoints { get; set; }
        public int PointDifference => ScoredPoints - ReceivedPoints;

        public int Rank { get; set; }
        public string Group {  get; set; }
    }
}
