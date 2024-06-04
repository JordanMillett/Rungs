using System.Diagnostics;
using System.Text.Json;

public interface ISongService
{
    Task InitializeAsync();
    Task<List<Song>> Search(string searchTerm);
}

public class Song
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Link { get; set; }

    public Song(string title, string artist, string link)
    {
        Title = title;
        Artist = artist;
        Link = link;
    }
}

public class SongService : ISongService
{
    Dictionary<string, Song> AllSongs;

    HttpClient Http { get; set; }

    public SongService(HttpClient httpClient)
    {
        AllSongs = new Dictionary<string, Song>();
        Http = httpClient;
    }

    public async Task InitializeAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        await LoadSongList();
        
        stopwatch.Stop();
        Console.WriteLine($"Loaded {AllSongs.Count} songs in {stopwatch.ElapsedMilliseconds}ms");
    }

    async Task LoadSongList()
    {
        string JSON = await Http.GetStringAsync("data/songlist.txt");

        List<Song> LoadedSongs = JsonSerializer.Deserialize<List<Song>>(JSON)!;

        foreach (Song S in LoadedSongs)
        {
            AddSong(S.Title, S.Artist, S.Link);
        }
    }

    private void AddSong(string title, string artist, string link)
    {
        Song S = new Song(title, artist, link);
        AllSongs.Add(title, S);
    }

    public async Task<List<Song>> Search(string searchTerm)
    {
        return await Task.Run(() =>
        {
            var matchedSongs = new List<Song>();

            foreach (var song in AllSongs.Values)
            {
                if (song.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    song.Artist.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    matchedSongs.Add(song);
                }
            }

            return matchedSongs;
        });
    }
}
