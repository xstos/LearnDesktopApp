using System.Diagnostics;
using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using OneOf.Types;

[DebuggerDisplay("{Data} {NodeStr}")]
public class Node
{
    public const string cursorSymbol = "█";
    private const string historyCursor = "▒";
    public object Data;
    public Node? Prev { get; set; }
    public Node? Next { get; set; }
    public Node? Parent { get; set; }
    public Node? Partner { get; set; }
    public Node? Cursor { get; set; }
    
    public static implicit operator Node(char c) => new() { Data = c };
    public static implicit operator Node(string c) => new() { Data = c };
    public static Node[] N(params Node[] nodes) => nodes;

    public bool IsRoot => Parent == null;
    public bool IsOpen => Equals(Data,"<");
    public bool IsClose => Equals(Data,">");
    public bool IsAtom => !IsOpen && !IsClose || IsCursor || IsHistoryCursor;
    public bool IsCursor => Equals(Data,cursorSymbol);
    public bool IsHistoryCursor => Equals(Data,historyCursor);

    public Node GetRoot()
    {
        var cur = this;
        while (cur.Parent != null) cur = Parent;

        return cur;
    }
    public static (Node,Node,Node) CreateCursor()
    {
        var (ro,cur, rc) = N("<",cursorSymbol, ">");
        ro.PartnerWith(rc);
        E(ro, cur, rc);
        cur.Parent = ro;
        ro.Cursor = cur;
        return (ro,cur,rc);
    }
    public static (Node,Node) CreateCell()
    {
        var (ro, rc) = N("<", ">");
        ro.PartnerWith(rc);
        E(ro, rc);
        return (ro,rc);
    }

    public Node PartnerWith(Node other)
    {
        Partner = other;
        other.Partner = this;
        return this;
    }

    public Node InsertAtom(Node node)
    {
        E(Prev, node, this);
        node.Parent = Parent;
        return this;
    }

    public Node ReplaceWith(Node node)
    {
        E(Prev,node,Next);
        node.Parent = Parent;
        return this;
    }

    public Node SwapWith(Node node)
    {
        var (p1,n1) = (Prev,Next);
        var (p2,n2) = (node.Prev,node.Next);
        E(p1,node,n1);
        E(p2,this,n2);
        return this;
    }
    public Node MoveBetween(Node a, Node b)
    {
        if (a==null || b==null) return this;
        // (a)█< (a)█> (<)█a (>)█a (a)█a (<)█< (>)█> (<)█> (>)█<
        Node newParent=null;
        if(a==this||b==this) return this;
        if (a.IsAtom) newParent = a.Parent;
        if (a.IsOpen) newParent = a;
        if (a.IsClose) newParent = a.Partner.Parent;
        if (newParent == null) throw new Exception("this should never happen");
        var oldParent = Parent;
        if (oldParent != newParent)
        {
            var hist = MakeHistoryCursor();
            
            oldParent.Cursor = hist;
            this.ReplaceWith(hist);
            var newParentCursor = newParent.Cursor;
            
            newParent.Cursor = this;
            E(a, this, b);
            Parent = newParent;
            if (newParentCursor!=null && newParentCursor != this)
            {
                newParentCursor.Remove();
            }
        }
        else
        {
            Remove();
            E(a, this, b);
            
        }
        return this;
    }
    public Node InsertCell()
    {
        var (open,close) = CreateCell();
        open.Parent = close.Parent = Parent;
        E(this, open,close,Next);
        return this;
    }
    
    public static IEnumerable<Node> E(params Node[] nodes)
    {
        for (int i = 1; i < nodes.Length; i++)
        {
            Edge(nodes[i-1], nodes[i]);
        }
        GC.Collect();
        return nodes;
    }

    public static Node MakeHistoryCursor() => historyCursor;

    public string NodeStr => Nodes().Select(n => n.Data + "")._Join("");
    public Node MoveForward()
    {
        var next = Next;
        if (next.IsRoot) return this;
        MoveBetween(next, next.Next);
        return this;
    }

    public void Remove()
    {
        E(Prev, Next);
    }
    public Node MoveBack()
    {
        var prev = Prev;
        if (prev.IsRoot) return this;
        MoveBetween(prev.Prev, prev);
        return this;
    }

    public Node Backspace()
    {
        var prev = Prev;
        if (prev.IsAtom)
        {
            E(prev.Prev, this);
        }

        return this;
    }

    public Node DeleteAtom()
    {
        var next = Next;
        if (next.IsAtom)
        {
            E(this, next.Next);
        }

        return this;
    }

    public Node DeleteCell()
    {
        var next = Next;
        if (next.IsOpen)
        {
            E(this, next.Partner.Next);
        }

        return this;
    }

    public Node BackspaceCell()
    {
        var prev = Prev;
        if (prev.IsClose)
        {
            E(prev.Partner.Prev, this);
        }

        return this;
    }
    public static void Edge(Node a, Node b)
    {
        if (a!=null) a.Next = b;
        if (b!=null) b.Prev = a;
    }

    public IEnumerable<Node> Nodes()
    {
        IEnumerable<Node> Before()
        {
            var cur = Prev;
            while (cur != null)
            {
                yield return cur;
                cur = cur.Prev;
            }
        }
        IEnumerable<Node> After()
        {
            var cur = Next;
            while (cur != null)
            {
                yield return cur;
                cur = cur.Next;
            }
        }
        return Before().Reverse().Concat([this,..After()]);
    }

    public void ForEachNode(Action<Node> action)
    {
        var root = GetRoot();
        var partner = root.Partner;
        var n = root.Next;
        while (n != partner)
        {
            action(n);
            n = n.Next;
        }
    }
    public string NodesStr()
    {
        return Nodes().Select(n => n.Data.ToString())._Join(" ");
    }

    ~Node()
    {
        Console.WriteLine("destroy "+Data);
    }
    
}