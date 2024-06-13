using System.Diagnostics;
using System.Text.Json;

public class FileService
{
    public Dictionary<string, File> AllFiles;
    public Dictionary<string, Term> AllTerms = new Dictionary<string, Term>();

    DebugService Debug;
    HttpClient Http;

    public FileService(HttpClient http, DebugService debug)
    {
        AllFiles = new Dictionary<string, File>();
        Http = http;
        Debug = debug;
    }

    public async Task InitializeAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        await LoadFileList();
        stopwatch.Stop();
        Debug.LogSuccess($"Loaded {AllFiles.Count} files in {stopwatch.ElapsedMilliseconds}ms");
        stopwatch = Stopwatch.StartNew();
        await LoadTerms();
        stopwatch.Stop();
        Debug.LogSuccess($"Loaded {AllTerms.Count} terms in {stopwatch.ElapsedMilliseconds}ms");
    }

    async Task LoadFileList()
    {
        try
        {
            string JSON = await Http.GetStringAsync("data/filelist.json");
            List<File> LoadedSongs = JsonSerializer.Deserialize<List<File>>(JSON)!;

            foreach (File F in LoadedSongs)
            {
                AddFile(F.Title, F.Author, F.URL, F.Category);
            }
        }catch
        {
            await Debug.HandleError("Failed to load filelist.json", HandleOptions.ClearProfile);
        }
    }
    
    async Task LoadTerms()
    {
        try
        {
            string JSON = await Http.GetStringAsync("data/terms.json");
            AllTerms = JsonSerializer.Deserialize<Dictionary<string, Term>>(JSON)!;
        }catch
        {
            await Debug.HandleError("Failed to load terms.json", HandleOptions.ClearProfile);
        }
    }

    private void AddFile(string title, string author, string url, string category)
    {
        File F = new File(title, author, url, category);
        AllFiles.Add(url, F);
    }

    public async Task<List<File>> Search(string searchTerm)
    {
        return await Task.Run(() =>
        {
            var matchedFiles = new List<File>();

            foreach (File F in AllFiles.Values)
            {
                if (F.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    F.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    matchedFiles.Add(F);
                }
            }

            return matchedFiles;
        });
    }
}

public class File
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string URL { get; set; }
    public string Category { get; set; }

    public File(string title, string author, string url, string category)
    {
        Title = title;
        Author = author;
        URL = url;
        Category = category;
    }
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