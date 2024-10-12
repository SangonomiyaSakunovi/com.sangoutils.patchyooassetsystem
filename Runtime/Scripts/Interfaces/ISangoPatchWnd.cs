using System;

namespace SangoUtils.Patchs_YooAsset
{
    public interface ISangoPatchWnd
    {
        void OnInit(SangoPatchRoot root);
        void OnStart();
        void OnShowMessageBox(string content, Action onMessageBoxOKBtnClickedCB) ;
        void OnUpdateTips(string content) ;
        void OnUpdateSliderValue(float value) ;
        void OnEnd() ;
    }
}
