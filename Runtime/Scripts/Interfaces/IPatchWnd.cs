using System;

namespace SangoUtils.Patchs_YooAsset
{
    public interface IPatchWnd
    {
        void OnInit(PatchRoot root);
        void OnStart();
        void OnShowMessageBox(string content, Action onMessageBoxOKBtnClickedCB) ;
        void OnUpdateTips(string content) ;
        void OnUpdateSliderValue(float value) ;
        void OnEnd() ;
    }
}
