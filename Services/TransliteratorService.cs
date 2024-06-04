public interface ITransliterator
{
    string CyrillicToLatin(string cyrillicText);
    string LatinToCyrillic(string latinText);
}

public class TransliteratorService : ITransliterator
{
    private Dictionary<char, string> cyrillicToLatinMap = new Dictionary<char, string>
    {
        {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"},
        {'е', "e"}, {'ё', "yo"}, {'ж', "zh"}, {'з', "z"}, {'и', "i"},
        {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
        {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"},
        {'у', "u"}, {'ф', "f"}, {'х', "kh"}, {'ц', "ts"}, {'ч', "ch"},
        {'ш', "sh"}, {'щ', "sch"}, {'ъ', ""}, {'ы', "y"}, {'ь', ""},
        {'э', "e"}, {'ю', "yu"}, {'я', "ya"},
        {'А', "A"}, {'Б', "B"}, {'В', "V"}, {'Г', "G"}, {'Д', "D"},
        {'Е', "E"}, {'Ё', "Yo"}, {'Ж', "Zh"}, {'З', "Z"}, {'И', "I"},
        {'Й', "Y"}, {'К', "K"}, {'Л', "L"}, {'М', "M"}, {'Н', "N"},
        {'О', "O"}, {'П', "P"}, {'Р', "R"}, {'С', "S"}, {'Т', "T"},
        {'У', "U"}, {'Ф', "F"}, {'Х', "Kh"}, {'Ц', "Ts"}, {'Ч', "Ch"},
        {'Ш', "Sh"}, {'Щ', "Sch"}, {'Ъ', ""}, {'Ы', "Y"}, {'Ь', ""},
        {'Э', "E"}, {'Ю', "Yu"}, {'Я', "Ya"}
    };

    private Dictionary<string, char> latinToCyrillicMap;

    public TransliteratorService()
    {
        latinToCyrillicMap = new Dictionary<string, char>();

        foreach (var pair in cyrillicToLatinMap)
        {
            if (!latinToCyrillicMap.ContainsKey(pair.Value))
            {
                latinToCyrillicMap.Add(pair.Value, pair.Key);
            }
        }
    }

    public string CyrillicToLatin(string cyrillicText)
    {
       var latinText = "";

        foreach (var c in cyrillicText)
        {
            if (cyrillicToLatinMap.ContainsKey(c))
            {
                latinText += cyrillicToLatinMap[c];
            }
            else
            {
                latinText += c;
            }
        }

        return latinText;
    }

    public string LatinToCyrillic(string latinText)
    {
        var cyrillicText = "";

        for (int i = 0; i < latinText.Length; i++)
        {
            if (i < latinText.Length - 1 && latinToCyrillicMap.ContainsKey(latinText.Substring(i, 2)))
            {
                cyrillicText += latinToCyrillicMap[latinText.Substring(i, 2)];
                i++;
            }
            else if (latinToCyrillicMap.ContainsKey(latinText[i].ToString()))
            {
                cyrillicText += latinToCyrillicMap[latinText[i].ToString()];
            }
            else
            {
                cyrillicText += latinText[i];
            }
        }

        return cyrillicText;
    }
}