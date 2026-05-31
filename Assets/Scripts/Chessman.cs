using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;

    private string player;

    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        SetCoords();

        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;
        x *= 0.66f;
        y *= 0.66f;
        x += -2.3f;
        y += -2.3f;
        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard() { return xBoard; }
    public int GetYBoard() { return yBoard; }
    public void SetXBoard(int x) { xBoard = x; }
    public void SetYBoard(int y) { yBoard = y; }

    private void OnMouseUp()
    {
        Game game = controller.GetComponent<Game>();
        if (game.IsPaused()) return;
        if (!game.IsGameOver() && game.GetCurrentPlayer() == player)
        {
            DestroyMovePlates();
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
            Destroy(movePlates[i]);
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                CastlingMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
                break;
        }
    }

    public void CastlingMovePlate()
    {
        Game sc = controller.GetComponent<Game>();

        if (sc.HasMoved(gameObject)) return;

        int y = yBoard;

        if (sc.GetPosition(5, y) == null &&
            sc.GetPosition(6, y) == null &&
            sc.GetUnmovedRook(7, y, player) != null)
        {
            MovePlateCastlingSpawn(6, y, true);
        }

        if (sc.GetPosition(1, y) == null &&
            sc.GetPosition(2, y) == null &&
            sc.GetPosition(3, y) == null &&
            sc.GetUnmovedRook(0, y, player) != null)
        {
            MovePlateCastlingSpawn(2, y, false);
        }
    }

    public void MovePlateCastlingSpawn(int destX, int destY, bool kingside)
    {
        float wx = destX * 0.66f - 2.3f;
        float wy = destY * 0.66f - 2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(wx, wy, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.isCastling = true;
        mpScript.castlingKingside = kingside;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(destX, destY);
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionsOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionsOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
            MovePlateAttackSpawn(x, y);
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionsOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);
            if (cp == null)
                MovePlateSpawn(x, y);
            else if (cp.GetComponent<Chessman>().player != player)
                MovePlateAttackSpawn(x, y);
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        if (sc.PositionsOnBoard(x, y))
        {
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);

                if (this.name == "white_pawn" && yBoard == 1)
                    if (sc.GetPosition(x, y + 1) == null)
                        MovePlateSpawn(x, y + 1);

                if (this.name == "black_pawn" && yBoard == 6)
                    if (sc.GetPosition(x, y - 1) == null)
                        MovePlateSpawn(x, y - 1);
            }

            if (sc.PositionsOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null
                && sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
                MovePlateAttackSpawn(x + 1, y);

            if (sc.PositionsOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null
                && sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
                MovePlateAttackSpawn(x - 1, y);

            if (sc.PositionsOnBoard(x + 1, yBoard) && sc.IsEnPassantTarget(x + 1, yBoard))
            {
                Debug.Log("[EnPassant] Case droite disponible!");
                MovePlateEnPassantSpawn(x + 1, y);
            }

            if (sc.PositionsOnBoard(x - 1, yBoard) && sc.IsEnPassantTarget(x - 1, yBoard))
            {
                Debug.Log("[EnPassant] Case gauche disponible!");
                MovePlateEnPassantSpawn(x - 1, y);
            }
        }
    }

    public void MovePlateEnPassantSpawn(int matrixX, int matrixY)
    {
        float x = matrixX * 0.66f - 2.3f;
        float y = matrixY * 0.66f - 2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.isEnPassant = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX * 0.66f - 2.3f;
        float y = matrixY * 0.66f - 2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX * 0.66f - 2.3f;
        float y = matrixY * 0.66f - 2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}