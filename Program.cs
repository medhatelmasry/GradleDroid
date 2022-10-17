IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var androidAppsRoot = Configuration["Folders:AndroidAppsRoot"];
var searchFor = Configuration["Files:SearchFor"];
var fileToCopy = Configuration["Files:FileToCopy"];
var onlyPath = string.Empty;
var destination = string.Empty;

var appFolder = Directory.GetCurrentDirectory();

Console.WriteLine($"RootFolder={androidAppsRoot}");
Console.WriteLine($"searchFor={searchFor}");

string[] absoluteFileNames = Directory.GetFiles(androidAppsRoot, searchFor, SearchOption.AllDirectories);

Console.WriteLine($"Total Number of submissions: {absoluteFileNames.Length}");

int ndx = 0;
foreach (var item in absoluteFileNames)
{
    Console.WriteLine($"**************** Submission {++ndx} ****************");
    onlyPath = item.Substring(0, item.IndexOf(searchFor));

    Console.WriteLine($"CHANGE directory to: {Directory.GetCurrentDirectory()}");

    destination = $"{onlyPath}{fileToCopy}";

    Console.WriteLine($"**** START copy file {fileToCopy} to {destination}");
    File.Copy(fileToCopy, destination, true);
    Console.WriteLine($"**** END copy file {fileToCopy} to {destination}");

    Console.WriteLine($"**** START execute script");
    Directory.SetCurrentDirectory(onlyPath);
    executeApp();
    Console.WriteLine($"**** END execute script");
}

void executeApp()
{
    //string commandToExecute = appFolder + Path.DirectorySeparatorChar + Configuration["Commands:Mac:CommandToExecute"];
    string commandToExecute = string.Empty;

    var command = Configuration["Commands:Mac:Script"];

    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        commandToExecute = appFolder + Path.DirectorySeparatorChar + Configuration["Commands:Mac:CommandToExecute"];
    }

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