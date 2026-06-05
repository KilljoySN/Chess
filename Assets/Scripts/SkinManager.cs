using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }

    [Tooltip("All skin sets available in the game. Index 0 is the default.")]
    public SkinData[] availableSkins;

    private int currentSkinIndex = 0;

    public SkinData CurrentSkin
    {
        get
        {
            if (availableSkins == null || availableSkins.Length == 0) return null;
            return availableSkins[currentSkinIndex];
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentSkinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);
        ClampIndex();
    }

    private void ClampIndex()
    {
        if (availableSkins == null || availableSkins.Length == 0)
        {
            currentSkinIndex = 0;
            return;
        }
        currentSkinIndex = Mathf.Clamp(currentSkinIndex, 0, availableSkins.Length - 1);
    }

    public int SkinCount => availableSkins != null ? availableSkins.Length : 0;

    public int CurrentIndex => currentSkinIndex;

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