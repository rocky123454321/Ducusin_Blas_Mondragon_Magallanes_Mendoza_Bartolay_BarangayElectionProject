namespace BarangayElection.Services;

using BarangayElection.Models;

/// <summary>Votes and percentage for one candidate.</summary>
public sealed record CandidateResult(Candidate Candidate, int Votes, double Percentage);

/// <summary>Computed totals for the whole election. Contains no voter information.</summary>
public sealed record ElectionTally(
    IReadOnlyList<CandidateResult> Results,
    int TotalVotes,
    int RegisteredVoters)
{
    public int RemainingVoters => RegisteredVoters - TotalVotes;

    public double TurnoutPercentage =>
        RegisteredVoters == 0 ? 0 : TotalVotes * 100.0 / RegisteredVoters;

    /// <summary>Candidate(s) with the highest vote count (more than one means a tie). Empty if no votes yet.</summary>
    public IReadOnlyList<CandidateResult> Leaders
    {
        get
        {
            if (TotalVotes == 0) return Array.Empty<CandidateResult>();
            int max = Results.Max(r => r.Votes);
            return Results.Where(r => r.Votes == max).ToList();
        }
    }
}
