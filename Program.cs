IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var androidAppsRoot = Configuration["Folders:AndroidAppsRoot"];
var searchFor = Configuration["Files:SearchFor"];
var fileToCopy = Configuration["Files:FileToCopy"];
var apkFile = Configuration["Files:ApkFile"];
var reviewSourceDir = Configuration["Files:ReviewSourceDir"];

var onlyPath = string.Empty;
var destination = string.Empty;
var rvwDestination = string.Empty;

var appFolder = Directory.GetCurrentDirectory();

var separator = Path.DirectorySeparatorChar;

Console.WriteLine($"RootFolder={androidAppsRoot}");
Console.WriteLine($"searchFor={searchFor}");

string[] absoluteFileNames = Directory.GetFiles(androidAppsRoot, searchFor, SearchOption.AllDirectories);

Console.WriteLine($"Total Number of submissions: {absoluteFileNames.Length}");

int ndx = 0;
foreach (var item in absoluteFileNames)
{
    onlyPath = item.Substring(0, item.IndexOf(searchFor));
    rvwDestination = $"{onlyPath}{separator}{reviewSourceDir}";
    Directory.CreateDirectory(rvwDestination);

    Console.WriteLine($"\n**************** Submission {++ndx} ****************");

    Console.WriteLine($"CHANGE directory to: {Directory.GetCurrentDirectory()}");

    destination = $"{onlyPath}{fileToCopy}";

    Console.WriteLine($"\naaaaaaaaaa START copy file {fileToCopy} to {destination} aaaaaaaaaa");
    File.Copy(fileToCopy, destination, true);
    Console.WriteLine($"aaaaaaaaaa END copy file {fileToCopy} to {destination} aaaaaaaaaa");

    Console.WriteLine($"\nbbbbbbbbbb START execute script bbbbbbbbbb");
    Directory.SetCurrentDirectory(onlyPath);
    executeApp();
    Console.WriteLine($"bbbbbbbbbb END execute script bbbbbbbbbb");

    Console.WriteLine($"\ncccccccccc START copy APK file cccccccccc");
    string apkFileLocation = $"{onlyPath}";
    apkFileLocation += $"app{separator}";
    apkFileLocation += $"build{separator}";
    apkFileLocation += $"outputs{separator}";
    apkFileLocation += $"apk{separator}";
    apkFileLocation += $"debug{separator}";
    apkFileLocation += apkFile;

    destination = $"{onlyPath}{separator}{reviewSourceDir}{separator}{apkFile}";
    Console.WriteLine($"Copying APK file FROM \n{apkFileLocation} \nTO \n{destination}");

    if (File.Exists(apkFileLocation)) 
        File.Copy(apkFileLocation, destination, true);

    Console.WriteLine($"cccccccccc END copy APK file cccccccccc");

    Console.WriteLine($"\ndddddddddd START copy source code dddddddddd");
    string sourceCodeLocation = $"{onlyPath}";
    sourceCodeLocation += $"app{separator}";
    sourceCodeLocation += $"src{separator}";
    sourceCodeLocation += $"main{separator}.";

    Directory.CreateDirectory(rvwDestination);

    Console.WriteLine($"Copying source code files FROM  \n{sourceCodeLocation} \nTO \n{rvwDestination}");
    UnzipLHub.Models.Helper.CopyDirectory(sourceCodeLocation, rvwDestination, true);

    Console.WriteLine($"dddddddddd END copy source code dddddddddd");
}

void executeApp()
{
    string commandToExecute = string.Empty;

    var command = Configuration["Commands:Mac:Script"];

    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        commandToExecute = appFolder + separator + Configuration["Commands:Mac:CommandToExecute"];
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