namespace BarangayElection.UI;

using BarangayElection.Models;
using BarangayElection.Services;

/// <summary>
/// The menu "controller": reads what the user types, calls ElectionManager, and asks
/// ConsoleUi to draw the answer. It contains NO voting rules - those live in ElectionManager.
/// </summary>
internal sealed class MenuController
{
    private static readonly ConsoleColor[] CandidateColors =
    {
        ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Yellow,
        ConsoleColor.Magenta, ConsoleColor.Blue
    };

    private readonly ElectionManager _election;
    private readonly ConsoleUi _ui;

    public MenuController(ElectionManager election, ConsoleUi ui)
    {
        _election = election;
        _ui = ui;
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            ShowHome();
            string? choice = _ui.Prompt("Select an option (0-5)");

            if (choice is null) break; // input stream ended - exit cleanly

            switch (choice.Trim())
            {
                case "1": ListCandidates(); break;
                case "2": CastVote(); break;
                case "3": ViewResults(); break;
                case "4": ShowVoterStatus(); break;
                case "5": ShowParticipantDirectory(); break;
                case "0": running = false; break;
                case "":
                    _ui.Error("No input received. Type a number from 0 to 5.");
                    _ui.Pause();
                    break;
                default:
                    _ui.Error($"'{choice.Trim()}' is not a valid option. Valid choices: 1, 2, 3, 4, 5 or 0 to exit.");
                    _ui.Pause();
                    break;
            }
        }

        _ui.Clear();
        _ui.Header("Goodbye!", "Thank you for trying the Barangay Election Simulation.");
    }

    // ---------- Screens ----------

    private void ShowHome()
    {
        _ui.Clear();
        _ui.Header("BARANGAY ELECTION SIMULATION",
            $"Position: {ElectionManager.Position}      ");

        double turnout = _election.RegisteredVoterCount == 0
            ? 0
            : _election.VotesCast * 100.0 / _election.RegisteredVoterCount;

        _ui.StatCards(new[]
        {
            new StatCard("Registered", _election.RegisteredVoterCount.ToString(), "fictional data", ConsoleColor.Cyan),
            new StatCard("Votes cast", _election.VotesCast.ToString(), "anonymous", ConsoleColor.Green),
            new StatCard("Remaining", _election.RemainingVoterCount.ToString(), "still eligible", ConsoleColor.Yellow),
            new StatCard("Turnout", $"{turnout:0.0}%", "of registered", ConsoleColor.Magenta)
        });

        _ui.BlankLine();
        _ui.Panel("MAIN MENU", new[]
        {
            new UiLine("[1]  List candidates"),
            new UiLine("[2]  Cast a vote"),
            new UiLine("[3]  View election results"),
            new UiLine("[4]  Voter status / remaining eligible voters"),
            new UiLine("[5]  Participant directory (polymorphism demo)"),
            new UiLine("[0]  Exit", ConsoleColor.DarkGray)
        });

    }

    private void ListCandidates()
    {
        _ui.Clear();
        _ui.Header("CANDIDATES", $"Running for {ElectionManager.Position}");
        ShowCandidatePanel();
        _ui.Pause();
    }

    private void CastVote()
    {
        _ui.Clear();
        _ui.Header("CAST A MOCK VOTE", "Step 1 of 2 - identify the voter");

        string? voterId = _ui.Prompt("Enter Voter ID (e.g. V001)");

        // Check the voter BEFORE showing the ballot, so ineligible voters are stopped early.
        VoteResult eligibility = _election.CheckVoterEligibility(voterId);
        if (!eligibility.IsAccepted)
        {
            _ui.BlankLine();
            _ui.Error(eligibility.Message);
            _ui.Pause();
            return;
        }

        _ui.Clear();
        _ui.Header("CAST A MOCK VOTE", "Step 2 of 2 - choose a candidate");
        ShowCandidatePanel();

        string? candidateId = _ui.Prompt("Enter Candidate ID (e.g. C01)");
        VoteResult result = _election.CastVote(voterId, candidateId);

        if (!result.IsAccepted)
        {
            _ui.BlankLine();
            _ui.Error(result.Message);
            _ui.Pause();
            return;
        }

        // Wipe the screen so the next person cannot see which candidate was chosen.
        _ui.Clear();
        _ui.Header("VOTE COMPLETE", "Thank you!");
        _ui.Success(result.Message);
        _ui.Info("The screen was cleared so your choice is not shown to the next voter.");
        _ui.Pause();
    }

    private void ViewResults()
    {
        ElectionTally tally = _election.GetTally();

        _ui.Clear();
        _ui.Header("ELECTION RESULTS", "SIMULATED RESULTS - NOT OFFICIAL");

        var lines = new List<UiLine>();
        for (int i = 0; i < tally.Results.Count; i++)
        {
            CandidateResult r = tally.Results[i];
            ConsoleColor color = CandidateColors[i % CandidateColors.Length];

            lines.Add(new UiLine($"{r.Candidate.Id}  {r.Candidate.Name}", ConsoleColor.White));
            lines.Add(new UiLine(
                $"{ConsoleUi.Bar(r.Percentage, 30)}  {r.Votes,3} vote(s)  {r.Percentage,5:0.0}%", color));
            lines.Add(new UiLine(""));
        }

        lines.Add(new UiLine($"Total ballots counted : {tally.TotalVotes} of {tally.RegisteredVoters} registered voters ({tally.TurnoutPercentage:0.0}% turnout)"));

        if (tally.RemainingVoters > 0)
            lines.Add(new UiLine($"Voting not finished   : {tally.RemainingVoters} registered voter(s) have not voted yet.", ConsoleColor.Yellow));

        lines.Add(new UiLine(DescribeLeaders(tally), ConsoleColor.Green));

        _ui.Panel("VOTE COUNT BY CANDIDATE", lines);
        _ui.Pause();
    }

    private void ShowVoterStatus()
    {
        _ui.Clear();
        _ui.Header("VOTER STATUS", "Shows who has voted - never who they voted for");

        var lines = new List<UiLine>();
        foreach (Voter voter in _election.Voters)
        {
            string status = voter.HasVoted ? "VOTED" : "NOT YET VOTED";
            ConsoleColor color = voter.HasVoted ? ConsoleColor.Green : ConsoleColor.Yellow;
            lines.Add(new UiLine($"{voter.Id,-6} {voter.Name,-26} {status}", color));
        }

        lines.Add(new UiLine(""));
        lines.Add(new UiLine(
            $"Remaining eligible voters: {_election.RemainingVoterCount} of {_election.RegisteredVoterCount}",
            ConsoleColor.White));

        _ui.Panel("VOTER REGISTRY", lines);
        _ui.Pause();
    }

    /// <summary>
    /// POLYMORPHISM DEMO: one list typed as the base class ElectionParticipant holds both
    /// Candidates and Voters. The same calls (p.Role, p.GetSummary()) run a different
    /// override depending on the real object type - decided at runtime.
    /// </summary>
    private void ShowParticipantDirectory()
    {
        _ui.Clear();
        _ui.Header("PARTICIPANT DIRECTORY", "Candidates and voters shown through the base type");

        var lines = new List<UiLine>();
        foreach (ElectionParticipant participant in _election.GetAllParticipants())
        {
            lines.Add(new UiLine($"[{participant.Role}]", ConsoleColor.Cyan));
            lines.Add(new UiLine("   " + participant.GetSummary())); // Voter or Candidate override runs here
        }

        _ui.Panel("ALL PARTICIPANTS", lines);
        _ui.Pause();
    }

    // ---------- Helpers ----------

    private void ShowCandidatePanel()
    {
        var lines = new List<UiLine>();
        foreach (Candidate candidate in _election.Candidates)
        {
            lines.Add(new UiLine($"{candidate.Id}  {candidate.Name}", ConsoleColor.White));
            lines.Add(new UiLine($"     Platform: {candidate.Platform}", ConsoleColor.Gray));
            lines.Add(new UiLine(""));
        }

        _ui.Panel($"CANDIDATES - {ElectionManager.Position.ToUpperInvariant()}", lines);
    }

    private static string DescribeLeaders(ElectionTally tally)
    {
        if (tally.TotalVotes == 0)
            return "No votes have been cast yet.";

        string label = tally.RemainingVoters == 0 ? "Winner (simulated)" : "Current leader";

        if (tally.Leaders.Count == 1)
        {
            CandidateResult leader = tally.Leaders[0];
            return $"{label}: {leader.Candidate.Name} with {leader.Votes} vote(s)";
        }

        string names = string.Join(" & ", tally.Leaders.Select(l => l.Candidate.Name));
        return $"Tie: {names} with {tally.Leaders[0].Votes} vote(s) each";
    }
}
