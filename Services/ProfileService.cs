using System.Diagnostics;

public class Term
{
    public string RU { get; set; }

    public Term(string ru)
    {
        RU = ru;
    }
}

public class ProfileService
{
    Blazored.LocalStorage.ILocalStorageService Local;

    //-----     SAVED DATA     -----
    public string ProfileName = "User";
    public Dictionary<string, Term> Terms = new Dictionary<string, Term>();

    public ProfileService(Blazored.LocalStorage.ILocalStorageService localStorage)
    {
        Local = localStorage;
    }

    public async Task InitializeAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        await LoadProfile();

        stopwatch.Stop();
        Console.WriteLine($"Loaded {Terms.Count} terms in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Profile: {ProfileName} loaded");
    }
    
    public async Task AddTerm(string Value)
    {
        if (!string.IsNullOrWhiteSpace(Value))
        {
            if (!Terms.ContainsKey(Value))
            {
                Terms.Add(Value, new Term(Value));
                await SaveProfile();
            }
        }
    }
    
    public async Task RemoveTerm(string Value)
    {
        Terms.Remove(Value);
        await SaveProfile();
    }

    public async Task LoadProfile()
    {
        if(await Local.ContainKeyAsync("name"))
        {
            ProfileName = await Local.GetItemAsync<string>("name") ?? ProfileName;
        }
        
        if(await Local.ContainKeyAsync("terms"))
        {
            Terms = await Local.GetItemAsync<Dictionary<string, Term>>("terms") ?? Terms;
        }
    }
    
    public async Task SaveProfile()
    {
        await Local.SetItemAsync<string>("name", ProfileName);
        
        await Local.SetItemAsync<Dictionary<string, Term>>("terms", Terms);
    }
}
