namespace Services
{
    public interface IDataAccessService
    {
        public JsonSchema.Response.StageDetailsInfo GetStageDetailInfo(int mode, int level);
        public JsonSchema.Response.EnemyGenerateInfo GetEnemyGenerateInfo(int mode, int level);
        public JsonSchema.Response.EnemyScore GetEnemyScoreInfo();
        public JsonSchema.Response.ScoreTimeBonusInfo GetScoreTimeBonusInfo(int mode, int level);
        public JsonSchema.Response.RandomNameInfo GetRandomNameInfo();
    }
}
