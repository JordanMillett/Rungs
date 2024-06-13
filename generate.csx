#r "nuget:Newtonsoft.Json,13.0.3"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

public class FileData
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string URL { get; set; }
    public string Category { get; set; }
}

public struct Term
{
    public string RU { get; set; }
    public string EN { get; set; }

    public Term(string ru, string en)
    {
        RU = ru;
        EN = en;
    }
}

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

Program.client = new HttpClient();
Program.client.Timeout = TimeSpan.FromSeconds(1);
string url = $"http://localhost:5148";
try
{
    HttpResponseMessage response = await Program.client.GetAsync(url);
    Program.RuslexRunning = response.IsSuccessStatusCode;
}catch
{
    Program.RuslexRunning = false;
}

await Program.GetFiles("songs");
await Program.GetFiles("texts");

string json = JsonConvert.SerializeObject(Program.files, Formatting.Indented);
File.WriteAllText(@"wwwroot\data\filelist.json", json);

if (Program.RuslexRunning)
{
    json = JsonConvert.SerializeObject(Program.terms, Formatting.Indented);
    File.WriteAllText(@"wwwroot\data\terms.json", json);

    json = JsonConvert.SerializeObject(Program.missing_terms, Formatting.Indented);
    File.WriteAllText(@"wwwroot\data\missing_terms.json", json);
} 
   
stopwatch.Stop(); 
TimeSpan elapsed = stopwatch.Elapsed;

if (Program.RuslexRunning)
{
    Console.WriteLine($"{Program.terms.Count} terms loaded");
    Console.WriteLine($"{Program.missing_terms.Count} terms not found");
}else
{
    Console.WriteLine("Ruslex not running, term files not changed");
}
Console.WriteLine($"{Program.files.Count} files generated in {elapsed.TotalSeconds} seconds");

public class Program
{
    public static List<FileData> files = new List<FileData>();
    public static Dictionary<string, Term> terms = new Dictionary<string, Term>();
    public static List<string> missing_terms = new List<string>();
    public static HttpClient client;

    public static bool RuslexRunning = false;
    
    public static async Task GetFiles(string folder)
    {
        string directory = @"wwwroot\data\" + folder;
        
        foreach (string opened in Directory.GetFiles(directory, "*.txt"))
        {
            FileInfo fileInfo = new FileInfo(opened);
            string hash = GetMD5Hash(opened);
            string newFileName = hash + fileInfo.Extension;
            string newFilePath = Path.Combine(directory, newFileName);
            
            try
            {
                File.Move(opened, newFilePath);
            }catch
            {
                Console.WriteLine($"Filepath {opened} is a duplicate");
                continue;
            }

            string[] lines = File.ReadAllLines(newFilePath);
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

            if (RuslexRunning)
            {
                for (int i = 3; i < lines.Length; i++)
                {
                    if (!String.IsNullOrWhiteSpace(lines[i]))
                    {
                        string[] words = lines[i].Split(new char[] { ' ', '.', '?', ',', '-', '!', ';', ':', '—', '"', '\'', '–' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string word in words)
                        {
                            string key = word.ToLower().Replace("ё", "е"); ;

                            if (!terms.ContainsKey(key) && !missing_terms.Contains(key))
                            {
                                string url = $"http://localhost:5148/translations/{Uri.EscapeDataString(key)}";
                                HttpResponseMessage response = await client.GetAsync(url);

                                if (response.IsSuccessStatusCode)
                                {
                                    string jsonResponse = await response.Content.ReadAsStringAsync();
                                    Term T = JsonConvert.DeserializeObject<Term>(jsonResponse);

                                    terms.Add(key, T);
                                }
                                else
                                {
                                    missing_terms.Add(key);
                                }
                            }


                        }

                    }
                }
            }
        }
    }
    
    public static string GetMD5Hash(string filePath)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            using (var reader = new StreamReader(filePath))
            {
                var sb = new StringBuilder();
                for (int i = 0; i < 2; i++)
                {
                    string line = reader.ReadLine();
                    sb.AppendLine(line);
                }

                byte[] contentBytes = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] hashBytes = md5.ComputeHash(contentBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }

}