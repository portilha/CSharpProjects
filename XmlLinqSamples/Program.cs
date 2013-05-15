using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace XmlLinqSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            string document = @"C:\Users\2754\Desktop\Tutorial\Tutorial Project\SCADA Server\Output\Mimics\09GradientsAndTransparencies.svg";

            FormatWithXDocument(document);
            FormatWithXmlDocument(document);

            CreateXmlWithXDocument();

            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "XmlLinqSamples.csproj");

            XDocument doc = XDocument.Load(path);

            // XDocument doc = XDocument.Load(@"xmlTest.xml");

            foreach (var item in doc.Descendants(XName.Get("Reference", doc.Root.GetDefaultNamespace().ToString())))
            {
                // Console.WriteLine(item.ToString());
            }

            doc.Descendants().Where<XElement>(e => e.Name.LocalName == "" && e.HasElements);

            XElement.Parse(File.ReadAllText(path));

            foreach (var item in doc.Descendants(XName.Get("PostBuildEvent", doc.Root.GetDefaultNamespace().ToString())))
            {
                if (item.HasElements) // Check if the node has children nodes.
                {
                    Console.WriteLine("Child Elements:");

                    foreach (var childElements in item.Elements())
                    {
                        Console.WriteLine(childElements.ToString());
                    }
                }

                if (item.HasAttributes) // Check if the node has attributes
                {
                    Console.WriteLine("Attributes");

                    foreach (var att in item.Attributes())
                    {
                        Console.WriteLine(string.Format("{0} = {1}", att.Name, att.Value));
                    }
                }

                if (!string.IsNullOrWhiteSpace(item.Value)) // Check if the node has content
                {
                    Console.WriteLine("Content: " + item.Value);
                }
            }


            // Get a set of elements from a given type.

            Console.ReadKey();

        }

        private static void FormatWithXDocument(string document)
        {
            string space = "xml:space=\"preserve\"";
            string xml = File.ReadAllText(document);
            xml = xml.Remove(xml.IndexOf(space), space.Length);

            XDocument d = XDocument.Parse(xml);
            //// d.Declaration = new XDeclaration("1.0", "utf-8", null);

            d.Root.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));

            StringBuilder st = new StringBuilder();
            using (StringWriter sw = new StringWriter(st))
            {
                using (XmlTextWriter xml22 = new XmlTextWriter(sw))
                {
                    xml22.Formatting = Formatting.Indented;
                    xml22.Indentation = 0;
                    xml22.IndentChar = '\n';

                    d.Save(xml22);
                }
            }

            string re = st.ToString();

        }

        private static void CreateSimpleElementWithNamespace()
        {
            XElement element = new XElement(XName.Get("SpecificVersion", "http://schemas.microsoft.com/developer/msbuild/2003"), true);
            element.ToString();

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                element.Save(sw);
            }

            Console.WriteLine(sb.ToString());
        }

        private static void ShowAllComments(XDocument doc)
        {
            Console.WriteLine("Comments:");
            foreach (var item in doc.DescendantNodes().OfType<XComment>())
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine("End of Comments");
        }


        private static void FormatWithXmlDocument(string filePath)
        {
            XmlDocument document = new XmlDocument();
            document.PreserveWhitespace = false;
            document.Load(filePath);
            document.PreserveWhitespace = false;

            using (XmlTextWriter xmw = new XmlTextWriter(File.OpenWrite(@"d:\XmlDoc.xml"), Encoding.UTF8))
            {
                xmw.Formatting = Formatting.Indented;

                using (XmlReader xmlr = XmlReader.Create(File.OpenRead(filePath)))
                {
                    while (xmlr.Read())
                    {
                        xmw.WriteNode(xmlr, false);
                    }
                }
            }
        }




        /// <summary>
        /// Builds a sample Xml.
        /// </summary>
        private static void CreateXmlWithXDocument()
        {
            XNamespace Xml = "xml";

            XElement element = new XElement("myElememnt");
            element.Add(new XAttribute("asdf2", string.Empty), new XAttribute("asdf", 2));

            // element.AddBeforeSelf(new XElement("Tags", new XAttribute("My", true)));
            StringBuilder sb = new StringBuilder();
            element.Save(new StringWriter(sb));


            string result = sb.ToString();

            XDocument doc = new XDocument(
                       new XDeclaration("1.0", "utf-8", "yes"),

                       new XComment("A sample xml file"),
                       new XElement("employees", new XAttribute(XNamespace.Xml + "space", "preserve"),
                       new XElement("employee1",
                          new XAttribute("id", 1),
                          new XAttribute("salaried", "false"),
                             new XElement("name", "Gustavo Achong"),
                             new XElement("hire_date", "7/31/1996")),
                       new XElement("employee",
                          new XAttribute("id", 3),
                          new XAttribute("salaried", "true"),
                             new XElement("name", "Kim Abercrombie"),
                             new XElement("hire_date", "12/12/1997"))
                         ));


            StringBuilder sbx = new StringBuilder();
            using (StringWriter ms = new StringWriter(sbx))
            {
                using (XmlTextWriter xmlWritter = new XmlTextWriter(ms))
                {
                    xmlWritter.Formatting = Formatting.Indented;
                    xmlWritter.Indentation = 0;
                    xmlWritter.IndentChar = '\n';

                    doc.Save(xmlWritter);
                    xmlWritter.Flush();
                }

            }

            byte[] values = null;

            using (MemoryStream sr = new MemoryStream())
            {
                using (XmlWriter xml = XmlWriter.Create(sr, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8, IndentChars = "\n" }))
                {
                    doc.Save(xml);
                    xml.Flush();

                    sr.Position = 0;

                    XDocument docAppend = XDocument.Load(sr, LoadOptions.None);

                    foreach (var item in docAppend.Descendants(XName.Get("employee1", doc.Root.GetDefaultNamespace().ToString())))
                    {
                        item.Add(new XElement("MyGradient", new XAttribute("att", 1), new XElement("SubElement")));
                        item.Add(new XElement("MyGradient", new XAttribute("att", 1), new XElement("SubElement")));
                        item.Add(new XElement("MyGradient", new XAttribute("att", 1), new XElement("SubElement")));
                    }

                    sr.Position = 0;


                    StringBuilder stb = new StringBuilder();

                    using (StringWriter sw = new StringWriter(stb))
                    {
                        using (XmlWriter xml22 = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8 }))
                        {

                            docAppend.Save(xml22);
                        }
                    }

                    string result2 = stb.ToString();
                }
            }




            Debug.WriteLine(Directory.GetCurrentDirectory());

            doc.Save(@"xmlTest.xml");
        }
    }
}
