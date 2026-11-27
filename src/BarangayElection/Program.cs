using BarangayElection.Data;
using BarangayElection.Services;
using BarangayElection.UI;

try { Console.OutputEncoding = System.Text.Encoding.UTF8; }
catch (IOException) { }

var election = new ElectionManager();
SampleData.Load(election);

new MenuController(election, new ConsoleUi()).Run();