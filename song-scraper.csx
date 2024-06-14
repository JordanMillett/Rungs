#r "nuget: HtmlAgilityPack, 1.11.61"
#r "nuget: HtmlAgilityPack.CssSelectors.NetCore, 1.2.1"

using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

async Task Main(string[] args)
{
    if (args.Length < 4)
    {
        Console.WriteLine("Usage: dotnet script song-scraper.csx <artist> <title>");
        return;
    }
    
    string artist = args[2];
    string title = args[3];

    string url = "https://www.musixmatch.com/lyrics/" + artist.Replace(" ", "-") + "/" + title.Replace(" ", "-");
    
    try
    {
        HttpClient httpClient = new HttpClient();
        string html = await httpClient.GetStringAsync(url);

        //string htmlFilePath = "scraped.html";
        //await File.WriteAllTextAsync(htmlFilePath, html);
        //Console.WriteLine($"HTML content saved to {htmlFilePath}");

        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        
        string lyrics = title + "\n" + artist + "\n\n" + ExtractLyrics(htmlDocument);
        
        string extractedFilePath = $"wwwroot/data/songs/{artist + title}.txt";
        await File.WriteAllTextAsync(extractedFilePath, lyrics);
        Console.WriteLine($"Lyrics saved to {extractedFilePath}");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static string ExtractLyrics(HtmlDocument document)
{
    List<string> verseMarkers = new List<string> { "chorus", "verse", "outro", "bridge", "pre-chorus", "hook", "intro"};
    
    var lyricsStartMarker = "Lyrics of";
    var lyricsEndMarker = "Writer(s):";

    var lyrics = new StringBuilder();
    bool isLyricsSection = false;

    var startNode = document.DocumentNode.SelectSingleNode($"//*[contains(text(), '{lyricsStartMarker}')]");

    if (startNode == null)
    {
        return "Lyrics start marker not found.";
    }

    // Traverse through subsequent sibling nodes
    foreach (var node in startNode.ParentNode.ChildNodes)
    {
        // Start collecting lyrics when we reach the node containing "Lyrics of"
        if (node == startNode)
        {
            isLyricsSection = true;
            continue;
        }

        // Stop collecting lyrics when we reach the node containing "Writer(s):"
        if (isLyricsSection && node.InnerText.Contains(lyricsEndMarker))
        {
            break;
        }

        // If we are inside the lyrics section, add the text to the StringBuilder
        if (isLyricsSection && !string.IsNullOrWhiteSpace(node.InnerText))
        {
            // Check if the node is a <div> or process its text directly
            if (node.Name == "div")
            {
                foreach (var divNode in node.ChildNodes)
                {
                    if (string.IsNullOrWhiteSpace(divNode.InnerText) ||
                    verseMarkers.Contains(divNode.InnerText.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        lyrics.AppendLine();
                        continue;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(divNode.InnerText))
                    {
                        lyrics.AppendLine(divNode.InnerText.Trim().Replace("&quot;", "\""));
                    }
                }
            }
            else
            {
                // Directly append the text of the node
                lyrics.AppendLine(node.InnerText.Trim().Replace("&quot;", "\""));
            }
            
            /*
            foreach (var divNode in node.Descendants("div"))
            {
                Console.WriteLine(divNode.InnerText);
                
                // Skip empty or "chorus"/"verse" nodes within deeper <div> elements
                if (string.IsNullOrWhiteSpace(divNode.InnerText) ||
                    verseMarkers.Contains(divNode.InnerText.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    lyrics.AppendLine();
                    continue;
                }

                lyrics.Append(divNode.ChildNodes.FirstOrDefault().InnerText.Trim());
                lyrics.AppendLine();
            }*/
        }
    }

    return lyrics.ToString().Trim();
}

// Ensure to call Main with the command-line arguments
await Main(Environment.GetCommandLineArgs());
