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

            CreateXmlWithXDocument();

            XDocument doc = XDocument.Load(@"xmlTest.xml");

            foreach (var item in doc.DescendantNodes())
            {
                Console.WriteLine(item.ToString());
            }

            // Get a set of elements from a given type.
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
