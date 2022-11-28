﻿using System.CommandLine;
using ZipCreator;

//
// This C# program creates a zip archive from a file containing a list of files to include in the archive.
// Paths in the provided inputFilePath text file should be relative to the input files location.
// Add an interceptor to perform actions on files before zipping such as code signing.
// Readonly attributes are cleared prior to zipping and restored afterwards.
//
// Usage:
// zippy <path to file containing list of files> <path to zip archive that will be created> [...options]
//

public class Program
{
    [STAThread]
    static public async Task<int> Main(string[] args)
    {
        var inputFileArg = new Argument<FileInfo>(
            name: "input",
            description: "Zip file manifest containing glob patterns of file paths to include"
        );

        var outputFileArg = new Argument<FileInfo>(
            name: "output",
            description: "Path to zip file output"
        );

        var forceOption = new Option<bool>(
            name: "--force",
            getDefaultValue: () => false,
            description: "Overwrite output file if it exists"
        );

        var verboseOption = new Option<bool>(
            name: "--verbose",
            getDefaultValue: () => false,
            description: "Show verbose log statements"
        );

        var rootCommand = new RootCommand("Create a zip via a manifest file containing glob pattern rules");
        rootCommand.AddArgument(inputFileArg);
        rootCommand.AddArgument(outputFileArg);
        rootCommand.AddOption(forceOption);
        rootCommand.AddOption(verboseOption);

        Action<FileInfo, FileInfo, bool, bool> handler = (inputFile, outputFile, force, verbose) =>
        {
            Log.Level = verbose ? LogLevel.Verbose : LogLevel.Normal;

            if (!inputFile.Exists)
            {
                Log.Info($"Zippy failed, input file {inputFile.FullName} does not exist");
                // TODO BG how do we get dotnet to fail with non-zero return code?
                Environment.Exit(-1);
            }

            var inputFileParentDirectory = inputFile.Directory;
            var inputFileParentDirectoryPath = inputFileParentDirectory!.FullName;
            Directory.SetCurrentDirectory(inputFileParentDirectoryPath);

            var zip = Zip.CreateFromFile(inputFile);
            zip.Settings.Output = outputFile;
            zip.Settings.Overwrite = force;
            zip.Settings.Interceptors.Add((f) => Log.Verbose($"Adding {f.FullName}..."));

            Log.Info($"Running zippy with input {inputFile.FullName}...");

            var result = zip.Write();

            if (result == ZipWriteResult.OverwriteError)
            {
                Log.Info($"Zippy failed, {outputFile.FullName} exists and overwrite is false");
                Environment.Exit(-1);
            }

            Log.Info($"Zippy created {outputFile.FullName}");
        };

        rootCommand.SetHandler(handler, inputFileArg, outputFileArg, forceOption, verboseOption);

        return await rootCommand.InvokeAsync(args);
    }
}