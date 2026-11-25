namespace BarangayElection.Services;

/// <summary>Outcome categories for a vote or an eligibility check.</summary>
public enum VoteStatus
{
    /// <summary>Vote accepted (or, for an eligibility check, the voter may vote).</summary>
    Accepted,
    EmptyVoterId,
    UnknownVoter,
    AlreadyVoted,
    EmptyCandidateId,
    UnknownCandidate
}

/// <summary>Status plus a helpful message the UI can show directly.</summary>
public readonly record struct VoteResult(VoteStatus Status, string Message)
{
    public bool IsAccepted => Status == VoteStatus.Accepted;
}
