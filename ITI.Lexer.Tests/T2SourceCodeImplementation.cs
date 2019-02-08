using System;
using System.Linq;
using NUnit.Framework;

namespace ITI.Lexer.Tests
{
    public class T2SourceCodeImplementation
    {
        private const string Program =
            @"var name = 'Me';
name == 'Me';
return name;
// Final Line";

        private SourceCode code = new SourceCode(Program);

        [Test]
        public void t1_soure_code_content_should_match()
        {
            Assert.That(code.Content, Is.EqualTo(Program));
        }

        [Test]
        public void t2_source_code_should_handle_one_subset()
        {
            var selectedLines = code.GetLines(1, 1);

            Assert.That(selectedLines.Length, Is.EqualTo(1));
            Assert.That(code.GetLine(1), Is.EqualTo(selectedLines.First()));
        }

        [Test]
        public void t3_source_code_should_return_correct_subset()
        {
            var selectedLines = code.GetLines(2, 4);

            Assert.That(selectedLines.Length, Is.EqualTo(3));
            CollectionAssert.Contains(selectedLines, code.GetLine(2));
            CollectionAssert.DoesNotContain(selectedLines, code.GetLine(1));
        }

        [Test]
        public void t4_subset_should_throw_out_of_range_exception()
        {
            Assert.Throws<IndexOutOfRangeException>(() => code.GetLine(0));
            Assert.Throws<IndexOutOfRangeException>(() => code.GetLine(-1));
            Assert.Throws<IndexOutOfRangeException>(() => code.GetLines(0, int.MaxValue));
        }
    }
}
