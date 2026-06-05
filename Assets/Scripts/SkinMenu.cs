using UnityEngine;
using UnityEngine.UI;

public class SkinMenu : MonoBehaviour
{
    [Header("UI Container")]
    [Tooltip("RectTransform with a VerticalLayoutGroup or HorizontalLayoutGroup for the skin cards.")]
    public RectTransform previewContainer;

    [Header("Card Appearance")]
    public Vector2 cardSize = new Vector2(120f, 160f);
    public Color selectedColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color defaultColor = new Color(1f, 1f, 1f, 1f);

    [Header("Close Button (optional)")]
    [Tooltip("Wire up a Close/X button in the menu to call HideMenu().")]
    public Button closeButton;

    // Auto-found at runtime — no need to assign in Inspector
    private SkinMenuToggle toggle;

    private void OnEnable()
    {
        // Find the toggle on the button that opens this menu
        if (toggle == null)
            toggle = FindFirstObjectByType<SkinMenuToggle>();

        BuildCards();

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HideMenu);
        }
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        ClearCards();
        gameObject.SetActive(false);

        // Tell the toggle so the game resumes
        toggle?.OnSkinChosen();
    }

    private void BuildCards()
    {
        if (previewContainer == null)
        {
            Debug.LogWarning("[SkinMenu] previewContainer is not assigned.");
            return;
        }

        ClearCards();

        SkinManager sm = SkinManager.Instance;
        if (sm == null || sm.SkinCount == 0)
        {
            Debug.LogWarning("[SkinMenu] No SkinManager found or no skins registered.");
            return;
        }

        for (int i = 0; i < sm.SkinCount; i++)
        {
            SkinData skin = sm.availableSkins[i];
            int capturedIndex = i;

            // ?? Card root ??????????????????????????????????????????????????
            GameObject card = new GameObject("SkinCard_" + i,
                typeof(RectTransform), typeof(Image));
            card.transform.SetParent(previewContainer, false);

            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.sizeDelta = cardSize;

            Image cardBg = card.GetComponent<Image>();
            cardBg.color = (i == sm.CurrentIndex) ? selectedColor : defaultColor;

            // ?? Preview image ??????????????????????????????????????????????
            GameObject imgObj = new GameObject("Preview",
                typeof(RectTransform), typeof(Image));
            imgObj.transform.SetParent(card.transform, false);

            RectTransform imgRect = imgObj.GetComponent<RectTransform>();
            imgRect.anchorMin = new Vector2(0.1f, 0.35f);
            imgRect.anchorMax = new Vector2(0.9f, 0.95f);
            imgRect.offsetMin = Vector2.zero;
            imgRect.offsetMax = Vector2.zero;

            Image previewImg = imgObj.GetComponent<Image>();
            if (skin != null && skin.previewSprite != null)
            {
                previewImg.sprite = skin.previewSprite;
                previewImg.preserveAspect = true;
            }
            else
            {
                previewImg.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            }

            // ?? Skin name label ????????????????????????????????????????????
            GameObject labelObj = new GameObject("Label",
                typeof(RectTransform), typeof(Text));
            labelObj.transform.SetParent(card.transform, false);

            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0.2f);
            labelRect.anchorMax = new Vector2(1f, 0.4f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            Text label = labelObj.GetComponent<Text>();
            label.text = skin != null ? skin.skinName : "Skin " + i;
            label.alignment = TextAnchor.MiddleCenter;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 13;
            label.color = Color.black;

            // ?? Select button ??????????????????????????????????????????????
            GameObject btnObj = new GameObject("SelectButton",
                typeof(RectTransform), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(card.transform, false);

            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.1f, 0.02f);
            btnRect.anchorMax = new Vector2(0.9f, 0.20f);
            btnRect.offsetMin = Vector2.zero;
            btnRect.offsetMax = Vector2.zero;

            Image btnImg = btnObj.GetComponent<Image>();
            btnImg.color = new Color(0.2f, 0.5f, 1f, 1f);

            Button btn = btnObj.GetComponent<Button>();
            btn.targetGraphic = btnImg;
            btn.onClick.AddListener(() => OnSkinSelected(capturedIndex));

            // Button label
            GameObject btnLabelObj = new GameObject("ButtonLabel",
                typeof(RectTransform), typeof(Text));
            btnLabelObj.transform.SetParent(btnObj.transform, false);

            RectTransform btnLabelRect = btnLabelObj.GetComponent<RectTransform>();
            btnLabelRect.anchorMin = Vector2.zero;
            btnLabelRect.anchorMax = Vector2.one;
            btnLabelRect.offsetMin = Vector2.zero;
            btnLabelRect.offsetMax = Vector2.zero;

            Text btnLabel = btnLabelObj.GetComponent<Text>();
            btnLabel.text = (i == sm.CurrentIndex) ? "Selected" : "Select";
            btnLabel.alignment = TextAnchor.MiddleCenter;
            btnLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            btnLabel.fontSize = 12;
            btnLabel.color = Color.white;
        }
    }

    private void OnSkinSelected(int index)
    {
        SkinManager sm = SkinManager.Instance;
        if (sm == null) return;

        sm.SelectSkin(index);  // applies skin to all pieces instantly via ApplySkinToAllPieces()

        BuildCards();          // refresh card highlights / button labels

        // Auto-close after a short moment so the player can see the board update
        Invoke(nameof(HideMenu), 0.4f);
    }

    private void ClearCards()
    {
        if (previewContainer == null) return;
        for (int i = previewContainer.childCount - 1; i >= 0; i--)
            Destroy(previewContainer.GetChild(i).gameObject);
    }
}