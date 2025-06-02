using System.Collections.Generic;

public class VariableTrie
{
    class Node
    {
        public Dictionary<char, Node> Children = new();
        public uint? Value = null;
    }

    private Node root = new();

    public bool TryGet(string name, out uint value)
    {
        var n = root;
        foreach (var c in name)
        {
            if (!n.Children.TryGetValue(c, out var next))
            {
                value = 0;
                return false;
            }
            n = next;
        }
        if (n.Value.HasValue)
        {
            value = n.Value.Value;
            return true;
        }
        value = 0;
        return false;
    }

    public void Set(string name, uint value)
    {
        var n = root;
        foreach (var c in name)
        {
            if (!n.Children.ContainsKey(c))
                n.Children[c] = new Node();
            n = n.Children[c];
        }
        n.Value = value;
    }

    public bool Remove(string name)
    {
        return Remove(root, name, 0);
    }
    private bool Remove(Node node, string name, int depth)
    {
        if (depth == name.Length)
        {
            if (!node.Value.HasValue) return false;
            node.Value = null;
            return true;
        }
        char c = name[depth];
        if (!node.Children.ContainsKey(c)) return false;
        bool removed = Remove(node.Children[c], name, depth + 1);
        if (removed && node.Children[c].Children.Count == 0 && node.Children[c].Value == null)
            node.Children.Remove(c);
        return removed;
    }

    public IEnumerable<(string, uint)> ListAll()
    {
        return ListAll(root, "");
    }

    private IEnumerable<(string, uint)> ListAll(Node node, string prefix)
    {
        if (node.Value.HasValue)
            yield return (prefix, node.Value.Value);
        foreach (var kv in node.Children)
            foreach (var entry in ListAll(kv.Value, prefix + kv.Key))
                yield return entry;
    }
}