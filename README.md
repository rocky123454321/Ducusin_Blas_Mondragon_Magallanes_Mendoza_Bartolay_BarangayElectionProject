# Barangay Election Simulation

A C# **console application** for an Object-Oriented Programming final project.
It simulates a small **Barangay Chairperson** election using **fictional, in-memory data only**.

> **Simulation only.** This is a classroom demo, not an official election system. It uses no real
> voter records, no files, no database and no network, and it must not be used for a real election.

---

## Requirements

| Item | Value |
|------|-------|
| .NET SDK | **.NET 8 SDK or newer** (8, 9 or 10 all work) |
| OS | Windows, macOS or Linux |
| Terminal | Use a UTF-8 capable terminal (Windows Terminal / VS Code terminal) so the box and bar characters draw correctly |

Check your version with `dotnet --version`.
If you only have .NET 9/10, it still runs (the project is set to roll forward). You may also change
`<TargetFramework>net8.0</TargetFramework>` in `BarangayElection.csproj` to `net9.0` / `net10.0`.

## Build and run

```bash
# from the project root folder (where this README is)
dotnet build
dotnet run --project src/BarangayElection
```

In Visual Studio: open `BarangayElection.sln` and press **F5**.

## Folder structure

```
BarangayElectionProject/
├── README.md
├── BarangayElection.sln
├── .gitignore
└── src/
    └── BarangayElection/
        ├── BarangayElection.csproj
        ├── Program.cs                 Entry point (creates objects, starts the menu)
        ├── Models/                    The "things" in the election
        │   ├── ElectionParticipant.cs   abstract base class
        │   ├── Voter.cs                 inherits ElectionParticipant
        │   ├── Candidate.cs             inherits ElectionParticipant
        │   └── Ballot.cs                anonymous ballot (candidate ID only)
        ├── Services/                  The rules and calculations
        │   ├── ElectionManager.cs       owns private data, validates votes, counts results
        │   ├── ElectionTally.cs         result objects (totals, percentages, leaders)
        │   └── VoteResult.cs            status + message returned for every vote attempt
        ├── Data/
        │   └── SampleData.cs            fictional candidates and voters (edit here)
        └── UI/                        Everything the user sees
            ├── ConsoleUi.cs             dashboard drawing: stat cards, panels, bars, prompts
            └── MenuController.cs        reads input, calls ElectionManager, shows screens
```

**Where to change things**

- Add/rename candidates or voters → `Data/SampleData.cs`
- Change voting rules → `Services/ElectionManager.cs`
- Change colours, layout or the dashboard look → `UI/ConsoleUi.cs`
- Add a menu option → `UI/MenuController.cs`

## Menu and sample flow

| Option | What it does |
|--------|--------------|
| 1 | List candidates (ID, name, platform) |
| 2 | Cast a vote: enter a Voter ID, then a Candidate ID |
| 3 | View results: votes, percentages and bars (labelled *simulated*) |
| 4 | Voter status: who has voted and how many eligible voters remain |
| 5 | Participant directory: candidates and voters shown through the base class (polymorphism demo) |
| 0 | Exit |

**Sample flow**

1. Run the program → the dashboard and main menu appear.
2. Type `1` → see candidates `C01`, `C02`, `C03`.
3. Type `2`, enter `V001`, then `C01` → vote recorded, screen cleared.
4. Type `2`, enter `V001` again → rejected (already voted).
5. Type `3` → totals show 1 vote for C01.
6. Type `0` → program exits normally.

Sample IDs: voters `V001`–`V012`, candidates `C01`–`C03` (not case-sensitive).

## OOP pillars: where to find them

| Pillar | Where | How |
|--------|-------|-----|
| **Encapsulation** | `ElectionManager` | `_candidates`, `_voters`, `_ballots` are `private`. Outside code only gets read-only views and must use `RegisterCandidate`, `RegisterVoter` and `CastVote`, which validate everything. `Voter.HasVoted` has a private setter, `Ballot` has an `internal` constructor, and registration locks after the first ballot. |
| **Abstraction** | `ElectionParticipant` | Abstract class holding the shared `Id` and `Name`, and requiring every subtype to implement `Role` and `GetSummary()`. |
| **Inheritance** | `Voter`, `Candidate` | Both inherit `Id`/`Name` and validation from `ElectionParticipant`. `Voter` adds `HasVoted`; `Candidate` adds `Platform`. |
| **Polymorphism** | `MenuController.ShowParticipantDirectory()` | A `foreach (ElectionParticipant p in ...)` loop calls `p.Role` and `p.GetSummary()`. The runtime picks the `Voter` or `Candidate` override. Try menu option 5. |

## Privacy and integrity design

- **Identity and choice are stored in different places.** The voter registry only stores *whether* a voter has voted (`Voter.HasVoted`). The ballot list only stores the *candidate ID*.
- **A `Ballot` has no voter ID**, no name, no timestamp and no counter, so nothing links it to a voter.
- **Ballots are inserted at random positions**, so the order of ballots cannot reveal the order in which people voted.
- **The screen is cleared after voting** so the next person cannot see the choice.
- **Results contain counts only**: no output ever pairs a voter with a candidate.
- **Duplicate votes are blocked** by checking `HasVoted` before a ballot is stored. A vote is saved only if the voter ID is registered, the voter has not voted, and the candidate exists. If any check fails nothing changes.
- **All data is fictional** and lives only in memory.

## Manual test record

| Test | Input / action | Expected result |
|------|----------------|-----------------|
| List candidates | Option 1 | All 3 candidates and platforms appear |
| Valid vote | `V001`, `C01` | Accepted; V001 shows VOTED |
| Repeat vote | `V001` again | Rejected; totals unchanged |
| Unknown voter | `V999` | Clear message; no ballot recorded |
| Unknown candidate | `V002`, `C99` | Clear message; no ballot recorded; V002 can still vote |
| Empty input | Press Enter at voter or candidate prompt | Helpful message; back to menu |
| Results | Option 3 after known votes | Counts and percentages match |
| Ballot privacy | Check options 3 and 4 | No screen pairs a voter with a candidate |
| Invalid menu input | `9` or Enter | Explains valid choices; continues |
| Exit | Option 0 | Program ends normally |

(Fill in the **Actual result** and **Pass/Fail** columns in your report after running them yourselves.)

## Limitations

- Classroom simulation only; not legally or technically suitable for a real election.
- All data is in memory, so it resets every time the program restarts.
- There is no voter authentication: anyone who types a valid voter ID can vote for that voter.
- Voting never "closes" automatically; results can be viewed at any time and are labelled interim until everyone has voted.
- The console scrollback of a terminal is outside the program's control. Clearing the screen is a best-effort measure.
- Only one position (Barangay Chairperson) is supported.
