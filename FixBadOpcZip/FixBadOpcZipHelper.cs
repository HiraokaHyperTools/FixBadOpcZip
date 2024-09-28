using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FixBadOpcZip
{
    public class FixBadOpcZipHelper
    {
        private readonly Regex _piece = new Regex("^(?<path>.+?)/\\[(?<number>\\d+)\\]\\.piece$");
        private readonly Regex _lastPiece = new Regex("^(?<path>.+?)/\\[(?<number>\\d+)\\]\\.last\\.piece$");

        private class IntermediatePiece
        {
            public MemoryStream Stream { get; set; } = new MemoryStream();
            public int LastNumber { get; set; } = 1;
            public DateTimeOffset LastWriteTime { get; set; }
            public string Path { get; set; }
        }

        public void RebuildZipFile(string sourceZipFile, string newZipFile)
        {
            if (File.Exists(newZipFile))
            {
                File.Delete(newZipFile);
            }

            using (var sourceZip = ZipFile.OpenRead(sourceZipFile))
            using (var newZip = ZipFile.Open(newZipFile, ZipArchiveMode.Create))
            {
                RebuildZip(sourceZip, newZip);
            }
        }

        public void RebuildZip(ZipArchive sourceZip, ZipArchive newZip)
        {
            // [Content_Types].xml/[0].piece
            // ...
            // [Content_Types].xml/[1].piece
            // ...
            // [Content_Types].xml/[2].piece
            // ...
            // [Content_Types].xml/[3].piece
            // ...
            // [Content_Types].xml/[4].piece
            // [Content_Types].xml/[5].last.piece

            var intermediatePieces = new Dictionary<string, IntermediatePiece>();

            foreach (var entry in sourceZip.Entries)
            {
                var matchPiece = _piece.Match(entry.FullName);
                if (matchPiece.Success)
                {
                    var number = int.Parse(matchPiece.Groups["number"].Value);
                    var path = matchPiece.Groups["path"].Value;
                    IntermediatePiece piece;
                    if (number == 0)
                    {
                        piece = new IntermediatePiece
                        {
                            Path = path,
                            LastNumber = number,
                            LastWriteTime = entry.LastWriteTime,
                        };
                        intermediatePieces[path] = piece;
                    }
                    else if (intermediatePieces.TryGetValue(path, out piece))
                    {
                        if (piece.LastNumber != number - 1)
                        {
                            throw new InvalidDataException();
                        }

                        piece.LastNumber = number;
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }

                    using (var sourceStream = entry.Open())
                    {
                        sourceStream.CopyTo(piece.Stream);
                    }
                }
                else
                {
                    var lastMatchPiece = _lastPiece.Match(entry.FullName);
                    if (lastMatchPiece.Success)
                    {
                        var number = int.Parse(lastMatchPiece.Groups["number"].Value);
                        var path = lastMatchPiece.Groups["path"].Value;
                        IntermediatePiece piece;
                        if (intermediatePieces.TryGetValue(path, out piece))
                        {
                            if (piece.LastNumber != number - 1)
                            {
                                throw new InvalidDataException();
                            }
                            else
                            {
                                var newEntry = newZip.CreateEntry(piece.Path);
                                newEntry.LastWriteTime = piece.LastWriteTime;
                                using (var newStream = newEntry.Open())
                                {
                                    piece.Stream.Position = 0;
                                    piece.Stream.CopyTo(newStream);
                                }

                                intermediatePieces.Remove(path);
                            }
                        }
                        else
                        {
                            throw new InvalidDataException();
                        }
                    }
                    else
                    {
                        var newEntry = newZip.CreateEntry(entry.FullName);
                        newEntry.LastWriteTime = entry.LastWriteTime;
                        using (var copyFromStream = entry.Open())
                        using (var copyToStream = newEntry.Open())
                        {
                            copyFromStream.CopyTo(copyToStream);
                        }
                    }
                }
            }
        }

        public bool DoesNeedToFixZipFile(string sourceZipFile)
        {
            using (var sourceZip = ZipFile.OpenRead(sourceZipFile))
            {
                return DoesNeedToFixZip(sourceZip);
            }
        }

        public bool DoesNeedToFixZip(ZipArchive sourceZip)
        {
            // [Content_Types].xml/[0].piece

            foreach (var entry in sourceZip.Entries)
            {
                var matchPiece = _piece.Match(entry.FullName);
                if (matchPiece.Success && matchPiece.Groups["number"].Value == "0")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
