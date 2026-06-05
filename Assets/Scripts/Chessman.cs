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

        player = this.name.StartsWith("white") ? "white" : "black";

        if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            ApplySkin(SkinManager.Instance.CurrentSkin);
        else
            ApplyFallbackSprite();
    }

    public void ApplySkin(SkinData skin)
    {
        if (skin == null) return;
        Sprite s = skin.GetSprite(this.name);
        if (s != null)
            this.GetComponent<SpriteRenderer>().sprite = s;
        else
            ApplyFallbackSprite();
    }

    public void SetCoords()
    {
        this.transform.position = BoardToWorld(xBoard, yBoard);
        // Pieces sit at z = -1 so they render above the board
        this.transform.position = new Vector3(
            this.transform.position.x,
            this.transform.position.y,
            -1.0f);
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
        foreach (GameObject mp in GameObject.FindGameObjectsWithTag("MovePlate"))
            Destroy(mp);
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "white_queen":
            case "black_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;

            case "white_knight":
            case "black_knight":
                LMovePlate();
                break;

            case "white_bishop":
            case "black_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;

            case "white_king":
            case "black_king":
                SurroundMovePlate();
                CastlingMovePlates();
                break;

            case "white_rook":
            case "black_rook":
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

    private void CastlingMovePlates()
    {
        Game sc = controller.GetComponent<Game>();

        if (sc.CanCastleKingside(player))
        {
            int row = (player == "white") ? 0 : 7;
            GameObject mp = Instantiate(movePlate, BoardToWorld(6, row), Quaternion.identity);
            MovePlate mpScript = mp.GetComponent<MovePlate>();
            mpScript.isCastling = true;
            mpScript.castlingKingside = true;
            mpScript.SetReference(gameObject);
            mpScript.SetCoords(6, row);
        }

        if (sc.CanCastleQueenside(player))
        {
            int row = (player == "white") ? 0 : 7;
            GameObject mp = Instantiate(movePlate, BoardToWorld(2, row), Quaternion.identity);
            MovePlate mpScript = mp.GetComponent<MovePlate>();
            mpScript.isCastling = true;
            mpScript.castlingKingside = false;
            mpScript.SetReference(gameObject);
            mpScript.SetCoords(2, row);
        }
    }

    public void LineMovePlate(int xInc, int yInc)
    {
        Game sc = controller.GetComponent<Game>();
        int x = xBoard + xInc;
        int y = yBoard + yInc;

        while (sc.PositionsOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            TrySpawnMovePlate(x, y, false, false);
            x += xInc;
            y += yInc;
        }

        if (sc.PositionsOnBoard(x, y) &&
            sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            TrySpawnMovePlate(x, y, true, false);
        }
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
        if (!sc.PositionsOnBoard(x, y)) return;

        GameObject cp = sc.GetPosition(x, y);
        if (cp == null)
            TrySpawnMovePlate(x, y, false, false);
        else if (cp.GetComponent<Chessman>().player != player)
            TrySpawnMovePlate(x, y, true, false);
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (!sc.PositionsOnBoard(x, y)) return;

        if (sc.GetPosition(x, y) == null)
        {
            TrySpawnMovePlate(x, y, false, false);

            if (this.name == "white_pawn" && yBoard == 1 && sc.GetPosition(x, y + 1) == null)
                TrySpawnMovePlate(x, y + 1, false, false);
            if (this.name == "black_pawn" && yBoard == 6 && sc.GetPosition(x, y - 1) == null)
                TrySpawnMovePlate(x, y - 1, false, false);
        }

        if (sc.PositionsOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null &&
            sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            TrySpawnMovePlate(x + 1, y, true, false);

        if (sc.PositionsOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
            sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            TrySpawnMovePlate(x - 1, y, true, false);

        if (sc.PositionsOnBoard(x + 1, yBoard) && sc.IsEnPassantTarget(x + 1, yBoard))
            MovePlateEnPassantSpawn(x + 1, y);

        if (sc.PositionsOnBoard(x - 1, yBoard) && sc.IsEnPassantTarget(x - 1, yBoard))
            MovePlateEnPassantSpawn(x - 1, y);
    }

    private void TrySpawnMovePlate(int x, int y, bool isAttack, bool isCastle)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.MoveLeavesKingInCheck(gameObject, x, y)) return;

        if (isAttack)
            MovePlateAttackSpawn(x, y);
        else
            MovePlateSpawn(x, y);
    }

    public void MovePlateEnPassantSpawn(int matrixX, int matrixY)
    {
        GameObject mp = Instantiate(movePlate, BoardToWorld(matrixX, matrixY), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.isEnPassant = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        GameObject mp = Instantiate(movePlate, BoardToWorld(matrixX, matrixY), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        GameObject mp = Instantiate(movePlate, BoardToWorld(matrixX, matrixY), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    private Vector3 BoardToWorld(int x, int y)
    {
        return new Vector3(x * 0.66f - 2.3f, y * 0.66f - 2.3f, -3.0f);
    }

    private void ApplyFallbackSprite()
    {
        Sprite s = null;
        switch (this.name)
        {
            case "black_queen": s = black_queen; break;
            case "black_knight": s = black_knight; break;
            case "black_bishop": s = black_bishop; break;
            case "black_king": s = black_king; break;
            case "black_rook": s = black_rook; break;
            case "black_pawn": s = black_pawn; break;
            case "white_queen": s = white_queen; break;
            case "white_knight": s = white_knight; break;
            case "white_bishop": s = white_bishop; break;
            case "white_king": s = white_king; break;
            case "white_rook": s = white_rook; break;
            case "white_pawn": s = white_pawn; break;
        }
        if (s != null) this.GetComponent<SpriteRenderer>().sprite = s;
    }
}