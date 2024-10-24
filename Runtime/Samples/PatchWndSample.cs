using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SangoUtils.Patchs_YooAsset
{
    public class PatchWndSample : MonoBehaviour, IPatchWnd
    {
        private PatchRoot _patchRoot;
        private Transform _messageBoxTrans;

        private Action _clickMessageBoxOkCB;
        private Button _messageBoxOkBtn;
        private TMP_Text _messageBoxContent;

        public void OnInit(PatchRoot root)
        {
            _patchRoot = root;
            _messageBoxTrans.gameObject.SetActive(false);
        }

        public void OnStart()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public void OnEnd()
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        private void OnMessageBoxOKBtnClicked()
        {
            _clickMessageBoxOkCB?.Invoke();
            _messageBoxTrans.gameObject.SetActive(false);
        }

        public void OnMessageBoxEvent(PatchMessageBoxEventType messageType, Action onMessageBoxBtnTryAgainClickedCB, params string[] extensionDatas)
        {
            _messageBoxOkBtn.onClick.RemoveAllListeners();
            _clickMessageBoxOkCB = onMessageBoxBtnTryAgainClickedCB;

            switch (messageType)
            {
                case PatchMessageBoxEventType.PartFilesDownloadFailed:
                    _messageBoxContent.SetText("部分文件下载失败，是否重试？");
                    break;
            }

            _messageBoxOkBtn.onClick.AddListener(OnMessageBoxOKBtnClicked);
            _messageBoxTrans.gameObject.SetActive(true);
            _messageBoxTrans.SetAsLastSibling();
        }

        public void OnUpdateDownloadingProgress(int currentDownloadCount, int totalDownloadCount, long currentDownloadSizeBytes, long totalDownloadSizeBytes)
        {
            string currentSizeMB = (currentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (totalDownloadSizeBytes / 1048576f).ToString("f1");
            string tips = $"{currentDownloadCount}/{totalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
            //Show tips
        }
    }
}