using BasketballTournament.Models;
using System.Text.Json;

namespace BasketballTournament.Data
{
    public class DataLoader
    {
        public static List<Group> LoadGroups(string groupsPath, string exibitionsPath)
        {
            var jsonGroups = File.ReadAllText(groupsPath);
            var rawGroups = JsonSerializer.Deserialize<Dictionary<string, List<TeamEntity>>>(jsonGroups);
            var jsonExibitions = File.ReadAllText(exibitionsPath);
            var rawMatches = JsonSerializer.Deserialize<Dictionary<string, List<Match>>>(jsonExibitions);

            var matchDictionary = rawMatches.ToDictionary(entry => entry.Key, entry => entry.Value);

            var groups = new List<Group>();

            foreach (var groupEntry in rawGroups)
            {
                var group = new Group(groupEntry.Key);
                group.Teams.AddRange(groupEntry.Value);
                groups.Add(group);

                foreach (var team in groupEntry.Value)
                {
                    if (matchDictionary.TryGetValue(team.ISOCode, out var teamMatches))
                    {
                        team.Matches.AddRange(teamMatches);
                    }
                }
            }
            return groups;
        }       
    }
}
