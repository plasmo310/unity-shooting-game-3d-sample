using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Services
{
    /// <summary>
    /// Audio関連管理クラス
    /// </summary>
    public class AudioService : MonoBehaviour, IAudioService
    {
        /// <summary>
        /// Audioファイル格納パス
        /// </summary>
        private const string AudioPath = "Audio/";
        
        /// <summary>
        /// キャッシュしたAudioClip
        /// key: ファイル名
        /// </summary>
        private readonly IDictionary<string, AudioClip> _cachedAudioDictionary = new Dictionary<string, AudioClip>();
        
        /// <summary>
        /// AudioSource
        /// </summary>
        private AudioSource _seAudioSource;  // SE再生用
        private AudioSource _bgmAudioSource; // BGM再生用

        private void Awake()
        {
            _seAudioSource = gameObject.AddComponent<AudioSource>();
            _bgmAudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        /// <summary>
        /// 効果音再生
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        public void PlayOneShot(string fileName)
        {
            var audioClip = LoadAudioClip(fileName);
            var volume = _seVolume * GetVolume(fileName);
            _seAudioSource.PlayOneShot(audioClip, volume);
        }

        /// <summary>
        /// BGM再生
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        public void PlayBGM(string fileName)
        {
            var audioClip = LoadAudioClip(fileName);
            
            // 既に同じBDMが再生中ならそのまま
            if (_bgmAudioSource.clip == audioClip)
            {
                return;
            }

            // 再生中なら止める
            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.Stop();
            }

            _bgmAudioSource.clip = LoadAudioClip(fileName);
            _bgmAudioSource.volume = _bgmVolume * GetVolume(fileName);
            _bgmAudioSource.loop = true;
            _bgmAudioSource.Play();
        }

        /// <summary>
        /// BGM停止
        /// </summary>
        public void StopBGM()
        {
            // 再生中なら止める
            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.Stop();
            }
        }

        /// <summary>
        /// SEピッチ変更
        /// </summary>
        /// <param name="pitch">ピッチ値</param>
        public void ChangeSePitch(float pitch)
        {
            _seAudioSource.pitch = pitch;
        }
        
        /// <summary>
        /// ボリューム
        /// </summary>
        private float _bgmVolume = 1.0f;
        private float _seVolume = 1.0f;
        
        /// <summary>
        /// BGMボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeBgmVolume(float volume)
        {
            _bgmVolume = volume;
            
            // 再生中のBGM音量を変更
            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.volume = _bgmVolume * GetVolume(_bgmAudioSource.name);
            }
        }
        
        /// <summary>
        /// SEボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeSeVolume(float volume)
        {
            _seVolume = volume;
        }
        
        /// <summary>
        /// AudioClipの読み込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>AudioClip</returns>
        private AudioClip LoadAudioClip(string fileName)
        {
            // ファイル名をキーとしてキャッシュする
            if (!_cachedAudioDictionary.ContainsKey(fileName))
            {
                var audioClip = Resources.Load(AudioPath + fileName) as AudioClip;
                _cachedAudioDictionary.Add(fileName, audioClip);
            }
            return _cachedAudioDictionary[fileName];
        }

        // デフォルトVolume
        private const float DefaultVolume = 0.8f; // 調整可能にするため少し下げる

        /// <summary>
        /// 指定ファイルのボリューム取得
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>ボリューム値</returns>
        private float GetVolume(string fileName)
        {
            // 特定のファイルのボリューム調整を行う
            var adjustVolume = 1.0f;
            switch (fileName)
            {
                case GameConst.AudioNameSeClick:
                case GameConst.AudioNameSeCount:
                    adjustVolume = 0.8f;
                    break;
                case GameConst.AudioNameSeBeam:
                    adjustVolume = 0.35f;
                    break;
                case GameConst.AudioNameSeMachine:
                    adjustVolume = 0.2f;
                    break;
                case GameConst.AudioNameSeShot:
                    adjustVolume = 0.5f;
                    break;
                case GameConst.AudioNameSeLucky:
                    adjustVolume = 0.8f;
                    break;
            }
            return adjustVolume;
        }
    }
}
