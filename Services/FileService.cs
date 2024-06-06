using System.Diagnostics;
using System.Text.Json;

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

public class FileService
{
    Dictionary<string, File> AllFiles;

    DebugService Debug;
    HttpClient Http { get; set; }

    public FileService(HttpClient httpClient, DebugService debug)
    {
        AllFiles = new Dictionary<string, File>();
        Http = httpClient;
        Debug = debug;
    }

    public async Task InitializeAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        await LoadFileList();
        
        stopwatch.Stop();
        Debug.Log($"Loaded {AllFiles.Count} files in {stopwatch.ElapsedMilliseconds}ms");
    }

    async Task LoadFileList()
    {
        string JSON = await Http.GetStringAsync("data/songlist.txt");
        List<File> LoadedSongs = JsonSerializer.Deserialize<List<File>>(JSON)!;

        foreach (File F in LoadedSongs)
        {
            AddFile(F.Title, F.Author, F.URL, "songs");
        }
    }

    private void AddFile(string title, string author, string url, string category)
    {
        File F = new File(title, author, url, category);
        AllFiles.Add(title, F);
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
