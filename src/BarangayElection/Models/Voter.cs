namespace BarangayElection.Models;

/// <summary>
/// INHERITANCE: a Voter is an ElectionParticipant that also tracks whether it has voted.
/// NOTE: this only records THAT the voter voted - never WHO they voted for.
/// </summary>
public sealed class Voter : ElectionParticipant
{
    public Voter(string id, string name) : base(id, name)
    {
    }

    /// <summary>True once this voter has cast a ballot. Can only be changed inside this project.</summary>
    public bool HasVoted { get; private set; }

    public override string Role => "Registered Voter";

    public override string GetSummary() =>
        $"{Id} | {Name} | {(HasVoted ? "Already voted" : "Eligible to vote")}";

    /// <summary>Called only by ElectionManager (internal) after a ballot is stored.</summary>
    internal void MarkAsVoted() => HasVoted = true;
}
