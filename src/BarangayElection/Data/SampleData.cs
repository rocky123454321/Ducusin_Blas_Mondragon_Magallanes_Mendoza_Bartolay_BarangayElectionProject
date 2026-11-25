namespace BarangayElection.Data;

using BarangayElection.Models;
using BarangayElection.Services;

/// <summary>
/// In-memory FICTIONAL sample data. No real people, no files, no database.
/// Edit this file to add more candidates or voters.
/// </summary>
public static class SampleData
{
    public static void Load(ElectionManager election)
    {
        // Candidates (at least 3)
        election.RegisterCandidate(new Candidate("C01", "Ana Mendoza",
            "Cleaner drainage, regular street lighting checks, and safer streets for everyone."));
        election.RegisterCandidate(new Candidate("C02", "Ramon Villareal",
            "Youth sports programs, livelihood training, and a barangay scholarship fund."));
        election.RegisterCandidate(new Candidate("C03", "Liza Fernandez",
            "Open budget reports, faster document processing, and a stronger health center."));

        // Registered voters (at least 10)
        string[] voterNames =
        {
            "Lourdes Aquino", "Benito Cruz", "Carmela Dizon", "Teodoro Evangelista",
            "Felisa Garcia", "Gregorio Hernandez", "Imelda Javier", "Jose Katigbak",
            "Luzviminda Manalo", "Nestor Ocampo", "Patricia Quizon", "Rodel Salazar"
        };

        for (int i = 0; i < voterNames.Length; i++)
            election.RegisterVoter(new Voter($"V{i + 1:000}", voterNames[i]));
    }
}
