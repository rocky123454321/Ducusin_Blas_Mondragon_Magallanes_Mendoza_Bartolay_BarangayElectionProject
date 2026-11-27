namespace BarangayElection.Models;

public abstract class ElectionParticipant
{
    protected ElectionParticipant(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Id = id.Trim().ToUpperInvariant();
        Name = name.Trim();
    }

    public string Id { get; }

    public string Name { get; }

    public abstract string Role { get; }

    public abstract string GetSummary();

    public override string ToString() => $"{Role}: {Id} - {Name}";
}