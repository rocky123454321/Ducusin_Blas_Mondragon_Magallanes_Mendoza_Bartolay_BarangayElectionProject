namespace BarangayElection.Models;

/// <summary>
/// INHERITANCE: a Candidate is an ElectionParticipant that also has a platform statement.
/// </summary>
public sealed class Candidate : ElectionParticipant
{
    public Candidate(string id, string name, string platform) : base(id, name)
    {
        if (string.IsNullOrWhiteSpace(platform))
            throw new ArgumentException("Platform statement is required.", nameof(platform));

        Platform = platform.Trim();
    }

    /// <summary>Short platform statement.</summary>
    public string Platform { get; }

    public override string Role => "Candidate - Barangay Chairperson";

    public override string GetSummary() => $"{Id} | {Name} | \"{Platform}\"";
}
