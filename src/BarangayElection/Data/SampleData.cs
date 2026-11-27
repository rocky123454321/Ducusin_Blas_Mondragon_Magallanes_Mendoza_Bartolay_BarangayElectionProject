namespace BarangayElection.Data;

using BarangayElection.Models;
using BarangayElection.Services;

public static class SampleData
{
    public static void Load(ElectionManager election)
    {
        election.RegisterCandidate(new Candidate("C01", "Ana Mendoza",
            "Cleaner drainage, regular street lighting checks, and safer streets for everyone."));
        election.RegisterCandidate(new Candidate("C02", "Ramon Villareal",
            "Youth sports programs, livelihood training, and a barangay scholarship fund."));
        election.RegisterCandidate(new Candidate("C03", "Liza Fernandez",
            "Open budget reports, faster document processing, and a stronger health center."));

        string[] voterNames =
        {
            "Rocky Ducusin", "Tristan Blas", "Cjosh Mondragon", "Ford Magallanes",
            "Elijah Mendoza", "Katsutoshi Bartolay", "Lourdes Aquino", "Benito Cruz",
            "Carmela Dizon", "Teodoro Evangelista", "Felisa Garcia", "Gregorio Hernandez"
        };

        for (int i = 0; i < voterNames.Length; i++)
            election.RegisterVoter(new Voter($"V{i + 1:000}", voterNames[i]));
    }
}