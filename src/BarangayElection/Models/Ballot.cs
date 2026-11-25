namespace BarangayElection.Models;

/// <summary>
/// An anonymous ballot. It stores ONLY the chosen candidate ID.
/// There is deliberately no voter ID, name, timestamp or counter here,
/// so a stored ballot can never be traced back to a voter.
/// </summary>
public sealed class Ballot
{
    // internal: only code inside this project (ElectionManager) can create ballots.
    internal Ballot(string candidateId) => CandidateId = candidateId;

    public string CandidateId { get; }
}
