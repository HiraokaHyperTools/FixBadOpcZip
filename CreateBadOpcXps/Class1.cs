using NUnit.Framework;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Windows;
using System.Threading;
using System.IO.Packaging;

namespace CreateBadOpcXps
{
    public class Class1
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        [Ignore("TDD")]
        public void MakeBadOpcXpsXps()
        {
            // This seems to set `System.IO.Packaging.Package.InStreamingCreation` to true.

            using (XpsDocument xpsDocument = new XpsDocument("BadOpcXps.xps", FileAccess.Write))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                FixedDocument fixedDoc = new FixedDocument();
                PageContent pageContent = new PageContent();
                fixedDoc.Pages.Add(pageContent);

                FixedPage fixedPage = new FixedPage();
                pageContent.Child = fixedPage;

                writer.Write(fixedDoc);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [Ignore("TDD")]
        public void MakeValidOpcXpsXps()
        {
            using (Package package = Package.Open("ValidOpcXps.xps", FileMode.CreateNew))
            using (XpsDocument xpsDocument = new XpsDocument(package))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                FixedDocument fixedDoc = new FixedDocument();
                PageContent pageContent = new PageContent();
                fixedDoc.Pages.Add(pageContent);

                FixedPage fixedPage = new FixedPage();
                pageContent.Child = fixedPage;

                writer.Write(fixedDoc);
            }
        }
    }
}
