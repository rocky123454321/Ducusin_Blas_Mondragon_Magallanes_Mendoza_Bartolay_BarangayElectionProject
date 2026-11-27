namespace BarangayElection.Services;

public enum VoteStatus
{
    Accepted,
    EmptyVoterId,
    UnknownVoter,
    AlreadyVoted,
    EmptyCandidateId,
    UnknownCandidate
}

public readonly record struct VoteResult(VoteStatus Status, string Message)
{
    public bool IsAccepted => Status == VoteStatus.Accepted;
}