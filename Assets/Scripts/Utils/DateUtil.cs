using UnityEngine;

namespace Utils
{
    /// <summary>
    /// 日付関連共通クラス
    /// </summary>
    public class DateUtil
    {
        /// <summary>
        /// 時間をmm:ssの形で変換して返却する
        /// </summary>
        /// <param name="time">時間(秒数)</param>
        /// <returns>時間(mm:ss)</returns>
        public static string ConvTimeToMmSs(float time)
        {
            var minute = (int) Mathf.Floor(time / 60.0f);
            var seconds = (int) Mathf.Floor(time % 60.0f);
            return minute.ToString("00") + ":" + seconds.ToString("00");;
        }
    }
}
