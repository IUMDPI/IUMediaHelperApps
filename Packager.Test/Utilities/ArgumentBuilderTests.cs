using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Packager.Utilities;
using Packager.Utilities.ProcessRunners;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class ArgumentBuilderTests
    {

        [Test]
        public void ConstructorFromStringShouldWorkCorrectly()
        {
            var arguments = new ArgumentBuilder("1 22 333 4444");
            Assert.That(arguments.Count, Is.EqualTo(4));    
            Assert.That(arguments.ToArray()[0], Is.EqualTo("1"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("22"));
            Assert.That(arguments.ToArray()[2], Is.EqualTo("333"));
            Assert.That(arguments.ToArray()[3], Is.EqualTo("4444"));
        }

        [Test]
        public void ConstructorFromStringShouldIgnoreEmptyEntries()
        {
            var arguments = new ArgumentBuilder(" 1 22  333  4444   ");
            Assert.That(arguments.Count, Is.EqualTo(4));
            Assert.That(arguments.ToArray()[0], Is.EqualTo("1"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("22"));
            Assert.That(arguments.ToArray()[2], Is.EqualTo("333"));
            Assert.That(arguments.ToArray()[3], Is.EqualTo("4444"));
        }

        [Test]
        public void AddArgumentsFromStringShouldWorkCorrectly()
        {
            var arguments = new ArgumentBuilder("aa");
            arguments.AddArguments("1 22 333 4444");
            Assert.That(arguments.Count, Is.EqualTo(5));
            Assert.That(arguments.ToArray()[0], Is.EqualTo("aa"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("1"));
            Assert.That(arguments.ToArray()[2], Is.EqualTo("22"));
            Assert.That(arguments.ToArray()[3], Is.EqualTo("333"));
            Assert.That(arguments.ToArray()[4], Is.EqualTo("4444"));
        }

        [Test]
        public void AddArgumentsFromStringShoulIgnoreEmptryEntriesCorrectly()
        {
            var arguments = new ArgumentBuilder("aa");
            arguments.AddArguments(" 1 22  333  4444   ");
            Assert.That(arguments.Count, Is.EqualTo(5));
            Assert.That(arguments.ToArray()[0], Is.EqualTo("aa"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("1"));
            Assert.That(arguments.ToArray()[2], Is.EqualTo("22"));
            Assert.That(arguments.ToArray()[3], Is.EqualTo("333"));
            Assert.That(arguments.ToArray()[4], Is.EqualTo("4444"));
        }

        [Test]
        public void AddArgumentsFromSingleStringShouldWorkCorrectly()
        {
            var arguments = new ArgumentBuilder("aa");
            arguments.AddArguments("1");
            Assert.That(arguments.Count, Is.EqualTo(2));
            Assert.That(arguments.ToArray()[0], Is.EqualTo("aa"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("1"));
        }

        [Test]
        public void AddArgumentsFromSingleStringShouldIgnoreEmptryEntriesCorrectly()
        {
            var arguments = new ArgumentBuilder("aa");
            arguments.AddArguments(" 1 ");
            Assert.That(arguments.Count, Is.EqualTo(2));
            Assert.That(arguments.ToArray()[0], Is.EqualTo("aa"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("1"));
        }

        [Test]
        public void AddArgumentsFromEnumerableShouldWorkCorrectly()
        {
            var arguments = new ArgumentBuilder("aa");
            arguments.AddArguments(new List<string> {"1", "22", "333", "4444"});
            Assert.That(arguments.Count, Is.EqualTo(5));
            Assert.That(arguments.ToArray()[0], Is.EqualTo("aa"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("1"));
            Assert.That(arguments.ToArray()[2], Is.EqualTo("22"));
            Assert.That(arguments.ToArray()[3], Is.EqualTo("333"));
            Assert.That(arguments.ToArray()[4], Is.EqualTo("4444"));
        }

        [Test]
        public void AddArgumentsFromEnumerableNormalizeEntries()
        {
            var arguments = new ArgumentBuilder("aa");
            arguments.AddArguments(new List<string> { "1 ", "22 ", " 333", " 4444" });
            Assert.That(arguments.Count, Is.EqualTo(5));
            Assert.That(arguments.ToArray()[0], Is.EqualTo("aa"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("1"));
            Assert.That(arguments.ToArray()[2], Is.EqualTo("22"));
            Assert.That(arguments.ToArray()[3], Is.EqualTo("333"));
            Assert.That(arguments.ToArray()[4], Is.EqualTo("4444"));
        }


        [Test]
        public void AddArgumentsFromEnumerableIgnoreEmptryEntries()
        {
            var arguments = new ArgumentBuilder("aa");
            arguments.AddArguments(new List<string> { "1", "22", "333", "4444", "" });
            Assert.That(arguments.Count, Is.EqualTo(5));
            Assert.That(arguments.ToArray()[0], Is.EqualTo("aa"));
            Assert.That(arguments.ToArray()[1], Is.EqualTo("1"));
            Assert.That(arguments.ToArray()[2], Is.EqualTo("22"));
            Assert.That(arguments.ToArray()[3], Is.EqualTo("333"));
            Assert.That(arguments.ToArray()[4], Is.EqualTo("4444"));
        }
    }
}
