using NUnit.Framework;
using System.Diagnostics;

namespace FixBadOpcZip.Tests
{
    public class Class1
    {
        [Test]
        [TestCase("valid.oxps", false)]
        [TestCase("valid.xps", false)]
        [TestCase("BadOpcXps.xps", true)]
        public void FixBadOpcZipHelperTest(string sourceFile, bool needToFix)
        {
            var sourcePath = Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                "..",
                "..",
                "..",
                "Samples",
                sourceFile
            );

            var helper = new FixBadOpcZipHelper();
            Assert.That(helper.DoesNeedToFixZipFile(sourcePath), Is.EqualTo(needToFix));
            helper.RebuildZipFile(sourcePath, $"Fixed-{sourceFile}");
        }

        //[Test]
        public void FixOnDemand()
        {
            var helper = new FixBadOpcZipHelper();
            helper.RebuildZipFile(@"V:\PDFsharp\PDFsharp\testing\PdfSharp.Xps.UnitTests\Render2\xps\ImageBrushTileMode.xps", "Fixed.xps");
            File.Copy("Fixed.xps", @"V:\PDFsharp\PDFsharp\testing\PdfSharp.Xps.UnitTests\Render2\xps\ImageBrushTileMode.xps", true);
        }
    }
}
