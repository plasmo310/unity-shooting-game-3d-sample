using NUnit.Framework;
using Utils;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests
{
    public class DateUtilTest
    {
        [TestCase("00:00", 0.0f)]
        [TestCase("00:00", 0.3f)]
        [TestCase("00:30", 30.0f)]
        [TestCase("01:00", 60.0f)]
        [TestCase("01:10", 70.0f)]
        [TestCase("10:00", 600.0f)]
        [TestCase("51:00", 3060.7f)]
        [TestCase("100:00", 6000.0f)] // 超えたら桁があふれるが通常はない
        public void ConvMmSs(string expected, float time)
        {
            Assert.AreEqual(expected, DateUtil.ConvTimeToMmSs(time));
        }
    }
}
