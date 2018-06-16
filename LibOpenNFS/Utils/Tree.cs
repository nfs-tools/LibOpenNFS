using System;
using System.Collections.Generic;

namespace LibOpenNFS.Utils
{
    /// <summary>
    /// A simple nested tree structure.
    /// </summary>
    public class Tree<T>
    {
        public List<TreeItem> Items;

        public Tree()
        {
            Items = new List<TreeItem>();
        }

        public void Add(T item)
        {
            if (!Items.Exists(i => EqualityComparer<T>.Default.Equals(item, i.Item)))
            {
                Items.Add(new TreeItem(item));
            }
        }

        /// <summary>
        /// Debug utility to print all items and their children.
        /// </summary>
        public void PrintItems(int indentLevel = 0)
        {
            Print(Items, indentLevel);
        }

        private void Print(IEnumerable<TreeItem> items, int level)
        {
            var indent = new string('\t', level);
            
            foreach (var item in items)
            {
                Console.WriteLine($"{indent} {item.Item}");

                if (item.SubItems.Count > 0)
                {
                    Print(item.SubItems, level + 1);
                }
            }
        }
        
        public class TreeItem
        {
            public T Item;
            public List<TreeItem> SubItems;

            public TreeItem(T item)
            {
                Item = item;
                SubItems = new List<TreeItem>();
            }

            public TreeItem AddSubItem(T item)
            {
                if (!SubItems.Exists(i => EqualityComparer<T>.Default.Equals(item, i.Item)))
                {
                    var newItem = new TreeItem(item);
                    SubItems.Add(newItem);

                    return newItem;
                }

                return SubItems.Find(i => EqualityComparer<T>.Default.Equals(item, i.Item));
            }
        }
    }
}