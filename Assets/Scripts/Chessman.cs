using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;

    private string player;

    // ---------------------------------------------------------------
    // Fallback sprites (used when no SkinManager / skin has no sprite)
    // ---------------------------------------------------------------
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    // ---------------------------------------------------------------
    // Activation
    // ---------------------------------------------------------------

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        // Determine player colour from name
        player = this.name.StartsWith("white") ? "white" : "black";

        // Try to apply the active skin; fall back to the inspector sprites
        if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
        {
            ApplySkin(SkinManager.Instance.CurrentSkin);
        }
        else
        {
            ApplyFallbackSprite();
        }
    }

    // ---------------------------------------------------------------
    // Skin API
    // ---------------------------------------------------------------

    /// <summary>Apply a SkinData to this piece (called by SkinManager too).</summary>
    public void ApplySkin(SkinData skin)
    {
        if (skin == null) return;

        Sprite s = skin.GetSprite(this.name);

        if (s != null)
        {
            this.GetComponent<SpriteRenderer>().sprite = s;
        }
        else
        {
            // Skin is missing this sprite – fall back gracefully
            ApplyFallbackSprite();
        }
    }

    // ---------------------------------------------------------------
    // Coordinates
    // ---------------------------------------------------------------

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

    // ---------------------------------------------------------------
    // Mouse interaction
    // ---------------------------------------------------------------

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

    // ---------------------------------------------------------------
    // Move plates
    // ---------------------------------------------------------------

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
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
        {
            MovePlateAttackSpawn(x, y);
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

            if (sc.PositionsOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null &&
                sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
                MovePlateAttackSpawn(x + 1, y);

            if (sc.PositionsOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
                sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
                MovePlateAttackSpawn(x - 1, y);

            if (sc.PositionsOnBoard(x + 1, yBoard) && sc.IsEnPassantTarget(x + 1, yBoard))
                MovePlateEnPassantSpawn(x + 1, y);

            if (sc.PositionsOnBoard(x - 1, yBoard) && sc.IsEnPassantTarget(x - 1, yBoard))
                MovePlateEnPassantSpawn(x - 1, y);
        }
    }

    // ---------------------------------------------------------------
    // Move plate spawners
    // ---------------------------------------------------------------

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

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

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