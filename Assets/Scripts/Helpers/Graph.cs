using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Profiling;

namespace Helpers.Graph
{
    public class Node<T>
    {
        // Private member-variables
        private T data;
        private NodeList<T> neighbors = null;

        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> neighbors)
        {
            this.data = data;
            this.neighbors = neighbors;
        }

        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        protected NodeList<T> Neighbors
        {
            get
            {
                return neighbors;
            }
            set
            {
                neighbors = value;
            }
        }
    }

    public class NodeList<T> : Collection<Node<T>>
    {
        public NodeList() : base() { }

        public NodeList(int initialSize)
        {
            // Add the specified number of items
            for (int i = 0; i < initialSize; i++)
                base.Items.Add(default(Node<T>));
        }

        public Node<T> FindByValue(T value)
        {
            // search the list for the value
            foreach (Node<T> node in Items)
                if (node.Value.Equals(value))
                    return node;

            // if we reached here, we didn't find a matching node
            return null;
        }
    }

    public class GraphNode<T> : Node<T>
    {
        private List<int> costs;

        public GraphNode() : base() { }
        public GraphNode(T value) : base(value) { }
        public GraphNode(T value, NodeList<T> neighbors) : base(value, neighbors) { }

        new public NodeList<T> Neighbors
        {
            get
            {
                if (base.Neighbors == null)
                    base.Neighbors = new NodeList<T>();

                return base.Neighbors;
            }
        }

        public List<int> Costs
        {
            get
            {
                if (costs == null)
                    costs = new List<int>();

                return costs;
            }
        }
    }


    public class Graph<T> : IEnumerable<T>
    {
        public delegate void AddNodeEvent(T node);
        public delegate void RemoveNodeEvent(T node);
        public delegate void AddDirectedEdgeEvent(T from, T to, int cost);
        public delegate void AddUndirectedEdgeEvent(T from, T to, int cost);
        public delegate void RemoveUndirectedEdgeEvent(T from, T to);

        public AddNodeEvent OnAddNode;
        public RemoveNodeEvent OnRemoveNode;
        public AddDirectedEdgeEvent OnAddDirectedEdge;
        public AddUndirectedEdgeEvent OnAddUndirectedEdge;
        public RemoveUndirectedEdgeEvent OnRemoveUndirectedEdge;

        private NodeList<T> nodeSet;
        private Dictionary<GraphNode<T>, GraphNode<T>> _previous;
        private List<T> _path;
        private HashSet<T> _excluded;
        private Queue<GraphNode<T>> _queue;
        private HashSet<GraphNode<T>> _seen;

        public Graph() : this(null) 
        { 
            _previous = new Dictionary<GraphNode<T>, GraphNode<T>>();
            _path = new List<T>{};
            _excluded = new HashSet<T>();
            _queue = new Queue<GraphNode<T>>();
            _seen = new HashSet<GraphNode<T>>();
        }

        public Graph(NodeList<T> nodeSet) : base()
        {        
            if (nodeSet == null)
                this.nodeSet = new NodeList<T>();
            else
                this.nodeSet = nodeSet;
        }

        public void AddNode(GraphNode<T> node)
        {
            // adds a node to the graph
            nodeSet.Add(node);
            
            if(OnAddNode != null)
                OnAddNode(node.Value);
        }

        public void AddNode(T value)
        {
            // adds a node to the graph
            AddNode(new GraphNode<T>(value));
        }

        public void AddDirectedEdge(T from, T to, int cost = 0)
        {
            AddDirectedEdge(
                GetNodeByValue(from),
                GetNodeByValue(to),
                cost
            );
        }

        public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);

            if(OnAddDirectedEdge != null)
                OnAddDirectedEdge(from.Value, to.Value, cost);
        }

        public void AddUndirectedEdge(T from, T to, int cost = 0)
        {
            AddUndirectedEdge(
                GetNodeByValue(from),
                GetNodeByValue(to),
                cost
            );
        }

        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);

            to.Neighbors.Add(from);
            to.Costs.Add(cost);

            if(OnAddUndirectedEdge != null)
                OnAddUndirectedEdge(from.Value, to.Value, cost);
        }

        public void RemoveUndirectedEdge(GraphNode<T> from, GraphNode<T> to)
        {
            from.Neighbors.Remove(to);
            to.Neighbors.Remove(from);

            if(OnRemoveUndirectedEdge != null)
                OnRemoveUndirectedEdge(from.Value, to.Value);
        }

        public void RemoveUndirectedEdges(GraphNode<T> target)
        {
            foreach(GraphNode<T> n in target.Neighbors.ToArray())
            {
                RemoveUndirectedEdge(target, n);
            }
        }

        public void RemoveUndirectedEdges(T value)
        {
            GraphNode<T> target = (GraphNode<T>)nodeSet.FindByValue(value);
            foreach(GraphNode<T> n in target.Neighbors.ToArray())
            {
                RemoveUndirectedEdge(target, n);
            }
        }

        public bool Contains(T value)
        {
            return nodeSet.FindByValue(value) != null;
        }

        public GraphNode<T> GetNodeByValue(T value)
        {
            return (GraphNode<T>)nodeSet.FindByValue(value);
        }

        public bool Remove(T value)
        {
            // first remove the node from the nodeset
            GraphNode<T> nodeToRemove = (GraphNode<T>)nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            RemoveUndirectedEdges(nodeToRemove);

                    
            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);
            
            return true;
        }

        public void Clear()
        {
            nodeSet.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool IsPathBetweenNodes(T start, T end)
        {
            return IsPathBetweenNodes(GetNodeByValue(start), GetNodeByValue(end));
        }

        public bool IsPathBetweenNodes(GraphNode<T> start, GraphNode<T> end)
        {
            Profiler.BeginSample("Graph_IsPathBetweenNodes");

            if(start.Equals(end))
            {
                Profiler.EndSample();
                return true;
            }

            _seen.Clear();
            _queue.Clear();

            _queue.Enqueue(start);

            while (_queue.Count > 0) 
            {
                var vertex = _queue.Dequeue();

                if(_seen.Contains(vertex))
                    continue;

                _seen.Add(vertex);

                if(vertex.Equals(end))
                {
                    Profiler.EndSample();
                    return true;
                }

                Profiler.BeginSample("Graph_IsPathBetweenNodes_GetNeighbours");    

                foreach(GraphNode<T> neighbor in vertex.Neighbors) 
                {
                    _queue.Enqueue(neighbor);
                }

                Profiler.EndSample();
            }

            Profiler.EndSample();
            return false;
        }

        public T IsPathToNodes(T startRegion, Func<T, bool> filter)
        {   
            Profiler.BeginSample("Graph_IsPathToNodes");

            var start = GetNodeByValue(startRegion);

            if(filter(start.Value))
            {
                Profiler.EndSample();
                return start.Value;
            }

            _seen.Clear();
            _queue.Clear();

            _queue.Enqueue(start);

            while (_queue.Count > 0) 
            {
                var vertex = _queue.Dequeue();

                if(_seen.Contains(vertex))
                    continue;

                _seen.Add(vertex);

                if(filter(vertex.Value))
                {
                    Profiler.EndSample();
                    return vertex.Value;
                }

                Profiler.BeginSample("Graph_IsPathToNodes_GetNeighbours");    

                foreach(GraphNode<T> neighbor in vertex.Neighbors) 
                {
                    _queue.Enqueue(neighbor);
                }

                Profiler.EndSample();
            }

            Profiler.EndSample();

            return default(T);
        }

        public bool ShortestPathSearch(T start, Func<T, bool> filter, Func<T, bool> predicate, ref List<T> path) 
        {
            return ShortestPathSearch(GetNodeByValue(start), filter, predicate, ref path);
        }

        public bool ShortestPathSearch(GraphNode<T> start, Func<T, bool> filter, Func<T, bool> predicate, ref List<T> path) 
        {
            
            if(!filter(start.Value))
                return false;

            _previous.Clear();
            _path.Clear();
            _queue.Clear();

            _queue.Enqueue(start);

            while (_queue.Count > 0) 
            {
                var vertex = _queue.Dequeue();
                if(predicate(vertex.Value))
                {
                    var current = vertex;
                    while (!current.Equals(start)) 
                    {
                        path.Add(current.Value);
                        current = _previous[current];
                    };

                    path.Add(start.Value);

                    return true;
                }

                foreach(GraphNode<T> neighbor in vertex.Neighbors) 
                {
                    if (!_previous.ContainsKey(neighbor) && filter(neighbor.Value))
                    {
                        _previous[neighbor] = vertex;
                        _queue.Enqueue(neighbor);
                    }                    
                }
            }

            return false;
        }


        public NodeList<T> Nodes
        {
            get
            {
                return nodeSet;
            }
        }

        public int Count
        {
            get { return nodeSet.Count; }
        }
    }

}