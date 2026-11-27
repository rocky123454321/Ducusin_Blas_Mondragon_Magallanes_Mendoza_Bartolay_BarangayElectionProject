namespace BarangayElection.Models;

public sealed class Voter : ElectionParticipant
{
    public Voter(string id, string name) : base(id, name)
    {
    }

    public bool HasVoted { get; private set; }

    public override string Role => "Registered Voter";

    public override string GetSummary() =>
        $"{Id} | {Name} | {(HasVoted ? "Already voted" : "Eligible to vote")}";

    internal void MarkAsVoted() => HasVoted = true;
}