public class ServiceManager
{
    DebugService DService;
    FileService FService;
    ProfileService PService;

    public ServiceManager(DebugService dservice, FileService fservice, ProfileService pservice)
    {
        DService = dservice;
        FService = fservice;
        PService = pservice;
    }
    
    public async Task InitializeAsync()
    {
        DService.Initialize(PService);
        await FService.InitializeAsync();
        await PService.InitializeAsync();

        DService.Log("All Services Started");
    }
}