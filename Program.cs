IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var androidAppsRoot = Configuration["Folders:AndroidAppsRoot"];
var searchFor = Configuration["Files:SearchFor"];
var apkFile = Configuration["Files:ApkFile"];
var reviewSourceDir = Configuration["Files:ReviewSourceDir"];

var onlyPath = string.Empty;
var destination = string.Empty;
var rvwDestination = string.Empty;

var separator = Path.DirectorySeparatorChar;

string? fileToCopySource = string.Empty;
string? fileToCopyTarget = string.Empty;

if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    fileToCopySource = Configuration["Commands:Mac:FileToCopySource"];
    fileToCopyTarget = Configuration["Commands:Mac:FileToCopyTarget"];
}
else
{
    fileToCopySource = Configuration["Commands:Windows:FileToCopySource"];
    fileToCopyTarget = Configuration["Commands:Windows:FileToCopyTarget"];
}

var appFolder = Directory.GetCurrentDirectory();

fileToCopySource = $"{appFolder}{separator}{fileToCopySource}";

Console.WriteLine($"RootFolder={androidAppsRoot}");
Console.WriteLine($"searchFor={searchFor}");

string[] absoluteFileNames = Directory.GetFiles(androidAppsRoot!, searchFor!, SearchOption.AllDirectories);

Console.WriteLine($"Total Number of submissions: {absoluteFileNames.Length}");

int ndx = 0;
foreach (var item in absoluteFileNames)
{
    onlyPath = item.Substring(0, item.IndexOf(searchFor!));
    rvwDestination = $"{onlyPath}{separator}{reviewSourceDir}";
    Directory.CreateDirectory(rvwDestination);

    Console.WriteLine($"\n**************** Submission {++ndx} ****************");

    Console.WriteLine($"CHANGE directory to: {Directory.GetCurrentDirectory()}");

    destination = $"{onlyPath}{fileToCopyTarget}";

    Console.WriteLine($"\n++++++++++ START copy file {fileToCopySource} to {destination} ++++++++++");
    File.Copy(fileToCopySource, destination, true);
    Console.WriteLine($"++++++++++ END copy file {fileToCopySource} to {destination} ++++++++++");

    Console.WriteLine($"\n########## START execute script ##########");
    Directory.SetCurrentDirectory(onlyPath);
    executeApp();
    Console.WriteLine($"########## END execute script ##########");

    Console.WriteLine($"\n~~~~~~~~~~ START copy APK file ~~~~~~~~~~");
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

    Console.WriteLine($"~~~~~~~~~~ END copy APK file ~~~~~~~~~~");

    Console.WriteLine($"\n========== START copy source code ==========");
    string sourceCodeLocation = $"{onlyPath}";
    sourceCodeLocation += $"app{separator}";
    sourceCodeLocation += $"src{separator}";
    sourceCodeLocation += $"main{separator}.";

    Directory.CreateDirectory(rvwDestination);

    Console.WriteLine($"Copying source code files FROM  \n{sourceCodeLocation} \nTO \n{rvwDestination}");
    UnzipLHub.Models.Helper.CopyDirectory(sourceCodeLocation, rvwDestination, true);

    Console.WriteLine($"========== END copy source code ==========");
}

void executeApp()
{
    string? arguments = string.Empty;
    string? command = string.Empty;

    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        command = Configuration["Commands:Mac:Script"];
        arguments = appFolder + separator + Configuration["Commands:Mac:Arguments"];
    } else {
        command = Configuration["Commands:Windows:Script"];
        arguments = Configuration["Commands:Windows:Arguments"];
    }

     var processInfo = new ProcessStartInfo()
    {
        FileName = command,
        Arguments = arguments,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        CreateNoWindow = true

    };
    Console.WriteLine($"Currrent Directory: {Directory.GetCurrentDirectory()}");
    Console.WriteLine($"{processInfo.FileName} {processInfo.Arguments}");

    System.Diagnostics.Process? process = Process.Start(processInfo);   // Start that process.
    while (!process!.StandardOutput.EndOfStream)
    {
        string? result = process.StandardOutput.ReadLine();
        // do something here
    }
    process.WaitForExit();
}