namespace BarangayElection.Services;

using BarangayElection.Models;

public sealed record CandidateResult(Candidate Candidate, int Votes, double Percentage);

public sealed record ElectionTally(
    IReadOnlyList<CandidateResult> Results,
    int TotalVotes,
    int RegisteredVoters)
{
    public int RemainingVoters => RegisteredVoters - TotalVotes;

    public double TurnoutPercentage =>
        RegisteredVoters == 0 ? 0 : TotalVotes * 100.0 / RegisteredVoters;

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