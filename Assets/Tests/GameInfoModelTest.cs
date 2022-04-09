using UnityEngine;
using NUnit.Framework;
using Scenes.Game.Model;
using Assert = UnityEngine.Assertions.Assert;
using Services;
using Utils;

namespace Tests
{
    public class GameInfoModelTest
    {
        private GameInfoModel _gameInfoModel;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Register<IDataAccessService>(new DataAccessJsonService());
            _gameInfoModel = new GameInfoModel();
        }

        [Test]
        public void RemainEnemyCountTest()
        {
            // 敵を10体生成
            _gameInfoModel.SetGenerateEnemyCount(10);
            Assert.IsTrue(Mathf.Abs(1.0f - _gameInfoModel.GetRemainEnemyRatio()) < GameConst.FloatEpsilon);

            // 1体ずつ減らす
            for (var i = 0; i < 10; i++)
            {
                _gameInfoModel.DecrementEnemyCount();
                var expected = 1.0f - 0.1f * (i + 1); // ratioは0.1ずつ減る
                Assert.IsTrue(Mathf.Abs(expected - _gameInfoModel.GetRemainEnemyRatio()) < GameConst.FloatEpsilon);
            }
            
            // 生成数以上減らしても0のまま
            _gameInfoModel.DecrementEnemyCount();
            Assert.IsTrue(Mathf.Abs(0.0f - _gameInfoModel.GetRemainEnemyRatio()) < GameConst.FloatEpsilon);
        }

        [Test]
        public void MaxBreakCountTest()
        {
            var enemyScore = new JsonSchema.Response.EnemyScore();
            enemyScore.perfectScore = 500;
            enemyScore.greatScore = 300;
            enemyScore.goodScore = 100;
            
            // 999回敵を倒す
            Assert.AreEqual(0, _gameInfoModel.GetTotalBreakEnemyCount());
            for (var i = 0; i < 999; i++)
            {
                _gameInfoModel.UpdateScoreInfoByHitType(GameConst.EnemyHitTypePerfect, enemyScore);
            }
            Assert.AreEqual(999, _gameInfoModel.GetTotalBreakEnemyCount());
            
            // それ以上倒しても増えないこと
            _gameInfoModel.UpdateScoreInfoByHitType(GameConst.EnemyHitTypePerfect, enemyScore);
            Assert.IsTrue(_gameInfoModel.GetTotalBreakEnemyCount() == 999);
        }
        
        [Test]
        public void MaxScoreInfoTest()
        {
            // 倒した数が99999999を超えないこと
            var enemyScore = new JsonSchema.Response.EnemyScore();
            enemyScore.perfectScore = 500;
            enemyScore.greatScore = 300;
            enemyScore.goodScore = 100;
            
            for (var i = 0; i < 1000; i++)
            {
                _gameInfoModel.UpdateScoreInfoByHitType(GameConst.EnemyHitTypePerfect, enemyScore);
                _gameInfoModel.UpdateScoreInfoByHitType(GameConst.EnemyHitTypeGreat, enemyScore);
                _gameInfoModel.UpdateScoreInfoByHitType(GameConst.EnemyHitTypeGood, enemyScore);
            }

            var scoreInfo = _gameInfoModel.GetResultScoreInfo();
            Assert.AreEqual(999, scoreInfo.PerfectCount);
            Assert.AreEqual(999, scoreInfo.GreatCount);
            Assert.AreEqual(999, scoreInfo.GoodCount);
            Assert.AreEqual(999, scoreInfo.GameTotalCount);
            Assert.AreEqual(999, scoreInfo.ResultTotalCount);
            Assert.AreEqual(500000, scoreInfo.PerfectTotalScore);
            Assert.AreEqual(300000, scoreInfo.GreatTotalScore);
            Assert.AreEqual(100000, scoreInfo.GoodTotalScore);
            Assert.AreEqual(900000, scoreInfo.GameTotalScore);
            Assert.AreEqual(900000, scoreInfo.ResultTotalScore);
            
            // スコアが99999999を超えないこと
            enemyScore = new JsonSchema.Response.EnemyScore();
            enemyScore.perfectScore = 5000000;
            enemyScore.greatScore = 3000000;
            enemyScore.goodScore = 1000000;
            
            for (var i = 0; i < 1000; i++)
            {
                _gameInfoModel.UpdateScoreInfoByHitType(GameConst.EnemyHitTypePerfect, enemyScore);
                _gameInfoModel.UpdateScoreInfoByHitType(GameConst.EnemyHitTypeGreat, enemyScore);
                _gameInfoModel.UpdateScoreInfoByHitType(GameConst.EnemyHitTypeGood, enemyScore);
            }
            
            scoreInfo = _gameInfoModel.GetResultScoreInfo();
            Assert.AreEqual(999, scoreInfo.PerfectCount);
            Assert.AreEqual(999, scoreInfo.GreatCount);
            Assert.AreEqual(999, scoreInfo.GoodCount);
            Assert.AreEqual(999, scoreInfo.GameTotalCount);
            Assert.AreEqual(999, scoreInfo.ResultTotalCount);
            Assert.AreEqual(999999, scoreInfo.PerfectTotalScore);
            Assert.AreEqual(999999, scoreInfo.GreatTotalScore);
            Assert.AreEqual(999999, scoreInfo.GoodTotalScore);
            Assert.AreEqual(999999, scoreInfo.GameTotalScore);
            Assert.AreEqual(999999, scoreInfo.ResultTotalScore);
        }
        
        [TestCase("1.00", 30.0f)]
        [TestCase("1.40", 28.0f)]
        [TestCase("1.80", 26.0f)]
        [TestCase("2.00", 25.2f)]
        [TestCase("2.00", 25.0f)]
        [TestCase("2.16", 24.2f)]
        [TestCase("2.20", 24.0f)]
        public void TimeScaleTest(string expected, float time)
        {
            // NormalTime=30.0f, BonusTime=25.0f として計算
            var scale = _gameInfoModel.CalculateTimeScale(30.0f, 25.0f, 1.0f, time);
            Assert.AreEqual(expected, scale.ToString("f2"));
        }
    }
}
