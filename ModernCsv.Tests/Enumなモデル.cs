using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModernCsv.Tests
{
    public enum Race
    {
        Normal,
        Developer,
        Money
    }

    public class EnumModel :ModelBase
    {
        public static ValidationResult RaceValidation(int value, ValidationContext validationContext)
        {
            return ParserHelper.IntToEnumValidation<EnumModel>(typeof(Race), value, validationContext);
        }

        public string Name { get; set; }
        public Race EnumRace { get; set; }
        
        [CustomValidation(typeof(EnumModel), "RaceValidation")]
        public int IntRace { get; set; }
    }

    [TestClass]
    public class Enumなモデル
    {
        [TestMethod]
        public void データ1行でEnumRace()
        {
            string text =
@"Name	EnumRace
takepara	1
";
            var results = Parser.Parse<EnumModel>(text);

            Assert.AreEqual(1, results.Count);

            var model = results.First();
            Assert.AreEqual("takepara", model.Name);
            Assert.AreEqual(Race.Developer, model.EnumRace);
        }

        [TestMethod]
        public void データ1行でEnumRaceの無効な値()
        {
            string text =
@"Name	EnumRace
takepara	10
";
            var results = Parser.Parse<EnumModel>(text);

            Assert.AreEqual(1, results.Count);

            var model = results.First();

            // これはすり抜ける。
            Assert.IsTrue(model.IsValid);
        }

        [TestMethod]
        public void データ1行でIntRaceの無効な値()
        {
            string text =
@"Name	IntRace
takepara	10
";
            var results = Parser.Parse<EnumModel>(text);

            Assert.AreEqual(1, results.Count);

            var model = results.First();

            // これはCustomValidationで捕まえれる。
            Assert.IsFalse(model.IsValid);
        }
    }
}
