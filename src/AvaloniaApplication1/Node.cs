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
    public Node? LastCursor { get; set; }
    
    public static implicit operator Node(char c) => new() { Data = c };
    public static implicit operator Node(string c) => new() { Data = c };
    public static Node[] N(params Node[] nodes) => nodes;

    public bool IsRoot => Parent == null;
    public bool IsOpen => Equals(Data,"<");
    public bool IsClose => Equals(Data,">");
    public bool IsAtom => !IsOpen && !IsClose;
    public bool IsCursor => Equals(Data,cursorSymbol);
    public bool IsHistoryCursor => Equals(Data,historyCursor);

    public Node GetRoot()
    {
        var cur = this;
        while (cur.Parent != null)
        {
            cur = Parent;
        }

        return cur;
    }
    public static (Node,Node,Node) CreateCursor()
    {
        var (ro,cur, rc) = N("<",cursorSymbol, ">");
        ro.PartnerWith(rc);
        E(ro, cur, rc);
        cur.Parent = ro;
        ro.LastCursor = cur;
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

    public void ClearLastCursor()
    {
        LastCursor?.Remove();
        LastCursor = null;
    }
    public static Node MakeHistoryCursor() => historyCursor;

    public Node MakeInsertHistoryCursor()
    {
        var hist = MakeHistoryCursor();
        InsertAtom(hist);
        Parent.LastCursor = hist;
        return hist;
    }

    public string NodeStr => Nodes().Select(n => n.Data + "")._Join("");
    public Node MoveForward()
    {
        var next = Next;
        if (next.IsRoot) return this;
        
        if (next.IsOpen)
        {
            //<derp█<hel▒lo>there>
            var oldParent = Parent;
            var newParent = next;
            newParent.ClearLastCursor();
            //<derp█<hello>there>
            var hist = MakeInsertHistoryCursor();
            oldParent.LastCursor = hist;
            //<derp▒█<hello>there>
            E(hist,next,this,next.Next);
            Parent = next;
            next.LastCursor = this;
            //<derp▒<█hello>there>
        }

        if (next.IsClose)
        {
            //<test<hello█>the▒re>
            var oldParent = Parent;
            var newParent = next.Partner.Parent;
            newParent.ClearLastCursor();
            //<test<hello█>there>
            var hist = MakeInsertHistoryCursor();
            oldParent.LastCursor = hist;
            //<test<hello▒█>there>
            Parent = next.Partner.Parent;
            newParent.LastCursor = this;
            E(hist, next, this, next.Next);
        }

        if (next.IsAtom)
        {
            //<test█foo>
            E(Prev, Next, this, next.Next);
            //<testf█oo>
        }
        
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
        if (prev.IsOpen)
        {
            //<de▒rp<█hello>there>
            var oldParent = Parent;
            var newParent = prev.Parent;
            newParent.ClearLastCursor();
            var hist = MakeHistoryCursor();
            var (po,o,n) = (prev.Prev,prev,Next);
            E(po, this, o, hist, n);
            Parent = newParent;
            o.LastCursor=hist;
            newParent.LastCursor=this;
            //<derp<█hello>there>

        }

        if (prev.IsClose)
        {
            //<derp<hel▒lo>█there>
            var hist = MakeHistoryCursor();
            var oldParent = Parent;
            var newParent = prev.Partner;
            prev.Partner.ClearLastCursor();
            //<derp<hello>█there>
            E(prev.Prev,this,prev,hist,Next);
            oldParent.LastCursor = hist;
            newParent.LastCursor = this;
            Parent = newParent;
        }

        if (prev.IsAtom)
        {
            //<derp<hel█lo>there>
            //<derp<he█llo>there>
            E(prev.Prev,this,prev,Next);
            
        }
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

    public void RenderAll(TextBlock ret)
    {
        var sb = new StringBuilder();
        var stack = new Stack<TextBlock>() ;
        stack.Push(ret);
        ForEachNode(node =>
        {
            var parent = stack.Peek();
            if (node.IsAtom)
            {
                parent.Inlines.Add(new Run(node.Data.ToString()));
            }
            else
            {
                parent.Inlines.Add(new Run(node.Data.ToString()));
            }
            
        });
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