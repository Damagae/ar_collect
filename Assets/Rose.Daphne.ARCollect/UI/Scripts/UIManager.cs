using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rose.Daphne.ARCollect.UI
{
    /// <summary>
    /// Manage UI on screen space.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Dialog")]
        [SerializeField]
        private CanvasGroup dialogGroup = null;

        [SerializeField]
        private TMPro.TMP_Text dialogTextReceiver = null;

        [SerializeField]
        private Image dialogIconReceiver = null;

        [SerializeField]
        private Button dialogActionButton = null;

        [SerializeField]
        private TMPro.TMP_Text dialogActionButtonText = null;

        [Header("Mission")]
        [SerializeField]
        private CanvasGroup missionGroup = null;

        [SerializeField]
        private TMPro.TMP_Text missionTextReceiver = null;

        [SerializeField]
        private TMPro.TMP_Text partsCountText = null;

        [Header("Message")]
        [SerializeField]
        private CanvasGroup messageGroup = null;

        [SerializeField]
        private TMPro.TMP_Text messageTextReceiver = null;

        [SerializeField]
        private GameObject loader = null;

        internal static UIManager Instance { private set; get; } = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            DeactivateGroup(dialogGroup);
            DeactivateGroup(missionGroup);
            ShowMessage("Scanning Space...", true);
        }

        private void DeactivateGroup(CanvasGroup group)
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
        }

        private void ActivateGroup(CanvasGroup group)
        {
            group.alpha = 1f;
            group.blocksRaycasts = true;
        }

        #region Message
        public void ShowMessage(string message, bool loaderActive = false)
        {
            ActivateGroup(messageGroup);
            messageTextReceiver.text = message;
            loader.SetActive(loaderActive);
        }

        public void HideMessage()
        {
            DeactivateGroup(messageGroup);
        }
        #endregion

        #region Dialog
        private int dialogIndex = 0;
        private string[] currentDialog = null;
        private Action dialogCallback = null;
        private string dialogButtonFinalLabel = default;

        /// <summary>
        /// Start a dialog that can be made of one or many messages.
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="finalLabel"></param>
        /// <param name="callback"></param>
        public void ShowDialog(string[] messages, string finalLabel = default, Action callback = null)
        {
            if (messages.Length == 0)
                return;

            ActivateGroup(dialogGroup);

            currentDialog = messages;
            dialogTextReceiver.text = messages[0];
            if (messages.Length > 1)
            {
                dialogActionButtonText.text = "Next";
                dialogActionButton.onClick.AddListener(ShowNextDialog);
                dialogCallback = callback;
                dialogButtonFinalLabel = finalLabel;
            }
            else
            {
                dialogActionButtonText.text = finalLabel != default ? finalLabel : "Close";
                dialogActionButton.onClick.AddListener(HideDialog);

                if (callback != null)
                {
                    dialogActionButton.onClick.AddListener(() => callback());
                }
            }
            
        }

        private void ShowNextDialog()
        {
            ++dialogIndex;
            dialogTextReceiver.text = currentDialog[dialogIndex];

            if (dialogIndex + 1 == currentDialog.Length)
            {
                dialogActionButton.onClick.RemoveAllListeners();
                dialogActionButtonText.text = dialogButtonFinalLabel != default ? dialogButtonFinalLabel : "Close";
                dialogActionButton.onClick.AddListener(HideDialog);

                if (dialogCallback != null)
                {
                    dialogActionButton.onClick.AddListener(() => dialogCallback());
                }

            }
        }

        public void HideDialog()
        {
            dialogIndex = 0;
            currentDialog = null;
            dialogActionButton.onClick.RemoveAllListeners();
            DeactivateGroup(dialogGroup);
        }
        #endregion

        #region Mission
        public void ShowMission()
        {
            ActivateGroup(missionGroup);
            partsCountText.text = "0";

        }

        public void UpdateParts(int count)
        {
            partsCountText.text = count.ToString();
        }

        public void HideMission()
        {
            DeactivateGroup(missionGroup);
        }
        #endregion
    }
}

