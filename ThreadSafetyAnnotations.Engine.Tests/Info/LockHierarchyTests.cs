using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadSafetyAnnotations.Engine.Info;

namespace ThreadSafetyAnnotations.Engine.Tests.Info
{
    [TestFixture]
    public class LockHierarchyTests
    {
        [Test]
        [TestCase("lock1", "", false)]
        [TestCase("lock1", "lock1", true)]
        [TestCase("lock1", "lock2", false)]
        [TestCase("lock1", "lock1|lock2", true)]
        [TestCase("lock1|lock2", "lock1", false)]
        [TestCase("lock1|lock2", "lock1|lock2", true)]
        [TestCase("lock1|lock2", "lock2|lock1", false)]
        [TestCase("lock1|lock2", "lock1|lock2|lock3", true)]
        [TestCase("lock1|lock2", "lock3|lock1|lock2", true)]
        [TestCase("lock1|lock2", "lock1|lock3|lock2", true)]
        [TestCase("lock1|lock2", "lock4|lock5|lock6", false)]
        public void IsSatisfiedBy_Test(string expectedHierarchy, string actualHierarchy, bool expectedOutcome)
        {
            List<string> expected = expectedHierarchy.Split('|').ToList();
            List<string> actual = actualHierarchy.Split('|').ToList();

            bool result = LockHierarchy.IsSatisfiedBy(LockHierarchy.FromStringList(expected), LockHierarchy.FromStringList(actual));

            Assert.AreEqual(expectedOutcome, result);
        }

        [Test]
        [TestCase("lock1", "", false)]
        [TestCase("lock1", "lock2", false)]
        [TestCase("lock1", "lock1", false)]

        [TestCase("lock1|lock2", "", false)]
        [TestCase("lock1|lock2", "lock1", false)]
        [TestCase("lock1|lock2", "lock2", false)]
        [TestCase("lock1|lock2", "lock1|lock2", false)]
        [TestCase("lock1|lock2", "lock2|lock1", true)]

        [TestCase("lock1|lock2|lock3", "", false)]
        [TestCase("lock1|lock2|lock3", "lock1", false)]
        [TestCase("lock1|lock2|lock3", "lock2", false)]
        [TestCase("lock1|lock2|lock3", "lock3", false)]
        [TestCase("lock1|lock2|lock3", "lock1|lock2", false)]
        [TestCase("lock1|lock2|lock3", "lock2|lock1", true)]
        [TestCase("lock1|lock2|lock3", "lock1|lock2|lock3", false)]
        [TestCase("lock1|lock2|lock3", "lock3|lock1|lock2", true)]
        [TestCase("lock1|lock2|lock3", "lock3|lock2|lock1", true)]

        [TestCase("lock1|lock2|lock3", "lock4|lock1", false)]
        [TestCase("lock1|lock2|lock3", "lock4|lock1|lock5", false)]
        [TestCase("lock1|lock2|lock3", "lock4|lock1|lock5|lock2|lock6|lock3", false)]
        [TestCase("lock1|lock2|lock3", "lock4|lock1|lock5|lock3|lock6|lock2", true)]

        [TestCase("lock1|lock2|lock3", "lock4|lock5|lock6", false)]
        public void Conflicts_Test(string h1, string h2, bool expectedOutcome)
        {
            List<string> first = h1.Split('|').ToList();
            List<string> second = h2.Split('|').ToList();

            bool result = LockHierarchy.Conflicts(LockHierarchy.FromStringList(first), LockHierarchy.FromStringList(second));

            Assert.AreEqual(expectedOutcome, result);

            result = LockHierarchy.Conflicts(LockHierarchy.FromStringList(second), LockHierarchy.FromStringList(first));

            Assert.AreEqual(expectedOutcome, result);
        }
    }
}
