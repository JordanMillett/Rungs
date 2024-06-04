#r "nuget:Newtonsoft.Json,13.0.3"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

public class Song
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string URL { get; set; }
}

Main();

void Main()
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    
    string directory = @"wwwroot\data\songs";
    string songListFilePath = @"wwwroot\data\songlist.txt";

    List<Song> songs = new List<Song>();

    foreach (string file in Directory.GetFiles(directory, "*.txt"))
    {
        FileInfo fileInfo = new FileInfo(file);
        string hash = GetMD5Hash(file);
        string newFileName = hash + fileInfo.Extension;
        File.Move(file, Path.Combine(directory, newFileName));

        string[] lines = File.ReadAllLines(Path.Combine(directory, newFileName));
        string title = lines[0];
        string artist = lines[1];

        Song song = new Song
        {
            Title = title,
            Artist = artist,
            URL = hash
        };

        songs.Add(song);
    }

    string json = JsonConvert.SerializeObject(songs, Formatting.Indented);
    File.WriteAllText(songListFilePath, json);
        
    stopwatch.Stop(); 
    TimeSpan elapsed = stopwatch.Elapsed;

    Console.WriteLine($"Song list generated in {elapsed.TotalSeconds} seconds");
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