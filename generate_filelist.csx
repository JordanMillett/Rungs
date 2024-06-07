#r "nuget:Newtonsoft.Json,13.0.3"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

public class FileData
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string URL { get; set; }
    public string Category { get; set; }
}

Main();

void Main()
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    
    List<FileData> files = new List<FileData>();


    GetFiles(files, "songs");
    GetFiles(files, "texts");


    string fileListFilePath = @"wwwroot\data\filelist.txt";
    string json = JsonConvert.SerializeObject(files, Formatting.Indented);
    File.WriteAllText(fileListFilePath, json);
        
    stopwatch.Stop(); 
    TimeSpan elapsed = stopwatch.Elapsed;

    Console.WriteLine($"File list generated in {elapsed.TotalSeconds} seconds");
}

void GetFiles(List<FileData> files, string folder)
{
    string directory = @"wwwroot\data\" + folder;
    
    foreach (string opened in Directory.GetFiles(directory, "*.txt"))
    {
        FileInfo fileInfo = new FileInfo(opened);
        string hash = GetMD5Hash(opened);
        string newFileName = hash + fileInfo.Extension;
        File.Move(opened, Path.Combine(directory, newFileName));

        string[] lines = File.ReadAllLines(Path.Combine(directory, newFileName));
        string title = lines[0];
        string author = lines[1];

        FileData F = new FileData
        {
            Title = title,
            Author = author,
            URL = hash,
            Category = folder
        };

        files.Add(F);
    }
}

string GetMD5Hash(string filePath)
{
    using (var md5 = System.Security.Cryptography.MD5.Create())
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            byte[] hashBytes = md5.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}