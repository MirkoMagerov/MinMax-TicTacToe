using System.Collections;
using UnityEngine;

public enum States
{
    CanMove,
    CantMove
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BoxCollider2D collider;
    public GameObject token1, token2;
    public int Size = 3;
    public int[,] Matrix;
    [SerializeField] private States state = States.CanMove;
    public Camera camera;

    void Start()
    {
        Instance = this;
        Matrix = new int[Size, Size];
        Calculs.CalculateDistances(collider, Size);
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Matrix[i, j] = 0;
            }
        }
    }

    private void Update()
    {
        if (state == States.CanMove)
        {
            Vector3 m = Input.mousePosition;
            m.z = 10f;
            Vector3 mousepos = camera.ScreenToWorldPoint(m);
            if (Input.GetMouseButtonDown(0))
            {
                if (Calculs.CheckIfValidClick((Vector2)mousepos, Matrix))
                {
                    state = States.CantMove;
                    if(Calculs.EvaluateWin(Matrix)==2)
                        StartCoroutine(WaitingABit());
                }
            }
        }
    }

    public void AITurn()
    {
        // Crear nodo inicial con el estado actual del tablero
        Node rootNode = new Node(
            parent: null,
            team: -1,  // La IA siempre juega como -1 
            alpha: -int.MaxValue,
            beta: int.MaxValue,
            x: -1,
            y: -1,
            matrixNode: Matrix
        );

        // Profundidad de búsqueda (ajustable)
        int depth = 4;

        // Ejecutar Minimax
        MinMax(rootNode, depth, true, -int.MaxValue, int.MaxValue);

        // Encontrar el mejor movimiento
        Node bestMove = null;
        int bestValue = -int.MaxValue;

        foreach (Node child in rootNode.NodeChildren)
        {
            if (child.Value > bestValue)
            {
                bestValue = child.Value;
                bestMove = child;
            }
        }

        // Realizar el mejor movimiento
        if (bestMove != null)
        {
            DoMove(bestMove.X, bestMove.Y, -1);
            state = States.CanMove;
        }
    }

    // Modificar WaitingABit para usar AITurn
    private IEnumerator WaitingABit()
    {
        yield return new WaitForSeconds(1f);
        AITurn();  // Reemplazar RandomAI con AITurn
    }

    public void DoMove(int x, int y, int team)
    {
        Matrix[x, y] = team;
        if (team == 1)
            Instantiate(token1, Calculs.CalculatePoint(x, y), Quaternion.identity);
        else
            Instantiate(token2, Calculs.CalculatePoint(x, y), Quaternion.identity);
        int result = Calculs.EvaluateWin(Matrix);
        switch (result)
        {
            case 0:
                Debug.Log("Draw");
                break;
            case 1:
                Debug.Log("You Win");
                break;
            case -1:
                Debug.Log("You Lose");
                break;
            case 2:
                if(state == States.CantMove)
                    state = States.CanMove;
                break;
        }
    }

    public void MinMax(Node node, int depth, bool isMaxing, int alpha, int beta)
    {
        int gameResult = Calculs.EvaluateWin(node.MatrixNode);

        // Early termination checks
        if (depth == 0 || gameResult != 2)
        {
            node.Value = gameResult * (depth + 1);
            return;
        }

        if (isMaxing)
        {
            int bestValue = -int.MaxValue;
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (node.MatrixNode[x, y] == 0)
                    {
                        Node childNode = CreateChildNode(node, x, y);
                        MinMax(childNode, depth - 1, false, alpha, beta);

                        bestValue = Mathf.Max(bestValue, childNode.Value);
                        alpha = Mathf.Max(alpha, bestValue);

                        if (beta <= alpha)
                            break;
                    }
                }
            }
            node.Value = bestValue;
        }
        else
        {
            int bestValue = int.MaxValue;
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (node.MatrixNode[x, y] == 0)
                    {
                        Node childNode = CreateChildNode(node, x, y);
                        MinMax(childNode, depth - 1, true, alpha, beta);

                        bestValue = Mathf.Min(bestValue, childNode.Value);
                        beta = Mathf.Min(beta, bestValue);

                        if (beta <= alpha)
                            break;
                    }
                }
            }
            node.Value = bestValue;
        }
    }

    public Node CreateChildNode(Node nodoActual, int x, int y)
    {
        // Crear una copia del matriz actual
        int[,] newMatrix = new int[Size, Size];
        System.Array.Copy(nodoActual.MatrixNode, newMatrix, nodoActual.MatrixNode.Length);

        // Cambiar el equipo del nodo hijo (alternar entre 1 y -1)
        int newTeam = -nodoActual.Team;

        // Crear nuevo nodo hijo
        Node childNode = new Node(
            parent: nodoActual,
            team: newTeam,
            alpha: nodoActual.Alpha,
            beta: nodoActual.Beta,
            x: x,
            y: y,
            matrixNode: newMatrix
        );

        // Realizar el movimiento en la nueva matriz
        childNode.MatrixNode[x, y] = newTeam;

        // Agregar el nodo hijo a la pila de hijos del nodo actual
        nodoActual.NodeChildren.Push(childNode);

        return childNode;
    }
}
