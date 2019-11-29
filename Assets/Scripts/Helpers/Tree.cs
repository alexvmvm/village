using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Helpers.Tree
{
    public class Node<T> where T : class
    {
        public T Value { get; private set; }
        public T Parent { get; private set; }
        public T[] Children { get; private set; }

        public Node(T value, T parent, T[] children)
        {
            this.Value = value;
            this.Parent = parent;
            this.Children = children;
        }
    }

    public class Tree<T> where T : class
    {
        private List<Node<T>> _nodes;
        private List<Node<T>> _path;

        public Tree()
        {
            _path = new List<Node<T>>();
        }

        public void Add(Node<T> node)
        {
            if(!_nodes.Contains(node))
            {
                _nodes.Add(node);
            }
        }     

        public void Remove(Node<T> node)
        {
            if(_nodes.Contains(node))
            {
                _nodes.Remove(node);
            }
        }

        public Node<T> GetNodeByValue(T value)
        {
            return _nodes.FirstOrDefault(n => n.Value == value);
        }

        public Node<T>[] GetRoots()
        {
            return _nodes
                .Where(n => n.Parent == null)
                .ToArray();
        }

        public Node<T>[] GetLeafs()
        {
            return _nodes
                .Where(n => n.Children == null || n.Children.Length == 0)
                .ToArray();
        }

        public Node<T>[] GetPathToRoot(Node<T> node)
        {
            _path.Clear();
            _path.Add(node);
            
            var parent = node.Parent;
            while(parent != null)
            {
                _path.Add(GetNodeByValue(parent));
                parent = _path.Last().Parent;
            }

            return _path.ToArray();
        }

        public void Clear()
        {
            _nodes.Clear();
        }
    }
}