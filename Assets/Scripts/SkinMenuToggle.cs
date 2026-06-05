using UnityEngine;

/// <summary>
/// Attach this to a "Skins" button in your GameScene.
/// It opens and closes the SkinMenu overlay while the game is paused.
/// </summary>
public class SkinMenuToggle : MonoBehaviour
{
    [Tooltip("The SkinMenu GameObject in your GameScene canvas.")]
    public GameObject skinMenuObject;

    private Game game;
    private bool skinMenuOpen = false;

    private void Start()
    {
        GameObject gc = GameObject.FindGameObjectWithTag("GameController");
        if (gc != null) game = gc.GetComponent<Game>();

        // Make sure the menu starts hidden
        if (skinMenuObject != null)
            skinMenuObject.SetActive(false);
    }

    public void ToggleSkinMenu()
    {
        if (game == null) return;

        skinMenuOpen = !skinMenuOpen;

        if (skinMenuOpen)
        {
            // Pause the game and show the skin menu
            game.PauseGame();
            skinMenuObject.SetActive(true);
        }
        else
        {
            // Hide the skin menu and resume
            skinMenuObject.SetActive(false);
            game.ResumeGame();
        }
    }

    /// <summary>Called by SkinMenu cards' Select buttons after a skin is chosen.</summary>
    public void OnSkinChosen()
    {
        // Close the menu and resume automatically after picking
        skinMenuObject.SetActive(false);
        skinMenuOpen = false;
        game?.ResumeGame();
    }
}
