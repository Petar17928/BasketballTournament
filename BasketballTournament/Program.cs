using BasketballTournament.Models;
using BasketballTournament.Data;
using BasketballTournament.Services;

namespace BasketballTournament
{
    class Program
    {
        static void Main(string[] args)
        {
            SimulationService simulationService = new SimulationService();
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(currentDirectory).FullName;
            string dataPath = Path.Combine(projectDirectory, "BasketballTournament\\Data", "groups.json");
            string exibitionsPath = Path.Combine(projectDirectory, "BasketballTournament\\Data", "exibitions.json");
            List<Group> groups = DataLoader.LoadGroups(dataPath,exibitionsPath);
        
            var teams = simulationService.SimulateGroupPhase(groups);
            simulationService.PrintGroupStandings(teams);

            var rankedTeams = simulationService.RankAllGroups(teams);
            var eliminationTeams = simulationService.SimulatePotDraw(rankedTeams);
            simulationService.SimulateEliminationPhase(eliminationTeams);
            Console.ReadLine();
        }
    }
}
