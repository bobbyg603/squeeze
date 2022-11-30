# Squeeze

Squeeze contains a .NET 7.0 command-line tool for creating zip files from [glob patterns](https://en.wikipedia.org/wiki/Glob_(programming)).

## ⚙️ Installation

The `Squeeze` command-line tool can be installed globally via [dotnet](https://dotnet.microsoft.com/).

```sh
dotnet tool install -g Squeeze
```

## 🏗️ Usage

Run `dotnet squeeze -h` to see usage information.

```sh
C:\Users\bobby> dotnet squeeze -h
Description:
  Create a zip via a manifest file containing glob pattern rules

Usage:
  squeeze <input> <output> [options]

Arguments:
  <input>   Zip file manifest containing glob patterns of file paths to include
  <output>  Path to zip file output

Options:
  --force         Overwrite output file if it exists [default: False]
  --verbose       Show verbose log statements [default: False]
  --version       Show version information
  -?, -h, --help  Show help and usage information
```

For the `input` argument, pass the name of a file containing glob patterns of files to include. 

[input.txt](./input.txt)
```txt
path/to/folder/**/*
README.md
LICENSE.md
```

For the `output` argument, pass a path to the location of the output file. If you'd like to overwrite the output file if it exists, add the `--force` option. To increase log verbosity add the `--verbose` flag to your command's arguments.
