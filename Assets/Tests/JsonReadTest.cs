using NUnit.Framework;
using Services;
using UnityEngine;

namespace Tests
{
    public class JsonReadTest : MonoBehaviour
    {
        private DataAccessJsonService _dataAccessJsonService;

        [SetUp]
        public void SetUp()
        {
            var testPath = "TestJson/";
            _dataAccessJsonService = new DataAccessJsonService(testPath);
        }

        [Test]
        public void ReadJsonTest()
        {
            // 存在するjsonファイル
            var json = _dataAccessJsonService.ReadJsonFile("test_get_stage_info_1");
            Assert.IsTrue(json != null);
            
            // 存在しないjsonファイル
            json = _dataAccessJsonService.ReadJsonFile("testtesttest");
            Assert.IsTrue(json == null);
        }

        [Test]
        public void ConvertResponse()
        {
            // jsonデータを取得できた場合: 正常にデータ取得できる
            var json = _dataAccessJsonService.ReadJsonFile("test_get_stage_info_1");
            var response = _dataAccessJsonService.ConvertResponse<JsonSchema.Response.StageDetailsInfo>(json);
            Assert.IsTrue(response != null);
            Assert.AreEqual(15, response.enemyCount);
            Assert.AreEqual("テスト", response.detail);

            // 変換する型が誤っている場合: デフォルト値が挿入される
            json = _dataAccessJsonService.ReadJsonFile("test_get_stage_info_2");
            response = _dataAccessJsonService.ConvertResponse<JsonSchema.Response.StageDetailsInfo>(json);
            Assert.AreEqual(0, response.enemyCount);
            Assert.AreEqual(null, response.detail);
            
            // jsonデータの型が謝っている場合: デフォルト値が挿入される
            json = _dataAccessJsonService.ReadJsonFile("test_get_stage_info_3");
            response = _dataAccessJsonService.ConvertResponse<JsonSchema.Response.StageDetailsInfo>(json);
            Assert.AreEqual(0, response.enemyCount);
            Assert.AreEqual("", response.detail); // nullじゃなく空になるっぽい
            
            // jsonデータを空で渡した場合: responseがnullで返る
            response = _dataAccessJsonService.ConvertResponse<JsonSchema.Response.StageDetailsInfo>(null);
            Assert.IsTrue(response == null);
        }
    }
}
