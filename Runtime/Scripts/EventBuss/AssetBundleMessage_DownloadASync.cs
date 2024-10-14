using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SangoUtils.Patchs_YooAsset
{
    public struct AssetBundleMessage_DownloadASync
    {
        /// <summary>
        /// Download Progress Callback.
        /// </summary>
        /// <param name="totalDownloadCount">Total counts of all files.</param>
        /// <param name="currentDownloadCount">Updating field.</param>
        /// <param name="totalDownloadBytes">Total count of all file size.</param>
        /// <param name="currentDownloadBytes">Updating field.</param>
        public delegate void OnDownloadProgressed(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes);

        /// <summary>
        /// Download Error Callback.
        /// </summary>
        /// <param name="fileName">The file name which not download correctly.</param>
        /// <param name="error">The error message.</param>
        public delegate void OnDownloadError(string fileName, string error);

        /// <summary>
        /// Download Done Callback.
        /// </summary>
        /// <param name="isSucceed">Is there no error happened.</param>
        public delegate void OnDownloadDone(bool isSucceed);

        /// <summary>
        /// The tag of YooAsset Collector, see YooAsset Editor Window.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The max file count of each downloading at the same time.
        /// </summary>
        public int DownloadingMaxNumber { get; set; }

        /// <summary>
        /// The try count of if one file is downloaded failed.
        /// </summary>
        public int FailedTryAgainCount { get; set; }

        /// <summary>
        /// After a long time, if one file still no response, we set it as downloaded failed.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Will receive them once the download async operation is started.
        /// </summary>
        public OnDownloadProgressed OnDownloadProgressedCallback { get; set; }

        /// <summary>
        /// If something error happened, will receive this message.
        /// </summary>
        public OnDownloadError OnDownloadErrorCallback { get; set; }

        /// <summary>
        /// If this download is over, this message will received.
        /// </summary>
        public OnDownloadDone OnDownloadDoneCallback { get; set; }
    }
}
