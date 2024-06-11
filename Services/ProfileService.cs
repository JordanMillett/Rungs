using System.Diagnostics;

public class ProfileService
{
    public event Action OnProfileSave = () => { };
    
    Blazored.LocalStorage.ILocalStorageService Local;
    DebugService Debug;
    //FileService FileService;

    //-----     SAVED DATA     -----
    public string ProfileName = "User";
    public List<string> Terms = new List<string>(); //make into full standalone dictionary
    public List<string> RecentPages = new List<string>();

    public ProfileService(Blazored.LocalStorage.ILocalStorageService localStorage, DebugService debug)
    {
        Local = localStorage;
        Debug = debug;
        //FileService = fileservice;
    }

    void InitializeProfile()
    {
        ProfileName = "User";
        //Terms = new Dictionary<string, Term>();
        RecentPages = new List<string>();
    }

    public async Task InitializeAsync()
    {
        InitializeProfile();
        
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        await LoadProfile();

        stopwatch.Stop();
        Debug.LogSuccess($"Loaded {Terms.Count} terms in {stopwatch.ElapsedMilliseconds}ms");
    }
    
    public async Task UpdateRecent(string Link)
    {
        for(int i = 0; i < RecentPages.Count; i++)
        {
            if (RecentPages[i] == Link)
                RecentPages.RemoveAt(i);
        }

        RecentPages.Insert(0, Link);

        int recentSize = 4;

        if (RecentPages.Count > recentSize)
        {
            RecentPages.RemoveAt(recentSize);
        }

        await SaveProfile();
    }
    
    public async Task AddTerm(string Value)
    {
        if (!string.IsNullOrWhiteSpace(Value))
        {
            Value = Value.ToLower();
            
            if (!Terms.Contains(Value))
            {
                Terms.Add(Value);
                Debug.Log("Added Term: " + Value);
                await SaveProfile();
            }
            /*
            if (!Terms.ContainsKey(Value))
            {
                Terms.Add(Value, new Term(Value, FileService.AllTerms[Value].EN));
                Debug.Log("Added Term: " + Value);
                await SaveProfile();
            }*/
        }
    }
    
    public async Task RemoveTerm(string Value)
    {
        Terms.Remove(Value.ToLower());
        Debug.Log("Removed Term: " + Value);
        await SaveProfile();
    }
    
    public bool HasTerm(string Value)
    {
        return Terms.Contains(Value.ToLower());
        //return Terms.ContainsKey(Value.ToLower());
    }
    
    public async Task ToggleTerm(string Value)
    {        
        if(HasTerm(Value))
        {
            await RemoveTerm(Value);
        }else
        {
            await AddTerm(Value);
        }
    }

    public async Task LoadProfile()
    {
        if(await Local.ContainKeyAsync("name"))
        {
            ProfileName = await Local.GetItemAsync<string>("name") ?? ProfileName;
        }
        
        if(await Local.ContainKeyAsync("terms"))
        {
            //Terms = await Local.GetItemAsync<Dictionary<string, Term>>("terms") ?? Terms;
            Terms = await Local.GetItemAsync<List<string>>("terms") ?? Terms;
        }
        
        if(await Local.ContainKeyAsync("recent"))
        {
            RecentPages = await Local.GetItemAsync<List<string>>("recent") ?? RecentPages;
        }
    }
    
    public async Task SaveProfile()
    {
        await Local.SetItemAsync<string>("name", ProfileName);
        
        //await Local.SetItemAsync<Dictionary<string, Term>>("terms", Terms);
        await Local.SetItemAsync<List<string>>("terms", Terms);
        
        await Local.SetItemAsync<List<string>>("recent", RecentPages);
        
        OnProfileSave?.Invoke();
    }
    
    public async Task ClearProfile()
    {
        await Local.ClearAsync();
        Debug.Log("Cleared Profile");
        InitializeProfile();
    }
}
