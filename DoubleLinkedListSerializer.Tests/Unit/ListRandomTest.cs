using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using DoubleLinkedListSerializer.Lib.Models;
using FluentAssertions;
using Xunit;

namespace DoubleLinkedListSerializer.Tests.Unit
{
    public class ListRandomTest
    {
        [Fact]
        public void CreateOneNode_RandomNotNull()
        {
            var list = new ListRandom(new List<(string, int)> { ("test", 0) });
            list.Head.Random.Should().NotBeNull();
        }

        [Fact]
        public void GetSecondNodeInList_ReturnSecond()
        {
            var list = new ListRandom(new List<(string, int)> { ("first", 1), ("second", 0), ("third", 2) });
            list.GetElementAt(1).Data.Should().Be("second");
        }

        [Fact]
        public void GetSecondNodeInList_ReturnFirst()
        {
            var list = new ListRandom(new List<(string, int)> { ("first", 1), ("second", 3), ("third", 2) });
            list.GetElementAt(0).Data.Should().Be("first");
        }

        [Fact]
        public void Serialize_ReturnString()
        {
            string text;
            var list = new ListRandom(new List<(string, int)> { ("first", 1), ("second", 3), ("third", 2) });
            using (var memStream = new MemoryStream())
            {
                list.Serialize(memStream);
                byte[] buffer = new byte[memStream.Length];
                memStream.Seek(0, SeekOrigin.Begin);
                memStream.Read(buffer, 0, buffer.Length);
                text = Encoding.UTF8.GetString(buffer);
            }

            text.Should().Be("{first,1} {second,3} {third,2}");
        }

        [Fact]
        public void Deserialize_ReturnHead()
        {
            string text = "{first,1} {second,3} {third,2}";
            var list = new ListRandom(new List<(string, int)>());
            
            using (var memStream = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                memStream.Write(buffer, 0, buffer.Length);
                list.Deserialize(memStream);
            }
            
            list.Head.Data.Should().Be("first");
        }

        [Fact]
        public void Deserialize_ReturnTail()
        {
            string text = "{first,1} {second,3} {third,2}";
            var list = new ListRandom(new List<(string, int)>());

            using (var memStream = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                memStream.Write(buffer, 0, buffer.Length);
                list.Deserialize(memStream);
            }

            list.Tail.Data.Should().Be("third");
        }

        [Fact]
        public void Deserialize_ReturnTailRandom()
        {
            string text = "{first,1} {second,2} {third,1}";
            var list = new ListRandom(new List<(string, int)>());

            using (var memStream = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                memStream.Write(buffer, 0, buffer.Length);
                list.Deserialize(memStream);
            }

            list.Tail.Random.Data.Should().Be("second");
        }
    }
}
