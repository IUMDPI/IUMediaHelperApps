using System.Collections.Generic;
using NUnit.Framework;
using Packager.Extensions;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class ArrayExtensionTests
    {
        [TestCase(1, "1 member")]
        [TestCase(2, "2 members")]
        [TestCase(0, "0 members")]
        public void IntegerToSingularOrPluralShouldReturnCorrectValues(int value, string expected)
        {
            Assert.That(value.ToSingularOrPlural("member", "members"), Is.EqualTo(expected));
        }

        [Test]
        public void DictionaryToSingularOrPluralShouldReturnPluralIfMoreThanOneMemberPresent()
        {
            var members = new Dictionary<string, string> {{"one", "one"}, {"two", "two"}};
            Assert.That(members.ToSingularOrPlural("member", "members"), Is.EqualTo("2 members"));
        }

        [Test]
        public void DictionaryToSingularOrPluralShouldReturnPluralIfNoMembersPresent()
        {
            var members = new Dictionary<string, string>();
            Assert.That(members.ToSingularOrPlural("member", "members"), Is.EqualTo("0 members"));
        }

        [Test]
        public void DictionaryToSingularOrPluralShouldReturnSingularIfOneMemberPresent()
        {
            var members = new Dictionary<string, string> {{"one", "one"}};
            Assert.That(members.ToSingularOrPlural("member", "member"), Is.EqualTo("1 member"));
        }

        [Test]
        public void EnumerableToSingularOrPluralShouldReturnPluralIfMoreThanOneMemberPresent()
        {
            var members = new[] {"one", "two"};
            Assert.That(members.ToSingularOrPlural("member", "members"), Is.EqualTo("2 members"));
        }

        [Test]
        public void EnumerableToSingularOrPluralShouldReturnPluralIfNoMembersPresent()
        {
            var members = new string[0];
            Assert.That(members.ToSingularOrPlural("member", "members"), Is.EqualTo("0 members"));
        }

        [Test]
        public void EnumerableToSingularOrPluralShouldReturnSingularIfOneMemberPresent()
        {
            var members = new[] {"one"};
            Assert.That(members.ToSingularOrPlural("member", "member"), Is.EqualTo("1 member"));
        }

        [Test]
        public void FromIndexShouldReturnCorrectValues()
        {
            var parts = new[] {"part1", "part2"};
            Assert.That(parts.FromIndex(0, "default value"), Is.EqualTo(parts[0]));
            Assert.That(parts.FromIndex(1, "default value"), Is.EqualTo(parts[1]));
            Assert.That(parts.FromIndex(2, "default value"), Is.EqualTo("default value"));
        }


        [Test]
        public void IsFirstShouldReturnCorrectResult()
        {
            var first = new object();
            var second = new object();
            var objects = new List<object> {first, second};
            Assert.That(objects.IsFirst(first), Is.True);
            Assert.That(objects.IsFirst(second), Is.False);
        }
    }
}