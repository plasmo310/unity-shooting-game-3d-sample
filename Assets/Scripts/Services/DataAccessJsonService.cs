using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// データアクセス管理クラス
    /// jsonファイル読み込み用
    /// </summary>
    public class DataAccessJsonService : IDataAccessService
    {
        /// <summary>
        /// Resources配下のjsonファイルパス
        /// </summary>
        private const string JsonFilePath = "Json/";
        private readonly string _jsonFilePath;

        public DataAccessJsonService(string path = null)
        {
            _jsonFilePath = path ?? JsonFilePath;
        }

        /// <summary>
        /// ステージ詳細情報(キャッシュ情報)
        /// </summary>
        private Dictionary<string, JsonSchema.Response.StageDetailsInfo> _cachedStageDetailInfos;

        /// <summary>
        /// ステージ詳細情報取得
        /// </summary>
        /// <param name="mode">モード</param>
        /// <param name="level">レベル</param>
        public JsonSchema.Response.StageDetailsInfo GetStageDetailInfo(int mode, int level)
        {
            // キャッシュしていなかったらjsonを読み込む
            if (_cachedStageDetailInfos == null)
            {
                var fileName = "mst_stage_detail";
                var json = ReadJsonFile(fileName);
                var mstStageDetailInfos = ConvertResponse<JsonSchema.Response.StageDetail>(json);
                if (mstStageDetailInfos?.stage_details == null || mstStageDetailInfos.stage_details.Length == 0)
                {
                    Debug.LogError("MstStageDetail not found data!!");
                    return null;
                }

                // キャッシュ情報を生成
                _cachedStageDetailInfos = new Dictionary<string, JsonSchema.Response.StageDetailsInfo>();
                foreach (var detail in mstStageDetailInfos.stage_details)
                {
                    var addKey = CreateGameModeLevelKey(detail.mode, detail.level);
                    _cachedStageDetailInfos.Add(addKey, detail);
                }
            }

            // キャッシュ情報から対象のデータを返却する
            var key = CreateGameModeLevelKey(mode, level);
            if (!_cachedStageDetailInfos.ContainsKey(key))
            {
                Debug.Log("MstStageDetail not found key: " + key);
                return null;
            }

            // 敵の生成数を付与して返却
            var response = _cachedStageDetailInfos[key];
            var generateInfo = GetEnemyGenerateInfo(mode, level);
            response.enemyCount = generateInfo.generateCount;
            return response;
        }
        
        /// <summary>
        /// 敵の生成情報(キャッシュ情報)
        /// </summary>
        private Dictionary<string, JsonSchema.Response.EnemyGenerateInfo> _cachedEnemyGenerateInfos;
        
        /// <summary>
        /// 敵の生成情報取得
        /// </summary>
        /// <param name="mode">モード</param>
        /// <param name="level">レベル</param>
        public JsonSchema.Response.EnemyGenerateInfo GetEnemyGenerateInfo(int mode, int level)
        {
            // キャッシュしていなかったらjsonを読み込む
            if (_cachedEnemyGenerateInfos == null)
            {
                var fileName = "mst_enemy_generate";
                var json = ReadJsonFile(fileName);
                var mstEnemyGenerateInfo = ConvertResponse<JsonSchema.Response.EnemyGenerate>(json);
                if (mstEnemyGenerateInfo?.generate_infos == null || mstEnemyGenerateInfo.generate_infos.Length == 0)
                {
                    Debug.LogError("MstEnemyGenerateInfo not found data!!");
                    return null;
                }
                
                // キャッシュ情報を生成
                _cachedEnemyGenerateInfos = new Dictionary<string, JsonSchema.Response.EnemyGenerateInfo>();
                foreach (var generateInfo in mstEnemyGenerateInfo.generate_infos)
                {
                    var addKey = CreateGameModeLevelKey(generateInfo.mode, generateInfo.level);
                    _cachedEnemyGenerateInfos.Add(addKey, generateInfo);
                }
            }
            
            // キャッシュ情報から対象のデータを返却する
            var key = CreateGameModeLevelKey(mode, level);
            if (!_cachedEnemyGenerateInfos.ContainsKey(key))
            {
                Debug.Log("MstEnemyGenerateInfo not found key: " + key);
                return null;
            }
            return _cachedEnemyGenerateInfos[key];
        }

        /// <summary>
        /// 敵のスコア情報(キャッシュ情報)
        /// </summary>
        private JsonSchema.Response.EnemyScore _cachedEnemyScoreInfo;
        
        /// <summary>
        /// 敵のスコア情報取得
        /// </summary>
        public JsonSchema.Response.EnemyScore GetEnemyScoreInfo()
        {
            if (_cachedEnemyScoreInfo == null)
            {
                var fileName = "mst_enemy_score";
                var json = ReadJsonFile(fileName);
                var mstEnemyScore = ConvertResponse<JsonSchema.Response.EnemyScore>(json);
                if (mstEnemyScore == null)
                {
                    Debug.LogError("MstEnemyScore not found data!!");
                    return null;
                }
                
                // キャッシュ情報を生成
                _cachedEnemyScoreInfo = mstEnemyScore;
            }
            // キャッシュ情報を返却する
            return _cachedEnemyScoreInfo;
        }

        /// <summary>
        /// スコア時間ボーナス情報(キャッシュ情報)
        /// </summary>
        private Dictionary<string, JsonSchema.Response.ScoreTimeBonusInfo> _cachedScoreTimeBonusInfos;
        
        /// <summary>
        /// スコア時間ボーナス情報取得
        /// </summary>
        public JsonSchema.Response.ScoreTimeBonusInfo GetScoreTimeBonusInfo(int mode, int level)
        {
            // キャッシュしていなかったらjsonを読み込む
            if (_cachedScoreTimeBonusInfos == null)
            {
                var fileName = "mst_score_time_bonus";
                var json = ReadJsonFile(fileName);
                var mstScoreTimeBonus = ConvertResponse<JsonSchema.Response.ScoreTimeBonus>(json);
                if (mstScoreTimeBonus?.time_bonus_infos == null || mstScoreTimeBonus.time_bonus_infos.Length == 0)
                {
                    Debug.LogError("MstScoreTimeBonus not found data!!");
                    return null;
                }
                
                // キャッシュ情報を生成
                _cachedScoreTimeBonusInfos = new Dictionary<string, JsonSchema.Response.ScoreTimeBonusInfo>();
                foreach (var timeBonusInfo in mstScoreTimeBonus.time_bonus_infos)
                {
                    var addKey = CreateGameModeLevelKey(timeBonusInfo.mode, timeBonusInfo.level);
                    _cachedScoreTimeBonusInfos.Add(addKey, timeBonusInfo);
                }
            }
            
            // キャッシュ情報から対象のデータを返却する
            var key = CreateGameModeLevelKey(mode, level);
            if (!_cachedScoreTimeBonusInfos.ContainsKey(key))
            {
                Debug.Log("MstScoreTimeBonus not found key: " + key);
                return null;
            }

            return _cachedScoreTimeBonusInfos[key];
        }
        
        /// <summary>
        /// ランダム名称情報(キャッシュ情報)
        /// </summary>
        private JsonSchema.Response.RandomNameInfo _cachedRandomNameInfo;
        
        /// <summary>
        /// ランダム名称情報取得
        /// </summary>
        public JsonSchema.Response.RandomNameInfo GetRandomNameInfo()
        {
            if (_cachedRandomNameInfo == null)
            {
                var fileName = "mst_random_name";
                var json = ReadJsonFile(fileName);
                var mstRandomName = ConvertResponse<JsonSchema.Response.RandomNameInfo>(json);
                if (mstRandomName == null)
                {
                    Debug.LogError("MstRandomName not found data!!");
                    return null;
                }
                
                // キャッシュ情報を生成
                _cachedRandomNameInfo = mstRandomName;
            }
            // キャッシュ情報を返却する
            return _cachedRandomNameInfo;
        }
        
        // ---------- 共通処理 ----------
        
        /// <summary>
        /// ゲームモードとレベルからキーを作成
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="level"></param>
        private static string CreateGameModeLevelKey(int mode, int level)
        {
            return mode + "_" + level; // 【mode】_【level】
        }

        /// <summary>
        /// jsonファイル読み込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>読み込んだjson文字列</returns>
        public string ReadJsonFile(string fileName)
        {
            try
            {
                // Resources配下のファイルを読み込む
                return Resources.Load<TextAsset>(_jsonFilePath + fileName)?.ToString();
            }
            catch (Exception e)
            {
                Debug.Log("*** json file read error: " + e);
                return null;
            }
        }

        /// <summary>
        /// 読み込んだjson文字列をレスポンスに変換する
        /// </summary>
        /// <param name="json">json文字列</param>
        /// <typeparam name="T">変換するレスポンス型</typeparam>
        /// <returns>レスポンス型データ</returns>
        public T ConvertResponse<T>(string json)
        {
            var response = default(T);
            try
            {
                response = JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.Log("*** json decode error: " + e);
            }
            return response;
        }
    }
}
