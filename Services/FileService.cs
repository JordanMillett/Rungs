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

public class FileService
{
    public Dictionary<string, File> AllFiles;
    public Dictionary<string, Term> AllTerms = new Dictionary<string, Term>();

    DebugService Debug;
    HttpClient Http { get; set; }
    ProfileService Profile;

    public FileService(HttpClient httpClient, DebugService debug, ProfileService profile)
    {
        AllFiles = new Dictionary<string, File>();
        Http = httpClient;
        Debug = debug;
        Profile = profile;
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
            string JSON = await Http.GetStringAsync("data/filelist.txt");
            List<File> LoadedSongs = JsonSerializer.Deserialize<List<File>>(JSON)!;

            foreach (File F in LoadedSongs)
            {
                AddFile(F.Title, F.Author, F.URL, F.Category);
            }
        }catch
        {
            Debug.LogError("Failed to load JSON, clearing profile...");
            await Profile.ClearProfile();
        }
    }
    
    async Task LoadTerms()
    {
        try
        {
            string JSON = await Http.GetStringAsync("data/terms.txt");
            AllTerms = JsonSerializer.Deserialize<Dictionary<string, Term>>(JSON)!;
        }catch
        {
            Debug.LogError("Failed to load JSON, clearing profile...");
            await Profile.ClearProfile();
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
