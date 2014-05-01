using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace ModernCsv.Tests
{
    public class SimpleModel : ModelBase
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class SimpleAndAnnotationModel : ModelBase
    {
        public string Name { get; set; }
        [Range(1,50)]
        public int Level { get; set; }
        public DateTime Birthday { get; set; }
    }

    [TestClass]
    public class 単純なモデル
    {
        [TestMethod]
        public void データ1行で全カラム()
        {
            string text =
@"Name	Level	Birthday
takepara	12	2000/1/1
";
            var results = Parser.Parse<SimpleModel>(text);

            Assert.AreEqual(1, results.Count);

            var model = results.First();
            Assert.AreEqual("takepara", model.Name);
            Assert.AreEqual(12, model.Level);
            Assert.AreEqual(DateTime.Parse("2000/1/1"), model.Birthday);
        }


        [TestMethod]
        public void データ3行で全カラム()
        {
            string text =
@"Name	Level	Birthday
takepara	12	2000/1/1
takewara	7	1900/1/1
takefara	21	1950/1/1
";
            var results = Parser.Parse<SimpleModel>(text);

            Assert.AreEqual(3, results.Count);

            var model = results.First();
            Assert.AreEqual("takepara", model.Name);
            Assert.AreEqual(12, model.Level);
            Assert.AreEqual(DateTime.Parse("2000/1/1"), model.Birthday);
        }

        [TestMethod]
        public void データ1行でNameだけ()
        {
            string text =
@"Name
takepara
";
            var results = Parser.Parse<SimpleModel>(text);

            Assert.AreEqual(1, results.Count);

            var model = results.First();
            Assert.AreEqual("takepara", model.Name);
            Assert.AreEqual(0, model.Level);
            Assert.AreEqual(default(DateTime), model.Birthday);
        }

        [TestMethod]
        public void データ1行でLevelだけだけどRange範囲外()
        {
            string text =
@"Level
51
";
            var results = Parser.Parse<NullAndAnnotationModel>(text);

            Assert.AreEqual(1, results.Count);
            var model = results.First();

            Assert.IsFalse(model.IsValid);
        }
    }
}
