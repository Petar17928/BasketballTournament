using BasketballTournament.LanguageResources;
using BasketballTournament.Models;

namespace BasketballTournament.Services
{
    public class SimulationService
    {
        public SimulationService() { }
        public (int, int) ParseResult(string input)
        {
            string[] parts = input.Split('-');
            if (parts.Length != 2)
            {
                throw new ArgumentException(ConstantsEng.ResultString);
            }
            return (int.Parse(parts[0]), int.Parse(parts[1]));
        }
        public TeamEntity SimulateMatch(TeamEntity team1, TeamEntity team2)
        {
            var random = new Random();
            int team1Score = 0;
            int team1Allowed = 0;
            foreach (var match in team1.Matches)
            {
                team1Score += ParseResult(match.Result).Item1;
                team1Allowed += ParseResult(match.Result).Item2;
            }
            team1Score /= team1.Matches.Count;
            team1Allowed /= team1.Matches.Count;
            int team2Score = 0;
            int team2Allowed = 0;

            foreach (var match in team2.Matches)
            {
                team2Score += ParseResult(match.Result).Item1;
                team2Allowed += ParseResult(match.Result).Item2;

            }
            team2Score /= team2.Matches.Count;
            team2Allowed /= team1.Matches.Count;

            team1Score = (team1Score + team2Allowed) / 2;
            team2Score = (team2Score + team1Allowed) / 2;
            team1Score = random.Next(team1Score - 10, team1Score + 10);
            do
            {
                team2Score = random.Next(team2Score - 10, team2Score + 10);

            } while (team2Score == team1Score);

            var match1 = new Match(team2.ISOCode, DateTime.Now.ToString(), team1Score.ToString() + "-" + team2Score.ToString());
            var match2 = new Match(team1.ISOCode, DateTime.Now.ToString(), team2Score.ToString() + "-" + team1Score.ToString());

            team1.Matches.Add(match1);
            team2.Matches.Add(match2);
            team1.ScoredPoints += team1Score;
            team1.ReceivedPoints += team2Score;
            team2.ScoredPoints += team2Score;
            team2.ReceivedPoints += team1Score;

            if (team1Score > team2Score)
            {
                team1.Wins++;
                team2.Losses++;
                team1.Points += 2;
                if(random.Next(0,101) == 1)
                    team2.Points += 0;
                else
                    team2.Points += 1;
                return team1;
            }
            else
            {
                team2.Wins++;
                team1.Losses++;
                team2.Points += 2;
                if (random.Next(0, 101) == 1)
                    team1.Points += 0;
                else
                    team1.Points += 1;
                return team2;
            }
        }
        public Dictionary<string, List<TeamEntity>> SimulateGroupPhase(List<Group> groups)
        {
            Dictionary<string, List<TeamEntity>> teams = new Dictionary<string, List<TeamEntity>>();

            foreach (var group in groups)
            {
                Console.WriteLine($"{ConstantsEng.Group} - {group.Name}:");

                int numberOfTeams = group.Teams.Count;
                int numberOfRounds = numberOfTeams - 1;

                teams[group.Name] = new List<TeamEntity>();

                for (int round = 1; round <= numberOfRounds; round++)
                {
                    Console.WriteLine($"\t{ConstantsEng.Round} {round}:");
                    for (int i = 0; i < numberOfTeams / 2; i++)
                    {
                        var team1 = group.Teams[i];
                        var team2 = group.Teams[numberOfTeams - 1 - i];
                        SimulateMatch(team1, team2);

                        Console.WriteLine($"\t\t{team1.Team} - {team2.Team} ({team1.LatestMatch.Result})");
                    }
                    RotateTeams(group.Teams);
                }
                teams[group.Name].AddRange(group.Teams);
            }
            return teams;
        }
        private void RotateTeams(List<TeamEntity> teams)
        {
            var temp = teams[teams.Count - 1];
            for (int i = teams.Count - 1; i > 1; i--)
            {
                teams[i] = teams[i - 1];
            }
            teams[1] = temp;
        }
        public void PrintGroupStandings(Dictionary<string, List<TeamEntity>> groups)
        {
            Console.WriteLine(ConstantsEng.GroupStats);

            foreach (var group in groups)
            {
                Console.WriteLine($"{ConstantsEng.Group} {group.Key}:");

                var sortedTeams = SortTeams(group.Value);

                for (int i = 0; i < sortedTeams.Count; i++)
                {
                    var team = sortedTeams[i];
                    Console.WriteLine($"\t{i + 1}. {team.Team,-20} {team.Wins,2} / {team.Losses,2} / {team.Points,2} / {team.ScoredPoints,3} / {team.ReceivedPoints,3} / {team.PointDifference,3}");
                }
            }
        }
        public List<TeamEntity> RankAllGroups(Dictionary<string, List<TeamEntity>> groups)
        {
            var firstPlaces = new List<(string Group, TeamEntity Team)>();
            var secondPlaces = new List<(string Group, TeamEntity Team)>();
            var thirdPlaces = new List<(string Group, TeamEntity Team)>();

            foreach (var group in groups)
            {
                var sortedGroup = SortTeams(group.Value);
                firstPlaces.Add((group.Key, sortedGroup[0]));
                secondPlaces.Add((group.Key, sortedGroup[1]));
                thirdPlaces.Add((group.Key, sortedGroup[2]));
            }

            var firstPlacesRanked = SortTeams(firstPlaces.Select(t => t.Team).ToList());
            var secondPlacesRanked = SortTeams(secondPlaces.Select(t => t.Team).ToList());
            var thirdPlacesRanked = SortTeams(thirdPlaces.Select(t => t.Team).ToList());

            var allTeams = new List<TeamEntity>();

            void AddTeamsToList(List<TeamEntity> rankedTeams, List<(string Group, TeamEntity Team)> originalTeams, int baseRank)
            {
                for (int i = 0; i < rankedTeams.Count; i++)
                {
                    var team = rankedTeams[i];
                    var originalTeam = originalTeams.First(t => t.Team == team);
                    team.Group = originalTeam.Group;
                    team.Rank = baseRank + i;
                    allTeams.Add(team);
                }
            }

            AddTeamsToList(firstPlacesRanked, firstPlaces, 1);
            AddTeamsToList(secondPlacesRanked, secondPlaces, 4);
            AddTeamsToList(thirdPlacesRanked, thirdPlaces, 7);

            foreach (var team in allTeams)
            {
                Console.WriteLine($"\n{team.Rank}. {team.Team} - {ConstantsEng.Points}: {team.Points}, {ConstantsEng.PointDifference}: {team.PointDifference} ({ConstantsEng.Group}: {team.Group})");
            }

            allTeams.RemoveAt(allTeams.Count - 1);
            return allTeams;
        }
        public List<TeamEntity> SortTeams(List<TeamEntity> teams)
        {
            var sortedTeams = teams
                    .OrderByDescending(t => t.Points)
                    .ThenByDescending(t => t.PointDifference)
                    .ThenByDescending(t => t.ScoredPoints)
                    .ToList();
            return sortedTeams;
        }
        public List<(TeamEntity, TeamEntity)> SimulatePotDraw(List<TeamEntity> teams)
        {
            var potD = new List<TeamEntity>();
            var potE = new List<TeamEntity>();
            var potF = new List<TeamEntity>();
            var potG = new List<TeamEntity>();

            foreach (var team in teams)
            {
                if (team.Rank == 1 || team.Rank == 2)
                    potD.Add(team);
                else if (team.Rank == 3 || team.Rank == 4)
                    potE.Add(team);
                else if (team.Rank == 5 || team.Rank == 6)
                    potF.Add(team);
                else if (team.Rank == 7 || team.Rank == 8)
                    potG.Add(team);
            }

            var random = new Random();
            var quarterFinalPairs = new List<(TeamEntity, TeamEntity)>();

            quarterFinalPairs.AddRange(MakeQuarterFinalPairs(potD, potG, random));
            quarterFinalPairs.AddRange(MakeQuarterFinalPairs(potE, potF, random));

            Console.WriteLine("Pots:");
            PrintPot("D", potD);
            PrintPot("E", potE);
            PrintPot("F", potF);
            PrintPot("G", potG);

            Console.WriteLine($"\n{ConstantsEng.EliminationPhase}:");
            foreach (var (team1, team2) in quarterFinalPairs)
            {
                Console.WriteLine($"{team1.Team} - {team2.Team}");
            }

            return quarterFinalPairs;
        }

        private List<(TeamEntity, TeamEntity)> MakeQuarterFinalPairs(List<TeamEntity> pot1, List<TeamEntity> pot2, Random random)
        {
            var pairs = new List<(TeamEntity, TeamEntity)>();
            var remainingPot2Teams = new List<TeamEntity>(pot2);

            foreach (var team1 in pot1)
            {
                var possibleOpponents = remainingPot2Teams.Where(t => t.Group != team1.Group).ToList();
                if (possibleOpponents.Any())
                {
                    var opponent = possibleOpponents[random.Next(possibleOpponents.Count)];
                    pairs.Add((team1, opponent));
                    remainingPot2Teams.Remove(opponent);
                }
                else
                {
                    var opponent = remainingPot2Teams[random.Next(remainingPot2Teams.Count)];
                    pairs.Add((team1, opponent));
                    remainingPot2Teams.Remove(opponent);
                }
            }

            return pairs;
        }

        private void PrintPot(string potName, List<TeamEntity> pot)
        {
            Console.WriteLine($"\t{ConstantsEng.Pot} {potName}");
            foreach (var team in pot)
            {
                Console.WriteLine($"\t\t{team.Team}");
            }
        }

        public void SimulateEliminationPhase(List<(TeamEntity Team1, TeamEntity Team2)> quarterFinalPairs)
        {
            var semiFinalPairs = new List<(TeamEntity semiTeam1, TeamEntity semiTeam2)>();
            (TeamEntity, TeamEntity) bronzeMatch;
            (TeamEntity, TeamEntity) finalMatch;

            Console.WriteLine($"\n{ConstantsEng.Quarterfinals}:");

            for(int i=0; i<quarterFinalPairs.Count; i+=2)
            {
                var quarterWinner1 = SimulateMatch(quarterFinalPairs[i].Team1, quarterFinalPairs[i].Team2);
                var quarterWinner2 = SimulateMatch(quarterFinalPairs[i + 1].Team1, quarterFinalPairs[i + 1].Team2);                
                Console.WriteLine($"{quarterFinalPairs[i].Team1.Team} - {quarterFinalPairs[i].Team2.Team} ({quarterFinalPairs[i].Team1.LatestMatch.Result})");
                Console.WriteLine($"{quarterFinalPairs[i + 1].Team1.Team} - {quarterFinalPairs[i + 1].Team2.Team} ({quarterFinalPairs[i + 1].Team1.LatestMatch.Result})");

                semiFinalPairs.Add((quarterWinner1, quarterWinner2));
            }

            Console.WriteLine($"\n{ConstantsEng.Semifinals}:");

            var semiWinner1 = SimulateMatch(semiFinalPairs[0].semiTeam1, semiFinalPairs[0].semiTeam2);
            var semiWinner2 = SimulateMatch(semiFinalPairs[1].semiTeam1, semiFinalPairs[1].semiTeam2);
            var semiLoser1 = (semiFinalPairs[0].semiTeam1 == semiWinner1) ? semiFinalPairs[0].semiTeam2 : semiFinalPairs[0].semiTeam1;
            var semiLoser2 = (semiFinalPairs[1].semiTeam1 == semiWinner2) ? semiFinalPairs[1].semiTeam2 : semiFinalPairs[1].semiTeam1;
            Console.WriteLine($"{semiFinalPairs[0].semiTeam1.Team} - {semiFinalPairs[0].semiTeam2.Team} ({semiFinalPairs[0].semiTeam1.LatestMatch.Result})");
            Console.WriteLine($"{semiFinalPairs[1].semiTeam1.Team} - {semiFinalPairs[1].semiTeam2.Team} ({semiFinalPairs[1].semiTeam1.LatestMatch.Result})");

            finalMatch = (semiWinner1, semiWinner2);
            bronzeMatch = (semiLoser1, semiLoser2);

            Console.WriteLine($"\n{ConstantsEng.ThirdPlaceGame}:");

            var bronzeWinner = SimulateMatch(bronzeMatch.Item1, bronzeMatch.Item2);
            Console.WriteLine($"{bronzeMatch.Item1.Team} - {bronzeMatch.Item2.Team} ({bronzeMatch.Item1.LatestMatch.Result})");           

            Console.WriteLine($"\n{ConstantsEng.Finals}:");
            var winner = SimulateMatch(finalMatch.Item1, finalMatch.Item2);
            var finalLoser = (finalMatch.Item1 == winner) ? finalMatch.Item2 : finalMatch.Item1;
            Console.WriteLine($"{finalMatch.Item1.Team} - {finalMatch.Item2.Team} ({finalMatch.Item1.LatestMatch.Result})");

            Console.WriteLine($"\n{ConstantsEng.Medals}:");
            Console.WriteLine($"{ConstantsEng.Gold}: { winner.Team}");
            Console.WriteLine($"{ConstantsEng.Silver}: {finalLoser.Team}");
            Console.WriteLine($"{ConstantsEng.Bronze}: {bronzeWinner.Team}");
        }

    }
}
