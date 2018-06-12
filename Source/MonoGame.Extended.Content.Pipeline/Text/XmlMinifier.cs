using System.IO;
using System.Xml;

namespace MonoGame.Extended.Content.Pipeline.Text
{

    /// <summary>
    /// Config object for the XML minifier.
    /// </summary>
    public class XmlMinifierSettings
    {
        public bool RemoveEmptyLines { get; set; }
        public bool RemoveSpaceBetweenElements { get; set; }
        public bool CloseEmptyTags { get; set; }
        public bool RemoveComments { get; set; }

        public static XmlMinifierSettings Aggressive
        {
            get
            {
                return new XmlMinifierSettings
                {
                    RemoveEmptyLines = true,
                    RemoveSpaceBetweenElements = true,
                    CloseEmptyTags = true,
                    RemoveComments = true
                };
            }
        }

        public static XmlMinifierSettings NoMinification
        {
            get
            {
                return new XmlMinifierSettings
                {
                    RemoveEmptyLines = false,
                    RemoveSpaceBetweenElements = false,
                    CloseEmptyTags = false,
                    RemoveComments = false
                };
            }
        }
    }

    /// <summary>
    /// XML minifier. No Regex allowed! :-)
    /// </summary>
    public class XmlMinifier
    {
        private XmlMinifierSettings _settings;

        public XmlMinifier(XmlMinifierSettings minifierSettings)
        {
            _settings = minifierSettings;
        }

        public string Minify(Stream data)
        {
            var document = new XmlDocument
            {
                PreserveWhitespace = !(_settings.RemoveSpaceBetweenElements || _settings.RemoveEmptyLines)
            };
            document.Load(data);

            //remove comments first so we have less to compress later
            if (_settings.RemoveComments)
            {
                var commentNodes = document.SelectNodes("//comment()");
                for (int i = 0, length = commentNodes.Count; i < length; i++)
                {
                    var commentNode = commentNodes[i];
                    commentNode.ParentNode.RemoveChild(commentNode);
                }
            }

            if (_settings.CloseEmptyTags)
            {
                var emptyElements = document.SelectNodes("descendant::*[not(*) and not(normalize-space())]");
                for (int i = 0, length = emptyElements.Count; i < length; i++)
                {
                    var emptyElement = emptyElements[i] as XmlElement;
                    emptyElement.IsEmpty = true;
                }
            }

            if (_settings.RemoveSpaceBetweenElements)
            {
                return document.InnerXml;
            }
            else
            {
                using (var stringWriter = new StringWriter())
                {
                    document.Save(stringWriter);
                    return stringWriter.ToString();
                }
            }
        }
    }
}