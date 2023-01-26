﻿using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using Imagibee.Gigantor;

namespace Testing {
    public class LineIndexerTests {
        readonly int chunkSize = 64 * 1024;
        readonly int maxWorkers = 1;
        readonly string simplePath = Path.Combine("Assets", "SimpleTest.txt");
        readonly string chunkPath = Path.Combine("Assets", "ChunkTest.txt");
        readonly string biblePath = Path.Combine("Assets", "BibleTest.txt");
        const string BIBLE_000001 = "The Project Gutenberg eBook of The King James Bible";
        const string BIBLE_001515 = "19:17 And it came to pass, when they had brought them forth abroad,";
        const string BIBLE_001516 = "that he said, Escape for thy life; look not behind thee, neither stay";
        const string BIBLE_002989 = "mother with the children.";
        const string BIBLE_002990 = "";
        const string BIBLE_002991 = "32:12 And thou saidst, I will surely do thee good, and make thy seed";
        const string BIBLE_100263 = "subscribe to our email newsletter to hear about new eBooks.";
        const string BIBLE_100264 = "";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InitialStateTest()
        {
            LineIndexer indexer = new(new AutoResetEvent(false), chunkSize, maxWorkers);
            Assert.AreEqual(false, indexer.Running);
            Assert.AreEqual(0, indexer.LineCount);
            Assert.AreEqual(true, indexer.LastError == "");
        }

        [Test]
        public void EmptyPathTest()
        {
            LineIndexer indexer = new(new AutoResetEvent(false), chunkSize, maxWorkers);
            indexer.Start("");
            indexer.Wait();
            Assert.AreEqual(true, indexer.LastError != "");
        }

        [Test]
        public void MissingPathTest()
        {
            LineIndexer indexer = new(new AutoResetEvent(false), chunkSize, maxWorkers);
            indexer.Start("A Missing File");
            indexer.Wait();
            Logger.Log($"error was {indexer.LastError}");
            Assert.AreEqual(true, indexer.LastError != "");
        }

        [Test]
        public void ChunkTest()
        {
            // 19 byte chunk size
            LineIndexer indexer = new(new AutoResetEvent(false), 19, maxWorkers);
            indexer.Start(chunkPath);
            indexer.Wait();
            //Assert.AreEqual(false, true);
            Logger.Log($"{indexer.LastError}");
            Assert.AreEqual(true, indexer.LastError == "");
            Assert.AreEqual(false, indexer.GetChunk(0).HasValue);
            var chunk = indexer.GetChunk(1).Value;
            Assert.AreEqual(0, chunk.StartFpos);
            Assert.AreEqual(1, chunk.StartLine);
            Assert.AreEqual(2, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(2).Value;
            Assert.AreEqual(0, chunk.StartFpos);
            Assert.AreEqual(1, chunk.StartLine);
            Assert.AreEqual(2, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(3).Value;
            Assert.AreEqual(22, chunk.StartFpos);
            Assert.AreEqual(3, chunk.StartLine);
            Assert.AreEqual(4, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(4).Value;
            Assert.AreEqual(22, chunk.StartFpos);
            Assert.AreEqual(3, chunk.StartLine);
            Assert.AreEqual(4, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(5).Value;
            Assert.AreEqual(44, chunk.StartFpos);
            Assert.AreEqual(5, chunk.StartLine);
            Assert.AreEqual(6, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(6).Value;
            Assert.AreEqual(44, chunk.StartFpos);
            Assert.AreEqual(5, chunk.StartLine);
            Assert.AreEqual(6, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(7).Value;
            Assert.AreEqual(66, chunk.StartFpos);
            Assert.AreEqual(7, chunk.StartLine);
            Assert.AreEqual(7, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(8).Value;
            Assert.AreEqual(77, chunk.StartFpos);
            Assert.AreEqual(8, chunk.StartLine);
            Assert.AreEqual(9, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(9).Value;
            Assert.AreEqual(77, chunk.StartFpos);
            Assert.AreEqual(8, chunk.StartLine);
            Assert.AreEqual(9, chunk.EndLine);
            Assert.AreEqual(19, chunk.ByteCount);
            chunk = indexer.GetChunk(10).Value;
            Assert.AreEqual(99, chunk.StartFpos);
            Assert.AreEqual(10, chunk.StartLine);
            Assert.AreEqual(10, chunk.EndLine);
            Assert.AreEqual(15, chunk.ByteCount);
        }

        [Test]
        public void FilePositionTest()
        {
            LineIndexer indexer = new(new AutoResetEvent(false), chunkSize, maxWorkers);
            indexer.Start(biblePath);
            indexer.Wait();
            Assert.AreEqual(true, indexer.LastError == "");
            Assert.AreEqual(-1, indexer.PositionFromLine(0));
            Assert.AreEqual(0, indexer.PositionFromLine(1));
            using var fileStream = new FileStream(biblePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fpos = indexer.PositionFromLine(1);
            fileStream.Seek(fpos, SeekOrigin.Begin);
            var line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_000001, line);
            fpos = indexer.PositionFromLine(1515);
            fileStream.Seek(fpos, SeekOrigin.Begin);
            line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_001515, line);
            fpos = indexer.PositionFromLine(1516);
            fileStream.Seek(fpos, SeekOrigin.Begin);
            line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_001516, line);
            fpos = indexer.PositionFromLine(2989);
            fileStream.Seek(fpos, SeekOrigin.Begin);
            line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_002989, line);
            fpos = indexer.PositionFromLine(2990);
            fileStream.Seek(fpos, SeekOrigin.Begin);
            line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_002990, line);
            fpos = indexer.PositionFromLine(2991);
            fileStream.Seek(fpos, SeekOrigin.Begin);
            line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_002991, line);
            fpos = indexer.PositionFromLine(100263);
            fileStream.Seek(fpos, SeekOrigin.Begin);
            line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_100263, line);
            fpos = indexer.PositionFromLine(100264);
            fileStream.Seek(fpos, SeekOrigin.Begin);
            line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_100264, line);
        }

        [Test]
        public void LineNumberTest()
        {
            LineIndexer indexer = new(new AutoResetEvent(false), chunkSize, maxWorkers);
            indexer.Start(biblePath);
            indexer.Wait();
            Logger.Log($"error was {indexer.LastError}");
            Assert.AreEqual(true, indexer.LastError == "");
        }

        [Test]
        public void SimpleTest()
        {
            LineIndexer indexer = new(new AutoResetEvent(false), chunkSize, maxWorkers);
            indexer.Start(simplePath);
            indexer.Wait();
            Assert.AreEqual(true, indexer.LastError == "");
            Assert.AreEqual(6, indexer.LineCount);
            var index = indexer.GetChunk(0);
            Assert.AreEqual(false, index.HasValue);
            for (var i = 1; i <= indexer.LineCount; i++) {
                index = indexer.GetChunk(i);
                Assert.AreEqual(true, index.HasValue);
                Assert.AreEqual(simplePath, index.Value.Path);
                Assert.AreEqual(1, index.Value.StartLine);
                Assert.AreEqual(6, index.Value.EndLine);
                Assert.AreEqual(0, index.Value.StartFpos);
            }
        }

        [Test]
        public void BibleTest()
        {
            AutoResetEvent progress = new(false);
            LineIndexer indexer = new(progress, chunkSize, 0);
            indexer.Start(biblePath);
            LineIndexer.Wait(
                new List<LineIndexer>() { indexer },
                progress,
                (_) => {},
                1000);
            Assert.AreEqual(true, indexer.LastError == "");
            Assert.AreEqual(100264, indexer.LineCount);
            using var fileStream = new FileStream(biblePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var chunk = indexer.GetChunk(1);
            Assert.AreEqual(true, chunk.HasValue);
            fileStream.Seek(chunk.Value.StartFpos, SeekOrigin.Begin);
            var line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_000001, line);
            Assert.AreEqual(chunk.Value.StartFpos, indexer.GetChunk(chunk.Value.EndLine).Value.StartFpos);
            chunk = indexer.GetChunk(1516);
            Assert.AreEqual(true, chunk.HasValue);
            fileStream.Seek(chunk.Value.StartFpos, SeekOrigin.Begin);
            line = new StreamReader(fileStream).ReadLine();
            Assert.AreEqual(BIBLE_001516, line);
            Assert.AreEqual(chunk.Value.StartFpos, indexer.GetChunk(chunk.Value.EndLine).Value.StartFpos);
        }
    }
}

