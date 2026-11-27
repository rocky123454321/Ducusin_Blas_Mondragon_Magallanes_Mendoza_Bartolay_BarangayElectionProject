namespace BarangayElection.Services;

using BarangayElection.Models;

public sealed class ElectionManager
{
    public const string Position = "Barangay Chairperson";

    private readonly List<Candidate> _candidates = new();
    private readonly Dictionary<string, Voter> _voters = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Ballot> _ballots = new();
    private readonly Random _random = new();

    public IReadOnlyList<Candidate> Candidates => _candidates.AsReadOnly();

    public IReadOnlyList<Voter> Voters =>
        _voters.Values.OrderBy(v => v.Id, StringComparer.Ordinal).ToList();

    public int RegisteredVoterCount => _voters.Count;
    public int VotesCast => _ballots.Count;
    public int RemainingVoterCount => _voters.Values.Count(v => !v.HasVoted);

    public IEnumerable<ElectionParticipant> GetAllParticipants() =>
        _candidates.Cast<ElectionParticipant>().Concat(Voters.Cast<ElectionParticipant>());

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

    public VoteResult CheckVoterEligibility(string? voterId) => CheckVoter(voterId, out _);

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

        _ballots.Insert(_random.Next(_ballots.Count + 1), new Ballot(candidate.Id));
        voter.MarkAsVoted();

        return new(VoteStatus.Accepted,
            $"Vote recorded for voter {voter.Id}. The ballot choice is stored anonymously.");
    }

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

    private void EnsureRegistrationOpen()
    {
        if (_ballots.Count > 0)
            throw new InvalidOperationException("Registration is closed because voting has started.");
    }
}