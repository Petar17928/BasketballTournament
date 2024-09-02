namespace BasketballTournament.Models
{
    public class Match
    {
        public string Date {  get; set; }
        public string Opponent { get; set; }
        public string Result { get; set; }
        public Match(string opponent, string date, string result)
        {
            Opponent = opponent;
            Date = date;
            Result = result;
        }
    }
}
