using HtmlAgilityPack;

public static class HtmlHelper
{
    internal static string GetSingleNodeAttributeValue(string html, string xpath, string attributeName = "value")
    {
        var htmlDocument = LoadHtmlDocument(html);
        return GetSingleNodeAttributeValue(htmlDocument, xpath, attributeName);
    }

    internal static string GetSingleNodeAttributeValue(HtmlDocument document, string xpath, string attributeName)
    {
        var node = document.DocumentNode.SelectSingleNode(xpath);
        return node.GetAttributeValue(attributeName, "").Trim();
    }

    internal static HtmlDocument LoadHtmlDocument(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        return document;
    }
}
