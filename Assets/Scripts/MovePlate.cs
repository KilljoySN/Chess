using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;
    GameObject reference = null;
    int matrixX;
    int matrixY;

    public bool attack = false;
    public bool isEnPassant = false;

    public bool isCastling = false;
    public bool castlingKingside = false;

    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
        if (isCastling)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.85f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game game = controller.GetComponent<Game>();

        if (attack)
        {
            if (isEnPassant)
            {
                GameObject captured = game.GetEnPassantTarget();
                if (captured != null)
                {
                    Chessman capturedCm = captured.GetComponent<Chessman>();
                    if (captured.name == "white_king") game.Winner("black");
                    if (captured.name == "black_king") game.Winner("white");
                    game.SetPositionEmptty(capturedCm.GetXBoard(), capturedCm.GetYBoard());
                    Destroy(captured);
                }
            }
            else
            {
                GameObject cp = game.GetPosition(matrixX, matrixY);
                if (cp != null)
                {
                    if (cp.name == "white_king") game.Winner("black");
                    if (cp.name == "black_king") game.Winner("white");
                    Destroy(cp);
                }
            }
        }

        Chessman movingCm = reference.GetComponent<Chessman>();
        int prevY = movingCm.GetYBoard();
        game.SetPositionEmptty(movingCm.GetXBoard(), movingCm.GetYBoard());
        movingCm.SetXBoard(matrixX);
        movingCm.SetYBoard(matrixY);
        movingCm.SetCoords();
        game.SetPosition(reference);

        game.MarkAsMoved(reference);

        if (isCastling)
        {
            string player = movingCm.name.Contains("white") ? "white" : "black";
            int row = matrixY;

            if (castlingKingside)
            {
                GameObject rook = game.GetPosition(7, row);
                if (rook != null)
                {
                    Chessman rookCm = rook.GetComponent<Chessman>();
                    game.SetPositionEmptty(7, row);
                    rookCm.SetXBoard(5);
                    rookCm.SetYBoard(row);
                    rookCm.SetCoords();
                    game.SetPosition(rook);
                    game.MarkAsMoved(rook);
                }
            }
            else
            {
                GameObject rook = game.GetPosition(0, row);
                if (rook != null)
                {
                    Chessman rookCm = rook.GetComponent<Chessman>();
                    game.SetPositionEmptty(0, row);
                    rookCm.SetXBoard(3);
                    rookCm.SetYBoard(row);
                    rookCm.SetCoords();
                    game.SetPosition(rook);
                    game.MarkAsMoved(rook);
                }
            }
        }

        if ((movingCm.name == "white_pawn" || movingCm.name == "black_pawn")
            && Mathf.Abs(matrixY - prevY) == 2)
        {
            game.SetEnPassantTarget(reference, matrixX, matrixY);
        }
        else
        {
            GameObject currentTarget = game.GetEnPassantTarget();
            if (currentTarget != null)
            {
                string movingPlayer = movingCm.name.Contains("white") ? "white" : "black";
                string targetPlayer = currentTarget.name.Contains("white") ? "white" : "black";
                if (movingPlayer == targetPlayer)
                    game.ClearEnPassantTarget();
            }
        }

        game.NextTurn();
        reference.GetComponent<Chessman>().DestroyMovePlates();

        if (movingCm.name == "white_pawn" && matrixY == 7)
        {
            game.PromotePawn(reference);
            return;
        }
        if (movingCm.name == "black_pawn" && matrixY == 0)
        {
            game.PromotePawn(reference);
            return;
        }
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}