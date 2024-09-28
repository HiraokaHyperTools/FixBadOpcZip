# HiraokaHyperTools.FixBadOpcZip

[![NuGet Version](https://img.shields.io/nuget/v/HiraokaHyperTools.FixBadOpcZip)](https://www.nuget.org/packages/HiraokaHyperTools.FixBadOpcZip)

Fix the invalid OPC file (xps file) created with `System.IO.Packaging.Package.InStreamingCreation` option enabled.

## Usage

```cs
using FixBadOpcZip;

var helper = new FixBadOpcZipHelper();
if (helper.DoesNeedToFixZipFile("input.xps"))
{
  helper.RebuildZipFile("input.xps", "fixed-input.xps");
}
```
