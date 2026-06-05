using UnityEngine;

/// <summary>
/// Create one asset per skin via Assets > Create > Chess > Skin.
/// Assign all 12 piece sprites and give it a display name.
/// </summary>
[CreateAssetMenu(fileName = "NewSkin", menuName = "Chess/Skin")]
public class SkinData : ScriptableObject
{
    [Tooltip("Display name shown in the Skin Menu.")]
    public string skinName = "Default";

    [Tooltip("Optional thumbnail shown in the Skin Menu selector.")]
    public Sprite previewSprite;

    [Header("White Pieces")]
    public Sprite white_pawn;
    public Sprite white_rook;
    public Sprite white_knight;
    public Sprite white_bishop;
    public Sprite white_queen;
    public Sprite white_king;

    [Header("Black Pieces")]
    public Sprite black_pawn;
    public Sprite black_rook;
    public Sprite black_knight;
    public Sprite black_bishop;
    public Sprite black_queen;
    public Sprite black_king;

    /// <summary>Returns the correct sprite for a piece by its GameObject name.</summary>
    public Sprite GetSprite(string pieceName)
    {
        switch (pieceName)
        {
            case "white_pawn": return white_pawn;
            case "white_rook": return white_rook;
            case "white_knight": return white_knight;
            case "white_bishop": return white_bishop;
            case "white_queen": return white_queen;
            case "white_king": return white_king;

            case "black_pawn": return black_pawn;
            case "black_rook": return black_rook;
            case "black_knight": return black_knight;
            case "black_bishop": return black_bishop;
            case "black_queen": return black_queen;
            case "black_king": return black_king;

            default: return null;
        }
    }
}