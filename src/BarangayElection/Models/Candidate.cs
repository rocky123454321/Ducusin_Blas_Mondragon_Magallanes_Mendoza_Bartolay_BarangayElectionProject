namespace BarangayElection.Models;

public sealed class Candidate : ElectionParticipant
{
    public Candidate(string id, string name, string platform) : base(id, name)
    {
        if (string.IsNullOrWhiteSpace(platform))
            throw new ArgumentException("Platform statement is required.", nameof(platform));

        Platform = platform.Trim();
    }

    public string Platform { get; }

    public override string Role => "Candidate - Barangay Chairperson";

    public override string GetSummary() => $"{Id} | {Name} | \"{Platform}\"";
}