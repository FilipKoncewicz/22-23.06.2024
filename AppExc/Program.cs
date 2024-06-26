﻿using System.Diagnostics;
using System.Text;
using AppExc1;


var countOfFiles = 0;
var sizeOfFile = 0;
char? pattern = null;
byte[]? key = null;
string? outputFilePath = null;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-n":
            i++;
            countOfFiles = int.Parse(args[i]);
            break;
        case "-s":
            i++;
            sizeOfFile = int.Parse(args[i]);
            break;
        case "-p":
            i++;
            pattern = char.Parse(args[i]);
            break;
        case "-k":
            i++;
            key = Encoding.UTF8.GetBytes(args[i]);
            break;
        case "-o":
            i++;
            outputFilePath = args[i];
            break;
    }
}

if (countOfFiles == 0 || sizeOfFile == 0 || pattern == null || key == null || outputFilePath == null)
{
    Console.WriteLine("Invalid arguments");
    return;
}

var service = new ApmAsync();

var listOfTasks = new List<Task<string>>();
var sw = new Stopwatch();
sw.Start();
var token = new CancellationToken();

for (var i = 0; i < countOfFiles; i++)
{
    if (token.IsCancellationRequested)
    {
        break;
    }
    
    var res = service.CalculateSha("./dummy_file_" + i, pattern.Value, sizeOfFile, key);
    listOfTasks.Add(res);
}

var allLines = await Task.WhenAll(listOfTasks);

sw.Stop();
await File.WriteAllLinesAsync(outputFilePath, allLines);

Console.WriteLine("The end in time: " + sw.Elapsed);