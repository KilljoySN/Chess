using UnityEngine;

/// <summary>
/// Singleton that survives scene loads.
/// Attach to a persistent GameObject (e.g. "SkinManager") in your Main Menu scene.
/// Assign your SkinData assets to the availableSkins array in the Inspector.
/// </summary>
public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }

    [Tooltip("All skin sets available in the game. Index 0 is the default.")]
    public SkinData[] availableSkins;

    private int currentSkinIndex = 0;

    /// <summary>The currently active skin.</summary>
    public SkinData CurrentSkin
    {
        get
        {
            if (availableSkins == null || availableSkins.Length == 0) return null;
            return availableSkins[currentSkinIndex];
        }
    }

    // ---------------------------------------------------------------
    // Lifecycle
    // ---------------------------------------------------------------

    private void Awake()
    {
        // Singleton – survive scene loads
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Restore the player's last choice
        currentSkinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);
        ClampIndex();
    }

    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    // ---------------------------------------------------------------
    // Private helpers
    // ---------------------------------------------------------------

    /// <summary>Keeps currentSkinIndex inside [0, availableSkins.Length - 1].</summary>
    private void ClampIndex()
    {
        if (availableSkins == null || availableSkins.Length == 0)
        {
            currentSkinIndex = 0;
            return;
        }
        currentSkinIndex = Mathf.Clamp(currentSkinIndex, 0, availableSkins.Length - 1);
    }

    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    /// <summary>Returns the number of skins registered.</summary>
    public int SkinCount => availableSkins != null ? availableSkins.Length : 0;

    /// <summary>Returns the index of the currently selected skin.</summary>
    public int CurrentIndex => currentSkinIndex;

    /// <summary>
    /// Select a skin by index, save the preference, and immediately
    /// re-skin every Chessman currently in the scene.
    /// </summary>
    public void SelectSkin(int index)
    {
        if (availableSkins == null || index < 0 || index >= availableSkins.Length)
        {
            Debug.LogWarning($"[SkinManager] Invalid skin index: {index}");
            return;
        }

        currentSkinIndex = index;
        PlayerPrefs.SetInt("SelectedSkinIndex", index);
        PlayerPrefs.Save();

        ApplySkinToAllPieces();
    }

    /// <summary>
    /// Push the current skin onto every Chessman in the scene.
    /// Called automatically by SelectSkin and by Chessman.Activate().
    /// </summary>
    public void ApplySkinToAllPieces()
    {
        if (CurrentSkin == null) return;

        Chessman[] pieces = FindObjectsByType<Chessman>(FindObjectsSortMode.None);
        foreach (Chessman piece in pieces)
        {
            piece.ApplySkin(CurrentSkin);
        }
    }
}