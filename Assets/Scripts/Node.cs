using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int EValue { get; set; }
    public int Team { get; set; }
    public Node Parent { get; set; }
    public int Alpha { get; set; }
    public int Beta { get; set; }
    public int Value { get; set; }
    public Stack<Node> NodeChildren { get; set; }
    public int[,] MatrixNode { get; set; }
    public Node BestChild { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool Pruned { get; set; }

    public Node(Node parent, int team, int alpha, int beta, int x, int y, int[,] matrixNode)
    {
        Team = team;
        Parent = parent;
        Alpha = alpha;
        Beta = beta;
        Value = -team * 2;
        NodeChildren = new Stack<Node>();
        MatrixNode = matrixNode;
        X = x;
        Y = y;
        BuildAuxMatrix(this);
        Pruned = false;
        //DebugMatrix(MatrixNode);
    }

    public void BuildAuxMatrix(Node node)
    {
        if (node.Parent != null && node.Parent.X >= 0 && node.Parent.Y >= 0)
        {
            MatrixNode[node.Parent.X, node.Parent.Y] = -node.Parent.Team;
            BuildAuxMatrix(node.Parent);
        }
    }

    public void DebugMatrix(int[,] matrix)
    {
        string text = "";
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                text += matrix[i, j] + " ";
            }
            text += "\n";
        }
        Debug.Log(text);
    }
}