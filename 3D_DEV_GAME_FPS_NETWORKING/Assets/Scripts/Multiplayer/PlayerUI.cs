using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

/// <summary>
/// Player UI. Constraint the UI to follow a PlayerManager GameObject in the world,
/// Affect a slider and text to display Player's name and health
/// </summary>
public class PlayerUI : MonoBehaviour
{
    #region Private Fields

    [Tooltip("Pixel offset from the player target")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    [Tooltip("UI Text to display Player's Name")]
    [SerializeField]
    private Text playerNameText;

    private PlayerManager target;
    private Transform targetTransform;
    private Renderer targetRenderer;
    private CanvasGroup canvasGroup;

    private float characterControllerHeight;
    private Vector3 targetPosition;

    #endregion

    #region MonoBehaviour Messages

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase
    /// </summary>
    void Awake()
    {

        canvasGroup = this.GetComponent<CanvasGroup>();

        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    /// <summary>
    /// MonoBehaviour method called after all Update functions have been called. This is useful to order script execution.
    /// In our case since we are following a moving GameObject, we need to proceed after the player was moved during a particular frame.
    /// </summary>
    void LateUpdate()
    {
        if (targetRenderer != null)
        {
            this.canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }

        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Assigns a Player Target to Follow and represent.
    /// </summary>
    /// <param name="target">Target.</param>
    public void SetTarget(PlayerManager target)
    {
        if (target == null)
        {
            Debug.LogError("Missing target");
            return;
        }

        // Cache references for efficiency because we are going to reuse them.
        this.target = target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponentInChildren<Renderer>();

        CharacterController _characterController = this.target.GetComponent<CharacterController>();

        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null)
        {
            characterControllerHeight = _characterController.height;
        }

        if (playerNameText != null)
        {
            playerNameText.text = this.target.photonView.Owner.NickName;
        }
    }

    public Text getPlayerName()
    {
        return playerNameText;
    }
    #endregion
}
