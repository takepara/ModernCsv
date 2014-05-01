using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace ModernCsv.Tests
{
    public class NullModel : ModelBase
    {
        public string Name { get; set; }
        public Nullable<int> Level { get; set; }
        public Nullable<DateTime> Birthday { get; set; }
    }

    public class NullAndAnnotationModel : ModelBase
    {
        [Required]
        public string Name { get; set; }
        public Nullable<int> Level { get; set; }
        public Nullable<DateTime> Birthday { get; set; }
    }

    [TestClass]
    public class Nullありモデル
    {
        [TestMethod]
        public void データ1行でNameだけ()
        {
            string text =
@"Name
takepara
";
            var results = Parser.Parse<NullModel>(text);

            Assert.AreEqual(1, results.Count);

            var model = results.First();
            Assert.AreEqual("takepara", model.Name);
            Assert.AreEqual(null, model.Level);
            Assert.AreEqual(null, model.Birthday);
        }

        [TestMethod]
        public void データ1行でLevelだけ()
        {
            string text =
@"Level
12
";
            var results = Parser.Parse<NullModel>(text);

            Assert.AreEqual(1, results.Count);

            var model = results.First();
            Assert.AreEqual(null, model.Name);
            Assert.AreEqual(12, model.Level);
            Assert.AreEqual(null, model.Birthday);
        }

        [TestMethod]
        public void データ1行でLevelだけだけどNameは必須()
        {
            string text =
@"Level
12
";
            var results = Parser.Parse<NullAndAnnotationModel>(text);

            Assert.AreEqual(1, results.Count);
            var model = results.First();

            Assert.IsFalse(model.IsValid);
        }
    }
}
