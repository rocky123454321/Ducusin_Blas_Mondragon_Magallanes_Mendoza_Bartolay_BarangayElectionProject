namespace BarangayElection.Models;

public sealed class Ballot
{
    internal Ballot(string candidateId) => CandidateId = candidateId;

    public string CandidateId { get; }
}