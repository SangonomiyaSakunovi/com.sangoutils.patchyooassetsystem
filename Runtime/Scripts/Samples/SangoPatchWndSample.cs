using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SangoUtils.Patchs_YooAsset
{
    public class SangoPatchWndSample : MonoBehaviour, ISangoPatchWnd
    {
        private SangoPatchRoot _sangoHotFixRoot;
        private Transform _messageBoxTrans;
        private TMP_Text _tips;

        private Action _clickMessageBoxOkCB;
        private Button _messageBoxOkBtn;
        private TMP_Text _messageBoxContent;

        public void OnInit(SangoPatchRoot root)
        {
            _sangoHotFixRoot = root;

            _messageBoxTrans = transform.Find("MessageBox");
            _tips = transform.Find("tips").GetComponent<TMP_Text>();

            _messageBoxOkBtn = _messageBoxTrans.Find("messageBoxOkBtn").GetComponent<Button>();
            _messageBoxContent = _messageBoxTrans.Find("messageBoxContent").GetComponent<TMP_Text>();

            _messageBoxTrans.gameObject.SetActive(false);
            _tips.SetText("欢迎使用热更新系统");
        }

        public void OnStart()
        {
            if(!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public void OnShowMessageBox(string content, Action onMessageBoxOKBtnClickedCB)
        {
            _messageBoxOkBtn.onClick.RemoveAllListeners();
            _messageBoxContent.SetText(content);
            _clickMessageBoxOkCB = onMessageBoxOKBtnClickedCB;
            _messageBoxOkBtn.onClick.AddListener(OnMessageBoxOKBtnClicked);
            _messageBoxTrans.gameObject.SetActive(true);
            _messageBoxTrans.SetAsLastSibling();
        }

        public void OnUpdateTips(string content)
        {
            _tips.SetText(content);
        }

        public void OnUpdateSliderValue(float value)
        {

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
    }
}