namespace Services
{
    /// <summary>
    /// Audio関連管理インターフェイス
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// 効果音再生
        /// </summary>
        /// <param name="fileName"></param>
        public void PlayOneShot(string fileName);

        /// <summary>
        /// BGM再生
        /// </summary>
        /// <param name="fileName"></param>
        public void PlayBGM(string fileName);

        /// <summary>
        /// BGM停止
        /// </summary>
        public void StopBGM();

        /// <summary>
        /// SEピッチ変更
        /// </summary>
        /// <param name="pitch">ピッチ値</param>
        public void ChangeSePitch(float pitch);
        
        /// <summary>
        /// BGMボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeBgmVolume(float volume);
        
        /// <summary>
        /// SEボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeSeVolume(float volume);
    }
}
