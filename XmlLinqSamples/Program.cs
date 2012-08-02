using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;

namespace XmlLinqSamples
{
    class Program
    {
        static void Main(string[] args)
        {
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
            foreach (var item in doc.DescendantNodes())
            {


                if (item is XComment)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            Console.WriteLine("End of Comments");
        }

        /// <summary>
        /// Builds a sample Xml.
        /// </summary>
        private static void CreateXmlWithXDocument()
        {
            XDocument doc = new XDocument(
                       new XDeclaration("1.0", "utf-8", "yes"),
                       new XComment("A sample xml file"),
                       new XElement("employees",
                       new XElement("employee",
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

            Debug.WriteLine(Directory.GetCurrentDirectory());

            doc.Save(@"xmlTest.xml");
        }
    }
}
