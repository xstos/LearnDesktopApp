using System.Diagnostics;

[DebuggerDisplay("{Data}")]
public class Node
{
    public object Data;
    public Node? Prev { get; set; }
    public Node? Next { get; set; }
    public Node? Parent { get; set; }
    public Node? Partner { get; set; }
    public static int Seed = 1;
    public static implicit operator Node(char c) => new() { Data = c };
    public static implicit operator Node(string c) => new() { Data = c };
    public static Node[] N(params Node[] nodes) => nodes;

    public bool IsRoot => Parent == null;
    public bool IsOpen => Equals(Data,"<");
    public bool IsClose => Equals(Data,">");
    public bool IsAtom => !IsOpen && !IsClose;

    public static Node CreateCursor()
    {
        Node cur = "█";
        var (ro, rc) = N("<", ">");
        ro.PartnerWith(rc);
        E(ro, cur, rc);
        cur.Parent = ro;
        return cur;
    }

    public static Node[] Cell()
    {
        var (o, c) = N("<", ">");
        o.PartnerWith(c);
        return [o,c];
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
        return node;
    }

    public Node InsertCell()
    {
        var (co, cc) = Cell();
        co.Parent = cc.Parent = Parent;
        E(this,co, cc,Next);
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
        if (Next.IsRoot) return this;
        if (Next.IsOpen) Parent = Next;
        if (Next.IsClose) Parent = Next.Parent;
        E(Prev, Next, this, Next.Next);
        return this;
    }

    public Node MoveBack()
    {
        if (Prev.IsRoot) return this;
        if (Prev.IsOpen) Parent = Prev.Parent;
        if (Prev.IsClose) Parent = Prev.Partner;
        E(Prev.Prev, this, Prev, Next);
        return this;
    }

    public void Clear()
    {
        Prev = null;
        Next = null;
        Parent = null;
        Partner = null;
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

    public string NodesStr()
    {
        return Nodes().Select(n => n.Data.ToString())._Join(" ");
    }

    ~Node()
    {
        Console.WriteLine("destroy "+Data);
    }
}