using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;
    public GameObject promotionMenu;
    private GameObject pawnToPromote;

    public Image promoQueenImage;
    public Image promoRookImage;
    public Image promoBishopImage;
    public Image promoKnightImage;

    public Sprite white_queen_sprite;
    public Sprite white_rook_sprite;
    public Sprite white_bishop_sprite;
    public Sprite white_knight_sprite;

    public Sprite black_queen_sprite;
    public Sprite black_rook_sprite;
    public Sprite black_bishop_sprite;
    public Sprite black_knight_sprite;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private string currentPlayer = "white";
    private bool gameOver = false;
    private bool isPaused = false;

    private GameObject enPassantTarget = null;
    private int enPassantX = -1;
    private int enPassantY = -1;

    private HashSet<GameObject> movedPieces = new HashSet<GameObject>();

    public void SetEnPassantTarget(GameObject pawn, int x, int y)
    {
        enPassantTarget = pawn;
        enPassantX = x;
        enPassantY = y;
    }

    public void ClearEnPassantTarget()
    {
        enPassantTarget = null;
        enPassantX = -1;
        enPassantY = -1;
    }

    public bool IsEnPassantTarget(int x, int y)
    {
        return enPassantX == x && enPassantY == y;
    }

    public GameObject GetEnPassantTarget() { return enPassantTarget; }

    public bool IsPaused() { return isPaused; }
    public bool IsPromoting() { return promotionMenu != null && promotionMenu.activeSelf; }

    public GameObject pauseMenu;

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void MarkAsMoved(GameObject piece)
    {
        movedPieces.Add(piece);
    }

    public bool HasMoved(GameObject piece)
    {
        return movedPieces.Contains(piece);
    }

    public GameObject GetUnmovedRook(int x, int y, string player)
    {
        GameObject obj = GetPosition(x, y);
        if (obj == null) return null;
        Chessman cm = obj.GetComponent<Chessman>();
        if (cm == null) return null;
        if (obj.name != player + "_rook") return null;
        if (HasMoved(obj)) return null;
        return obj;
    }

    void Start()
    {
        playerWhite = new GameObject[]
        {
            Create("white_rook",   0, 0), Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0), Create("white_queen",  3, 0),
            Create("white_king",   4, 0), Create("white_bishop", 5, 0),
            Create("white_knight", 6, 0), Create("white_rook",   7, 0),
            Create("white_pawn",   0, 1), Create("white_pawn",   1, 1),
            Create("white_pawn",   2, 1), Create("white_pawn",   3, 1),
            Create("white_pawn",   4, 1), Create("white_pawn",   5, 1),
            Create("white_pawn",   6, 1), Create("white_pawn",   7, 1),
        };

        playerBlack = new GameObject[]
        {
            Create("black_rook",   0, 7), Create("black_knight", 1, 7),
            Create("black_bishop", 2, 7), Create("black_queen",  3, 7),
            Create("black_king",   4, 7), Create("black_bishop", 5, 7),
            Create("black_knight", 6, 7), Create("black_rook",   7, 7),
            Create("black_pawn",   0, 6), Create("black_pawn",   1, 6),
            Create("black_pawn",   2, 6), Create("black_pawn",   3, 6),
            Create("black_pawn",   4, 6), Create("black_pawn",   5, 6),
            Create("black_pawn",   6, 6), Create("black_pawn",   7, 6),
        };

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y) { positions[x, y] = null; }

    public void SetPositionEmptty(int x, int y) { SetPositionEmpty(x, y); }

    public GameObject GetPosition(int x, int y) { return positions[x, y]; }

    public bool PositionsOnBoard(int x, int y)
    {
        return x >= 0 && y >= 0 && x < 8 && y < 8;
    }

    public string GetCurrentPlayer() { return currentPlayer; }
    public bool IsGameOver() { return gameOver; }

    public void NextTurn()
    {
        ClearEnPassantTarget();

        currentPlayer = (currentPlayer == "white") ? "black" : "white";
    }

    void Update()
    {
        if (gameOver && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("GameScene");
        }
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }

    public bool IsInCheck(string player)
    {
        int kingX = -1, kingY = -1;
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                GameObject obj = positions[x, y];
                if (obj != null && obj.name == player + "_king")
                {
                    kingX = x; kingY = y;
                }
            }

        if (kingX == -1) return false;

        string enemy = (player == "white") ? "black" : "white";

        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                GameObject obj = positions[x, y];
                if (obj == null) continue;
                if (!obj.name.StartsWith(enemy)) continue;

                if (PieceAttacksSquare(obj, kingX, kingY))
                    return true;
            }

        return false;
    }

    public bool MoveLeavesKingInCheck(GameObject piece, int toX, int toY)
    {
        Chessman cm = piece.GetComponent<Chessman>();
        int fromX = cm.GetXBoard();
        int fromY = cm.GetYBoard();
        string player = piece.name.Contains("white") ? "white" : "black";

        GameObject captured = positions[toX, toY];

        positions[fromX, fromY] = null;
        positions[toX, toY] = piece;
        cm.SetXBoard(toX);
        cm.SetYBoard(toY);

        bool inCheck = IsInCheck(player);

        positions[toX, toY] = captured;
        positions[fromX, fromY] = piece;
        cm.SetXBoard(fromX);
        cm.SetYBoard(fromY);

        return inCheck;
    }

    private bool PieceAttacksSquare(GameObject piece, int tx, int ty)
    {
        Chessman cm = piece.GetComponent<Chessman>();
        int px = cm.GetXBoard();
        int py = cm.GetYBoard();
        string name = piece.name;

        switch (name)
        {
            case "white_rook":
            case "black_rook":
                return RookAttacks(px, py, tx, ty);

            case "white_bishop":
            case "black_bishop":
                return BishopAttacks(px, py, tx, ty);

            case "white_queen":
            case "black_queen":
                return RookAttacks(px, py, tx, ty) || BishopAttacks(px, py, tx, ty);

            case "white_knight":
            case "black_knight":
                return KnightAttacks(px, py, tx, ty);

            case "white_king":
            case "black_king":
                return Mathf.Abs(tx - px) <= 1 && Mathf.Abs(ty - py) <= 1;

            case "white_pawn":
                return ty == py + 1 && (tx == px + 1 || tx == px - 1);

            case "black_pawn":
                return ty == py - 1 && (tx == px + 1 || tx == px - 1);
        }

        return false;
    }

    private bool RookAttacks(int px, int py, int tx, int ty)
    {
        if (px != tx && py != ty) return false;

        int dx = (tx == px) ? 0 : (tx > px ? 1 : -1);
        int dy = (ty == py) ? 0 : (ty > py ? 1 : -1);
        int x = px + dx, y = py + dy;

        while (x != tx || y != ty)
        {
            if (positions[x, y] != null) return false;
            x += dx; y += dy;
        }
        return true;
    }

    private bool BishopAttacks(int px, int py, int tx, int ty)
    {
        if (Mathf.Abs(tx - px) != Mathf.Abs(ty - py)) return false;

        int dx = tx > px ? 1 : -1;
        int dy = ty > py ? 1 : -1;
        int x = px + dx, y = py + dy;

        while (x != tx || y != ty)
        {
            if (positions[x, y] != null) return false;
            x += dx; y += dy;
        }
        return true;
    }

    private bool KnightAttacks(int px, int py, int tx, int ty)
    {
        int dx = Mathf.Abs(tx - px);
        int dy = Mathf.Abs(ty - py);
        return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);
    }

    public bool CanCastleKingside(string player)
    {
        int row = (player == "white") ? 0 : 7;

        GameObject king = GetPosition(4, row);
        if (king == null || king.name != player + "_king") return false;
        if (HasMoved(king)) return false;

        if (GetUnmovedRook(7, row, player) == null) return false;

        if (GetPosition(5, row) != null || GetPosition(6, row) != null) return false;

        if (IsInCheck(player)) return false;
        if (SquareIsAttackedBy(5, row, Opponent(player))) return false;
        if (SquareIsAttackedBy(6, row, Opponent(player))) return false;

        return true;
    }

    public bool CanCastleQueenside(string player)
    {
        int row = (player == "white") ? 0 : 7;

        GameObject king = GetPosition(4, row);
        if (king == null || king.name != player + "_king") return false;
        if (HasMoved(king)) return false;

        if (GetUnmovedRook(0, row, player) == null) return false;

        if (GetPosition(1, row) != null || GetPosition(2, row) != null || GetPosition(3, row) != null) return false;

        if (IsInCheck(player)) return false;
        if (SquareIsAttackedBy(3, row, Opponent(player))) return false;
        if (SquareIsAttackedBy(2, row, Opponent(player))) return false;

        return true;
    }

    private bool SquareIsAttackedBy(int tx, int ty, string attackingPlayer)
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                GameObject obj = positions[x, y];
                if (obj != null && obj.name.StartsWith(attackingPlayer) && PieceAttacksSquare(obj, tx, ty))
                    return true;
            }
        return false;
    }

    private string Opponent(string player) => player == "white" ? "black" : "white";

    public void PromotePawn(GameObject pawn)
    {
        pawnToPromote = pawn;
        bool isWhite = pawn.name.Contains("white");

        if (promoQueenImage  != null) promoQueenImage.sprite  = isWhite ? white_queen_sprite  : black_queen_sprite;
        if (promoRookImage   != null) promoRookImage.sprite   = isWhite ? white_rook_sprite   : black_rook_sprite;
        if (promoBishopImage != null) promoBishopImage.sprite = isWhite ? white_bishop_sprite : black_bishop_sprite;
        if (promoKnightImage != null) promoKnightImage.sprite = isWhite ? white_knight_sprite : black_knight_sprite;

        promotionMenu.SetActive(true);
    }

    public void PromoteToQueen()  { ReplacePawn("queen");  }
    public void PromoteToRook()   { ReplacePawn("rook");   }
    public void PromoteToBishop() { ReplacePawn("bishop"); }
    public void PromoteToKnight() { ReplacePawn("knight"); }

    void ReplacePawn(string pieceType)
    {
        Chessman cm = pawnToPromote.GetComponent<Chessman>();
        int x = cm.GetXBoard();
        int y = cm.GetYBoard();
        string player = cm.name.Contains("white") ? "white" : "black";

        Destroy(pawnToPromote);

        GameObject newPiece = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman newCm = newPiece.GetComponent<Chessman>();
        newCm.name = player + "_" + pieceType;
        newCm.SetXBoard(x);
        newCm.SetYBoard(y);
        newCm.Activate();
        SetPosition(newPiece);

        promotionMenu.SetActive(false);
    }
}