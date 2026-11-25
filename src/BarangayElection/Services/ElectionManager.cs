namespace BarangayElection.Services;

using BarangayElection.Models;

/// <summary>
/// ENCAPSULATION: owns all election state and is the ONLY way to change it.
///  - _candidates : who can be voted for
///  - _voters     : the voter registry (stores "has voted" status only)
///  - _ballots    : anonymous ballots (candidate ID only, no voter link)
/// All three collections are private; the outside world gets read-only views
/// and must go through RegisterX / CastVote, which enforce the rules.
/// </summary>
public sealed class ElectionManager
{
    public const string Position = "Barangay Chairperson";

    private readonly List<Candidate> _candidates = new();
    private readonly Dictionary<string, Voter> _voters = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Ballot> _ballots = new();
    private readonly Random _random = new();

    // ---------- Read-only views ----------

    public IReadOnlyList<Candidate> Candidates => _candidates.AsReadOnly();

    public IReadOnlyList<Voter> Voters =>
        _voters.Values.OrderBy(v => v.Id, StringComparer.Ordinal).ToList();

    public int RegisteredVoterCount => _voters.Count;
    public int VotesCast => _ballots.Count;
    public int RemainingVoterCount => _voters.Values.Count(v => !v.HasVoted);

    /// <summary>
    /// Candidates and voters as ONE list of the base type. Callers use it polymorphically:
    /// each item's own GetSummary()/Role override runs at runtime.
    /// </summary>
    public IEnumerable<ElectionParticipant> GetAllParticipants() =>
        _candidates.Cast<ElectionParticipant>().Concat(Voters.Cast<ElectionParticipant>());

    // ---------- Controlled registration (setup phase only) ----------

    public void RegisterCandidate(Candidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        EnsureRegistrationOpen();

        if (FindCandidate(candidate.Id) is not null)
            throw new InvalidOperationException($"Candidate ID '{candidate.Id}' already exists.");

        _candidates.Add(candidate);
    }

    public void RegisterVoter(Voter voter)
    {
        ArgumentNullException.ThrowIfNull(voter);
        EnsureRegistrationOpen();

        if (!_voters.TryAdd(voter.Id, voter))
            throw new InvalidOperationException($"Voter ID '{voter.Id}' already exists.");
    }

    // ---------- Voting ----------

    /// <summary>Checks (without changing anything) whether this voter ID may vote.</summary>
    public VoteResult CheckVoterEligibility(string? voterId) => CheckVoter(voterId, out _);

    /// <summary>
    /// Validates everything, then stores an anonymous ballot and marks the voter as voted.
    /// Nothing is changed unless ALL checks pass.
    /// </summary>
    public VoteResult CastVote(string? voterId, string? candidateId)
    {
        VoteResult voterCheck = CheckVoter(voterId, out Voter? voter);
        if (!voterCheck.IsAccepted || voter is null)
            return voterCheck;

        if (string.IsNullOrWhiteSpace(candidateId))
            return new(VoteStatus.EmptyCandidateId,
                "Candidate ID cannot be empty. Please enter an ID such as C01.");

        Candidate? candidate = FindCandidate(candidateId.Trim());
        if (candidate is null)
            return new(VoteStatus.UnknownCandidate,
                $"Candidate ID '{candidateId.Trim()}' does not exist. Choose an ID from the candidate list.");

        // PRIVACY: the ballot gets ONLY the candidate ID. It is inserted at a random position
        // so the order of ballots cannot reveal the order in which voters voted.
        _ballots.Insert(_random.Next(_ballots.Count + 1), new Ballot(candidate.Id));
        voter.MarkAsVoted();

        return new(VoteStatus.Accepted,
            $"Vote recorded for voter {voter.Id}. The ballot choice is stored anonymously.");
    }

    // ---------- Results ----------

    /// <summary>Counts anonymous ballots per candidate. Contains no voter information.</summary>
    public ElectionTally GetTally()
    {
        int total = _ballots.Count;

        var results = _candidates
            .Select(c =>
            {
                int votes = _ballots.Count(b =>
                    string.Equals(b.CandidateId, c.Id, StringComparison.OrdinalIgnoreCase));
                double percent = total == 0 ? 0 : votes * 100.0 / total;
                return new CandidateResult(c, votes, percent);
            })
            .ToList();

        return new ElectionTally(results, total, _voters.Count);
    }

    // ---------- Private helpers ----------

    private Candidate? FindCandidate(string id) =>
        _candidates.FirstOrDefault(c => string.Equals(c.Id, id, StringComparison.OrdinalIgnoreCase));

    private VoteResult CheckVoter(string? voterId, out Voter? voter)
    {
        voter = null;

        if (string.IsNullOrWhiteSpace(voterId))
            return new(VoteStatus.EmptyVoterId, "Voter ID cannot be empty. Please enter an ID such as V001.");

        string id = voterId.Trim();

        if (!_voters.TryGetValue(id, out Voter? found))
            return new(VoteStatus.UnknownVoter,
                $"Voter ID '{id}' is not registered. No ballot was recorded.");

        if (found.HasVoted)
            return new(VoteStatus.AlreadyVoted,
                $"Voter {found.Id} has already voted. Each voter can vote only once.");

        voter = found;
        return new(VoteStatus.Accepted, "Voter is eligible.");
    }

    // Once the first ballot exists, the registry is locked so records cannot change mid-election.
    private void EnsureRegistrationOpen()
    {
        if (_ballots.Count > 0)
            throw new InvalidOperationException("Registration is closed because voting has started.");
    }
}
