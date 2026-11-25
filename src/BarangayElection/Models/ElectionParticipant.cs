namespace BarangayElection.Models;

/// <summary>
/// ABSTRACTION: the shared idea of "someone who takes part in the election".
/// It holds the data every participant has (ID + name) and forces each concrete
/// type to describe itself through <see cref="Role"/> and <see cref="GetSummary"/>.
/// </summary>
public abstract class ElectionParticipant
{
    protected ElectionParticipant(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        // IDs are normalised so "v001" and "V001" are treated as the same ID.
        Id = id.Trim().ToUpperInvariant();
        Name = name.Trim();
    }

    /// <summary>Unique ID (read-only after construction).</summary>
    public string Id { get; }

    /// <summary>Full name (read-only after construction).</summary>
    public string Name { get; }

    /// <summary>Human-readable role, e.g. "Registered Voter". Each subtype must define it.</summary>
    public abstract string Role { get; }

    /// <summary>One-line description. Each subtype decides what to show (polymorphism).</summary>
    public abstract string GetSummary();

    public override string ToString() => $"{Role}: {Id} - {Name}";
}
