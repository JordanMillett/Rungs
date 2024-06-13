#r "nuget: HtmlAgilityPack, 1.11.61"
#r "nuget: HtmlAgilityPack.CssSelectors.NetCore, 1.2.1"

using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

async Task Main(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet-script scraper.csx <url> <keyword>");
        return;
    }

    string url = args[0];
    string keyword = args[1];

    await Task.Delay(500);

    url = "https://example.com/";
    //Musixmatch

    try
    {
        HttpClient httpClient = new HttpClient();
        string html = await httpClient.GetStringAsync(url);

        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        HtmlNode firstHeader = htmlDocument.DocumentNode.SelectSingleNode("//h1");
        if (firstHeader != null)
        {
            Console.WriteLine("First <h1> Header:");
            Console.WriteLine(firstHeader.InnerText.Trim());
        }
        else
        {
            Console.WriteLine("No <h1> header found.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// Ensure to call Main with the command-line arguments
await Main(Environment.GetCommandLineArgs());
