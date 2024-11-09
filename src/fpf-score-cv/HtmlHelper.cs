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

    internal static Dictionary<string, string> GetAttributeAndTextValuesFromNodeCollection(string html, string collectionXpath, string singleXPath, string attributeName = "value")
    {
        var values = new Dictionary<string, string>();
        var document = LoadHtmlDocument(html);
        var nodeCollection = document.DocumentNode.SelectNodes(collectionXpath);

        foreach (var node in nodeCollection)
        {
            var singleNode = node.SelectSingleNode(singleXPath);
            var attributeValue = singleNode?.GetAttributeValue(attributeName, "").Trim();

            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                continue;
            }

            var texValues = string.Join(" ", node.ChildNodes
                .Where(n => n != singleNode && !string.IsNullOrWhiteSpace(n.InnerText))
                .Select(n => n.InnerText.Trim()));

            values.Add(attributeValue, texValues);
        }

        return values;
    }
}
