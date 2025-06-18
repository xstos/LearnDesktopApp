using System.Diagnostics;
using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using OneOf.Types;

[DebuggerDisplay("{Data}")]
public class Node
{
    public const string cursorSymbol = "█";
    private const string historyCursor = "▒";
    public object Data;
    public Node? Prev { get; set; }
    public Node? Next { get; set; }
    public Node? Parent { get; set; }
    public Node? Partner { get; set; }
    public Node? LastCursorPos { get; set; }
    
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
        ro.LastCursorPos = cur;
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
    
    public Node MoveForward()
    {
        var next = Next;
        if (next.IsRoot) return this;
        
        if (next.IsOpen)
        {
            //<derp█<hello>there>
            var oldParent = Parent;
            Node hist = historyCursor;
            InsertAtom(hist);
            oldParent.LastCursorPos = hist;
            Parent = next;
            next.LastCursorPos?.Remove();
            E(hist,next,this,next.Next);
        }

        if (next.IsClose)
        {
            //<test<hello█>there>
        }

        if (next.IsAtom)
        {
            //<test█foo>
            E(Prev, Next, this, next.Next);
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
            //<derp<█hello>there>
            Parent = prev.Parent;
            
        }

        if (prev.IsClose)
        {
            //<derp<hello>█there>
            Parent = prev.Partner;
        }

        if (prev.IsAtom)
        {
            //<derp<hel█lo>there>
            //<derp<he█llo>there>
        }
        E(prev.Prev,this,prev,Next);
        Parent.LastCursorPos?.Remove();
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