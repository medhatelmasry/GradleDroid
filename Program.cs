IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var rootFolder = Configuration["Folders:RootFolder"];
var searchFor = Configuration["Files:SearchFor"];
var fileToCopy = Configuration["Files:FileToCopy"];
var onlyPath = string.Empty;
var destination = string.Empty;

Console.WriteLine($"RootFolder={rootFolder}");
Console.WriteLine($"searchFor={searchFor}");

string[] absoluteFileNames = Directory.GetFiles(rootFolder, searchFor, SearchOption.AllDirectories);

Console.WriteLine($"Total Number of submissions: {absoluteFileNames.Length}");

int ndx = 0;
foreach (var item in absoluteFileNames)
{
    Console.WriteLine($"**************** Submission {++ndx} ****************");
    onlyPath = item.Substring(0, item.IndexOf(searchFor));

    Console.WriteLine($"CHANGE directory to: {Directory.GetCurrentDirectory()}");

    destination = $"{onlyPath}{fileToCopy}";

    Console.WriteLine($"**** START copy file {fileToCopy} to {destination}");
    File.Copy(fileToCopy, destination,true);
    Console.WriteLine($"**** END copy file {fileToCopy} to {destination}");

    Console.WriteLine($"**** START execute script");
    Directory.SetCurrentDirectory(onlyPath);
    executeApp();
    Console.WriteLine($"**** END execute script");
}

void executeApp()
{
    string commandToExecute = Configuration["Commands:CommandToExecute"];

    var command = "bash";
    var arguments = string.Format("{0} {1} {2} {3} {4}", "testarg1", "testarg2", "testarg3", "testarg3", "testarg4");
    var processInfo = new ProcessStartInfo()
    {
        FileName = command,
        Arguments = commandToExecute,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        CreateNoWindow = true

    };

    System.Diagnostics.Process? process = Process.Start(processInfo);   // Start that process.
    while (!process!.StandardOutput.EndOfStream)
    {
        string? result = process.StandardOutput.ReadLine();
        // do something here
    }
    process.WaitForExit();
}