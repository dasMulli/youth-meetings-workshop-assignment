using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using WorkshopAssignmentApp.Data;

namespace WorkshopAssignmentApp.Assignment
{
    public class AssignmentGenerator
    {
        private readonly WorkshopAssignmentDbContext dbContext;

        public AssignmentGenerator(WorkshopAssignmentDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool TryGenerateAssignments([NotNullWhen(true)] out IList<Assignment>? assignments)
        {
            var costConfiguration = new CostConfiguration(0, 2, 5);
            var spots = dbContext.Workshops.Select(w => new WorkshopSpots(w, w.MaxParticipants)).ToArray();
            var participants = dbContext.Participants.ToArray();
            var szenario = new AssignmentSzenario(spots, participants, costConfiguration);

            if (TrySolveSzenario(szenario, out var solution))
            {
                assignments = solution.Assignments;
                return true;
            }
            else
            {
                assignments = null;
                return false;
            }
        }

        private bool TrySolveSzenario(AssignmentSzenario szenario, out AssignmentSzenarioSolution solution)
        {
            // Rows = Participants (Assignment Possibilites for Resources), Columns = Workshops (Resources)

            var allSpots = szenario.WorkshopSpots.SelectMany(s => Enumerable.Range(0, s.Spots).Select(_ => s.Workshop)).ToList();

            var costs = Matrix<double>.Build.Dense(szenario.Participants.Length, allSpots.Count, double.PositiveInfinity);

            for (int i = 0; i < szenario.Participants.Length; i++)
            {
                Participant? participant = szenario.Participants[i];
                if (participant.FirstChoice is Workshop firstChoice)
                {
                    var cost = participant.FirstChoiceAssignmentCostOverride ?? szenario.CostConfiguration.FirstChoiceAssignmentCost;
                    var colIndex = allSpots.IndexOf(firstChoice);
                    var length = allSpots.LastIndexOf(firstChoice) - colIndex + 1;
                    costs.SetRow(i, colIndex, length, Vector<double>.Build.Dense(length, cost));
                }
                if (participant.SecondChoice is Workshop secondChoice)
                {
                    var cost = participant.SecondChoiceAssignmentCostOverride ?? szenario.CostConfiguration.SecondChoiceAssignmentCost;
                    var colIndex = allSpots.IndexOf(secondChoice);
                    var length = allSpots.LastIndexOf(secondChoice) - colIndex + 1;
                    costs.SetRow(i, colIndex, length, Vector<double>.Build.Dense(length, cost));
                }
                if (participant.ThirdChoice is Workshop thirdChoice)
                {
                    var cost = participant.ThirdChoiceAssignmentCostOverride ?? szenario.CostConfiguration.ThirdChoiceAssignmentCost;
                    var colIndex = allSpots.IndexOf(thirdChoice);
                    var length = allSpots.LastIndexOf(thirdChoice) - colIndex + 1;
                    costs.SetRow(i, colIndex, length, Vector<double>.Build.Dense(length, cost));
                }
            }

            solution = AssignmentSzenarioSolution.EmptySolution;

            var assignmentVector = HungarianAlgorithm.FindAssignments(costs);

            // The implementation of HungarianAlgorithm assumes the problem is solvable
            // (it was created to include worst-case assignments, like "no solution" assignments or self-assignments with appropriate cost).
            // Therefore we need to check if 0 is in there multiple times or any assignment was made for an Infinity cost connection (which technically is a solution..).

            if (assignmentVector.Count(i => i == 0) > 1)
            {
                return false;
            }

            var assignments = assignmentVector
                .Select((workshopSpotIndex, participantIndex) =>
                {
                    var participant = szenario.Participants[participantIndex];
                    var workshop = allSpots[workshopSpotIndex];

                    return new Assignment
                    (
                        szenario.Participants[participantIndex],
                        allSpots[workshopSpotIndex],
                        workshop switch
                        {
                            Workshop w when w == participant.FirstChoice => 1,
                            Workshop w when w == participant.SecondChoice => 2,
                            Workshop w when w == participant.ThirdChoice => 3,
                            _ => -1
                        },
                        costs[participantIndex, workshopSpotIndex]
                    );
                })
                .ToArray();

            if (assignments.Any(a => a.ChoiceNumber == -1 || double.IsPositiveInfinity(a.AssignmentCost)))
            {
                return false;
            }

            solution = new AssignmentSzenarioSolution(assignments, assignments.Sum(a => a.AssignmentCost));

            return true;
        }
    }

    public record CostConfiguration(double FirstChoiceAssignmentCost, double SecondChoiceAssignmentCost, double ThirdChoiceAssignmentCost);

    public record WorkshopSpots(Workshop Workshop, int Spots);

    public record AssignmentSzenario(WorkshopSpots[] WorkshopSpots, Participant[] Participants, CostConfiguration CostConfiguration);

    public record Assignment(Participant Participant, Workshop Workshop, int ChoiceNumber, double AssignmentCost);

    public record AssignmentSzenarioSolution(Assignment[] Assignments, double Cost)
    {
        public static readonly AssignmentSzenarioSolution EmptySolution = new AssignmentSzenarioSolution(new Assignment[0], double.PositiveInfinity);
    };
}
