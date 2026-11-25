using BarangayElection.Data;
using BarangayElection.Services;
using BarangayElection.UI;

// Entry point: build the objects, load fictional sample data, start the menu.

try { Console.OutputEncoding = System.Text.Encoding.UTF8; } // needed for box-drawing characters
catch (IOException) { /* no console attached - ignore */ }

var election = new ElectionManager();
SampleData.Load(election);

new MenuController(election, new ConsoleUi()).Run();
