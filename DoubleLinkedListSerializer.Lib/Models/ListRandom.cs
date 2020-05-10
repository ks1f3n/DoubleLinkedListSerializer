using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoubleLinkedListSerializer.Lib.Models
{
    public partial class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(Stream s)
        {
            ListNode node = Head;
            if (node != null)
            {
                var i = 0;
                var sb = new StringBuilder();
                while (i < Count)
                {
                    sb.Append('{');
                    sb.Append(node.Data + ',');
                    sb.Append(node.RandomIndex.ToString() + '}');
                    if (i < Count - 1)
                        sb.Append(' ');
                    node = node.Next;
                    i++;
                }

                byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
                s.Write(buffer, 0, buffer.Length);
            }
        }

        public void Deserialize(Stream s)
        {
            string text;
            var listRandom = new List<(string collection, int indexes)>();

            byte[] buffer = new byte[s.Length];
            s.Seek(0, SeekOrigin.Begin);
            s.Read(buffer, 0, buffer.Length);
            text = Encoding.UTF8.GetString(buffer);

            string pattern = @"(?<={)\w+";
            foreach (Match match in Regex.Matches(text, pattern, RegexOptions.IgnoreCase))
                listRandom.Add((match.Value, 0));

            pattern = @"\d+(?=})";
            var i = 0;
            foreach (Match match in Regex.Matches(text, pattern, RegexOptions.IgnoreCase))
            {
                var idx = int.Parse(match.Value);
                var tuple = listRandom[i];
                listRandom[i] = (tuple.collection, idx);
                i++;
            }

            ClearList();
            CreateList(listRandom);
        }
    }

    public partial class ListRandom
    {
        public ListRandom(List<(string collection, int indexes)> value)
        {
            CreateList(value);
        }

        public void CreateList(List<(string collection, int indexes)> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var collection = value.Select(v => v.collection);
            var randomNodeIndexes = value.Select(v => v.indexes).ToArray();

            foreach (var item in collection)
            {
                AddLast(item);
            }

            Tail = Head?.Previous;

            AddRandomNodes(randomNodeIndexes);
        }

        public void ClearList()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        public ListNode AddLast(string data)
        {
            ListNode result = new ListNode(data);
            if (Head == null)
            {
                InternalInsertNodeToEmptyList(result);
            }
            else
            {
                InternalInsertNodeBefore(Head, result);
            }
            return result;
        }

        public ListNode GetElementAt(int idx)
        {
            ListNode node = Head;
            if (idx < Count / 2)
            {
                if (node != null)
                {
                    var i = 0;
                    while (i < idx)
                    {
                        node = node.Next;
                        i++;
                    }
                }
            }
            else if (idx < Count)
            {
                node = Tail;
                if (node != null)
                {
                    var i = Count - 1;
                    while (i > idx)
                    {
                        node = node.Previous;
                        i--;
                    }
                }
            }

            return node;
        }

        public override string ToString()
        {
            ListNode node = Head;
            StringBuilder sb = new StringBuilder();
            sb.Append(node.Data + $" ({node.Random.Data})");
            while (node != Tail)
            {
                node = node.Next;
                sb.Append(", " + node.Data + $" ({node.Random.Data})");
            }

            return sb.ToString();
        }

        private void InternalInsertNodeBefore(ListNode node, ListNode newNode)
        {
            newNode.Next = node;
            newNode.Previous = node.Previous;
            node.Previous.Next = newNode;
            node.Previous = newNode;
            Count++;
        }

        private void InternalInsertNodeToEmptyList(ListNode newNode)
        {
            newNode.Next = newNode;
            newNode.Previous = newNode;
            Head = newNode;
            Count++;
        }

        private void AddRandomNodes(IReadOnlyList<int> randomNodeIndexes)
        {
            ListNode node = Head;
            if (node != null)
            {
                var i = 0;
                while (i < Count)
                {
                    var idx = randomNodeIndexes[i];
                    node.Random = this.GetElementAt(idx);
                    node.RandomIndex = idx;
                    node = node.Next;
                    i++;
                }
            }
        }
    }
}
