using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monghost;
using Shouldly;

namespace UnitTests
{
    [TestClass]
    public class PluralizationTest
    {
        [TestMethod]
        public void ShouldPluralize()
        {
            var plural = Pluralization.Pluralize("cat");
            plural.ShouldBe("cats");
        }

        [TestMethod]
        public void ShouldPluralizeTch()
        {
            var plural = Pluralization.Pluralize("ditch");
            plural.ShouldBe("ditches");
        }

        [TestMethod]
        public void ShouldPluralizeO()
        {
            var plural = Pluralization.Pluralize("potato");
            plural.ShouldBe("potatoes");
        }

        [TestMethod]
        public void ShouldPluralizeS()
        {
            var plural = Pluralization.Pluralize("mess");
            plural.ShouldBe("messes");
        }

        [TestMethod]
        public void ShouldPluralizeX()
        {
            var plural = Pluralization.Pluralize("fox");
            plural.ShouldBe("foxes");
        }

        [TestMethod]
        public void ShouldPluralizeY()
        {
            var plural = Pluralization.Pluralize("butterfly");
            plural.ShouldBe("butterflies");
        }

        [TestMethod]
        public void ShouldPluralizeYWithPrecedingVowel()
        {
            var plural = Pluralization.Pluralize("monkey");
            plural.ShouldBe("monkeys");
        }

        [TestMethod]
        public void ShouldPluralizeIfe()
        {
            var plural = Pluralization.Pluralize("life");
            plural.ShouldBe("lives");
        }
    }
}
