using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.Xml.Linq;
using System.Xml;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using DocumentFormat.OpenXml.Validation;

namespace OpenWordBookMarks
{
    class Program
    {
        static void Main(string[] args)
        {
            string docName = @"d:\experiencia.docx";

            using (WordprocessingDocument doc = WordprocessingDocument.Open(docName, true))
            {
                // Add a new main document part. 

                var styles = doc.MainDocumentPart.StyleDefinitionsPart;

                var bookMarks = FindBookmarks(doc.MainDocumentPart.Document);

                foreach (var end in bookMarks)
                {
                    var textElement = new Text("asdfasdf");
                    var runElement = new Run(textElement);

                    end.Value.InsertAfterSelf(runElement);
                }

                // Create the Document DOM. 
                //doc.MainDocumentPart.Document.Body.Last().InsertAfterSelf(
                //      new Paragraph(
                //        new Run(
                //          new Text("Hello World!"))));

                // Save changes to the main document part. 
                doc.MainDocumentPart.Document.Save();
            }

            // DocumentFormat.OpenXml.OpenXmlReader reader = DocumentFormat.OpenXml.OpenXmlReader.Create(File.OpenRead(@""));


        }

        // Insert a table into a word processing document.
        public static void CreateTable(string fileName)
        {
            // Use the file name and path passed in as an argument 
            // to open an existing Word 2007 document.

            using (WordprocessingDocument doc
                = WordprocessingDocument.Open(fileName, true))
            {
                // Create an empty table.
                Table table = new Table();

                // Create a TableProperties object and specify its border information.
                TableProperties tblProp = new TableProperties(
                    new TableBorders(
                        new TopBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Dashed),
                            Size = 24
                        },
                        new BottomBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Dashed),
                            Size = 24
                        },
                        new LeftBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Dashed),
                            Size = 24
                        },
                        new RightBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Dashed),
                            Size = 24
                        },
                        new InsideHorizontalBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Dashed),
                            Size = 24
                        },
                        new InsideVerticalBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.Dashed),
                            Size = 24
                        }
                    )
                );

                // Append the TableProperties object to the empty table.
                table.AppendChild<TableProperties>(tblProp);

                // Create a row.
                TableRow tr = new TableRow();

                // Create a cell.
                TableCell tc1 = new TableCell();

                // Specify the width property of the table cell.
                tc1.Append(new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }));

                // Specify the table cell content.
                tc1.Append(new Paragraph(new Run(new Text("some text"))));

                // Append the table cell to the table row.
                tr.Append(tc1);

                // Create a second table cell by copying the OuterXml value of the first table cell.
                TableCell tc2 = new TableCell(tc1.OuterXml);

                // Append the table cell to the table row.
                tr.Append(tc2);

                // Append the table row to the table.
                table.Append(tr);

                // Append the table to the document.
                doc.MainDocumentPart.Document.Body.Append(table);
            }
        }


        // How to add an image part to a package.
        public static void AddImagePart(WordprocessingDocument wordDoc, string fileName)
        {
            MainDocumentPart mainPart = wordDoc.MainDocumentPart;

            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);

            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }
        }


        public static void SetRunFont(Run run)
        {
            // Use an object initializer for RunProperties and rPr.
            RunProperties rPr = new RunProperties(
                new RunFonts()
                {
                    Ascii = "Arial"
                });

            run.PrependChild<RunProperties>(rPr);
        }

        private static Dictionary<string, BookmarkEnd> FindBookmarks(OpenXmlElement documentPart, Dictionary<string, BookmarkEnd> results = null, Dictionary<string, string> unmatched = null)
        {
            results = results ?? new Dictionary<string, BookmarkEnd>();
            unmatched = unmatched ?? new Dictionary<string, string>();

            foreach (var child in documentPart.Elements())
            {
                if (child is BookmarkStart)
                {
                    var bStart = child as BookmarkStart;
                    unmatched.Add(bStart.Id, bStart.Name);
                }

                if (child is BookmarkEnd)
                {
                    var bEnd = child as BookmarkEnd;
                    foreach (var orphanName in unmatched)
                    {
                        if (bEnd.Id == orphanName.Key)
                            results.Add(orphanName.Value, bEnd);
                    }
                }

                FindBookmarks(child, results, unmatched);
            }

            return results;
        }

        public static void ReplaceBookmarkParagraphs(WordprocessingDocument doc, string bookmark, string text)
        {
            //Find all Paragraph with 'BookmarkStart' 
            var t = (from el in doc.MainDocumentPart.RootElement.Descendants<BookmarkStart>()
                     where (el.Name == bookmark) &&
                     (el.NextSibling<Run>() != null)
                     select el).First();
            //Take ID value
            var val = t.Id.Value;
            //Find the next sibling 'text'
            OpenXmlElement next = t.NextSibling<Run>();
            //Set text value
            next.GetFirstChild<Text>().Text = text;

            //Delete all bookmarkEnd node, until the same ID
            deleteElement(next.GetFirstChild<Text>().Parent, next.GetFirstChild<Text>().NextSibling(), val, true);
        }

        public static bool deleteElement(OpenXmlElement parentElement, OpenXmlElement elem, string id, bool seekParent)
        {
            bool found = false;

            //Loop until I find BookmarkEnd or null element
            while (!found && elem != null && (!(elem is BookmarkEnd) || (((BookmarkEnd)elem).Id.Value != id)))
            {
                if (elem.ChildElements != null && elem.ChildElements.Count > 0)
                {
                    found = deleteElement(elem, elem.FirstChild, id, false);
                }

                if (!found)
                {
                    OpenXmlElement nextElem = elem.NextSibling();
                    elem.Remove();
                    elem = nextElem;
                }
            }

            if (!found)
            {
                if (elem == null)
                {
                    if (!(parentElement is Body) && seekParent)
                    {
                        //Try to find bookmarkEnd in Sibling nodes
                        found = deleteElement(parentElement.Parent, parentElement.NextSibling(), id, true);
                    }
                }
                else
                {
                    if (elem is BookmarkEnd && ((BookmarkEnd)elem).Id.Value == id)
                    {
                        found = true;
                    }
                }
            }

            return found;
        }

        #region Style Manipulation

        // Extract the styles or stylesWithEffects part from a 
        // word processing document as an XDocument instance.
        public static XDocument ExtractStylesPart(
          string fileName,
          bool getStylesWithEffectsPart = true)
        {
            // Declare a variable to hold the XDocument.
            XDocument styles = null;

            // Open the document for read access and get a reference.
            using (var document = WordprocessingDocument.Open(fileName, false))
            {
                // Get a reference to the main document part.
                var docPart = document.MainDocumentPart;

                // Assign a reference to the appropriate part to the
                // stylesPart variable.
                StylesPart stylesPart = null;
                if (getStylesWithEffectsPart)
                    stylesPart = docPart.StylesWithEffectsPart;
                else
                    stylesPart = docPart.StyleDefinitionsPart;

                // If the part exists, read it into the XDocument.
                if (stylesPart != null)
                {
                    using (var reader = XmlNodeReader.Create(stylesPart.GetStream(FileMode.Open, FileAccess.Read)))
                    {
                        // Create the XDocument.
                        styles = XDocument.Load(reader);
                    }
                }
            }
            // Return the XDocument instance.
            return styles;
        }


        

        // Create a new paragraph style with the specified style ID, primary style name, and aliases and 
        // add it to the specified style definitions part.
        public static void CreateAndAddParagraphStyle(StyleDefinitionsPart styleDefinitionsPart,
            string styleid, string stylename, string aliases = "")
        {
            // Access the root element of the styles part.
            Styles styles = styleDefinitionsPart.Styles;
            if (styles == null)
            {
                styleDefinitionsPart.Styles = new Styles();
                styleDefinitionsPart.Styles.Save();
            }

            // Create a new paragraph style element and specify some of the attributes.
            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleid,
                CustomStyle = true,
                Default = false
            };

            // Create and add the child elements (properties of the style).
            Aliases aliases1 = new Aliases() { Val = aliases };
            AutoRedefine autoredefine1 = new AutoRedefine() { Val = OnOffOnlyValues.Off };
            BasedOn basedon1 = new BasedOn() { Val = "Normal" };
            LinkedStyle linkedStyle1 = new LinkedStyle() { Val = "OverdueAmountChar" };
            Locked locked1 = new Locked() { Val = OnOffOnlyValues.Off };
            PrimaryStyle primarystyle1 = new PrimaryStyle() { Val = OnOffOnlyValues.On };
            StyleHidden stylehidden1 = new StyleHidden() { Val = OnOffOnlyValues.Off };
            SemiHidden semihidden1 = new SemiHidden() { Val = OnOffOnlyValues.Off };
            StyleName styleName1 = new StyleName() { Val = stylename };
            NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() { Val = "Normal" };
            UIPriority uipriority1 = new UIPriority() { Val = 1 };
            UnhideWhenUsed unhidewhenused1 = new UnhideWhenUsed() { Val = OnOffOnlyValues.On };
            if (aliases != "")
                style.Append(aliases1);
            style.Append(autoredefine1);
            style.Append(basedon1);
            style.Append(linkedStyle1);
            style.Append(locked1);
            style.Append(primarystyle1);
            style.Append(stylehidden1);
            style.Append(semihidden1);
            style.Append(styleName1);
            style.Append(nextParagraphStyle1);
            style.Append(uipriority1);
            style.Append(unhidewhenused1);

            // Create the StyleRunProperties object and specify some of the run properties.
            StyleRunProperties styleRunProperties1 = new StyleRunProperties();
            Bold bold1 = new Bold();
            Color color1 = new Color() { ThemeColor = ThemeColorValues.Accent2 };
            RunFonts font1 = new RunFonts() { Ascii = "Lucida Console" };
            Italic italic1 = new Italic();
            // Specify a 12 point size.
            FontSize fontSize1 = new FontSize() { Val = "24" };
            styleRunProperties1.Append(bold1);
            styleRunProperties1.Append(color1);
            styleRunProperties1.Append(font1);
            styleRunProperties1.Append(fontSize1);
            styleRunProperties1.Append(italic1);

            // Add the run properties to the style.
            style.Append(styleRunProperties1);

            // Add the style to the styles part.
            styles.Append(style);
        }

        // Add a StylesDefinitionsPart to the document.  Returns a reference to it.
        public static StyleDefinitionsPart AddStylesPartToPackage(WordprocessingDocument doc)
        {
            StyleDefinitionsPart part;
            part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
            Styles root = new Styles();
            root.Save(part);
            return part;
        }

        #endregion

        #region Images

        public static void InsertAPicture(string document, string fileName)
        {
            using (WordprocessingDocument wordprocessingDocument =
                WordprocessingDocument.Open(document, true))
            {
                MainDocumentPart mainPart = wordprocessingDocument.MainDocumentPart;

                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                AddImageToBody(wordprocessingDocument, mainPart.GetIdOfPart(imagePart));
            }
        }

        private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }

        #endregion

        #region Validate Word Document

        public static void ValidateWordDocument(string filepath)
        {
            using (WordprocessingDocument wordprocessingDocument =
            WordprocessingDocument.Open(filepath, true))
            {
                try
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    int count = 0;
                    foreach (ValidationErrorInfo error in
                        validator.Validate(wordprocessingDocument))
                    {
                        count++;
                        Console.WriteLine("Error " + count);
                        Console.WriteLine("Description: " + error.Description);
                        Console.WriteLine("ErrorType: " + error.ErrorType);
                        Console.WriteLine("Node: " + error.Node);
                        Console.WriteLine("Path: " + error.Path.XPath);
                        Console.WriteLine("Part: " + error.Part.Uri);
                        Console.WriteLine("-------------------------------------------");
                    }

                    Console.WriteLine("count={0}", count);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                wordprocessingDocument.Close();
            }
        }

        public static void ValidateCorruptedWordDocument(string filepath)
        {
            // Insert some text into the body, this would cause Schema Error
            using (WordprocessingDocument wordprocessingDocument =
            WordprocessingDocument.Open(filepath, true))
            {
                // Insert some text into the body, this would cause Schema Error
                Body body = wordprocessingDocument.MainDocumentPart.Document.Body;
                Run run = new Run(new Text("some text"));
                body.Append(run);

                try
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    int count = 0;
                    foreach (ValidationErrorInfo error in
                        validator.Validate(wordprocessingDocument))
                    {
                        count++;
                        Console.WriteLine("Error " + count);
                        Console.WriteLine("Description: " + error.Description);
                        Console.WriteLine("ErrorType: " + error.ErrorType);
                        Console.WriteLine("Node: " + error.Node);
                        Console.WriteLine("Path: " + error.Path.XPath);
                        Console.WriteLine("Part: " + error.Part.Uri);
                        Console.WriteLine("-------------------------------------------");
                    }

                    Console.WriteLine("count={0}", count);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        #endregion

    }
}
