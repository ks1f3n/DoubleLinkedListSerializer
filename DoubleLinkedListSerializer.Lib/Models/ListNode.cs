using System;

namespace DoubleLinkedListSerializer.Lib.Models
{
    public partial class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // произвольный элемент внутри списка
        public string Data;
    }

    public partial class ListNode
    {
        public int RandomIndex;
        public ListNode(string data)
        {
            this.Data = data.Trim('{','}');
        }
    }
}