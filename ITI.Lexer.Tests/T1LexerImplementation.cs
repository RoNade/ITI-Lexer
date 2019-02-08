using System.Linq;
using NUnit.Framework;

namespace ITI.Lexer.Tests
{
    [TestFixture]
    public class T1LexerImplementation
    {
        private readonly Lexer _lexer = new Lexer();

        [Test]
        public void t1_lexer_should_scan_comment()
        {
            const string program = "/*Hi*///Hello, World!!!";

            var tokens = _lexer.LexFile(program).Where(token => token.Category == TokenCategory.Comment);
            Assert.That(tokens.First().Kind, Is.EqualTo(TokenKind.BlockComment));
            Assert.That(tokens.Last().Kind, Is.EqualTo(TokenKind.LineComment));
        }

        [Test]
        public void t2_lexer_should_scan_punctuation()
        {
            const string program = "->=>:;,.";
            var tokens = _lexer.LexFile(program).Where(token => token.Kind != TokenKind.EndOfFile);
            Assert.That(tokens.Select(token => token.Category), Is.All.EqualTo(TokenCategory.Punctuation));
        }

        [Test]
        public void t3_lexer_should_scan_grouping()
        {
            const string program = "{}()[]";
            var tokens = _lexer.LexFile(program).Where(token => token.Kind != TokenKind.EndOfFile);
            Assert.That(tokens.Select(token => token.Category), Is.All.EqualTo(TokenCategory.Grouping));
        }

        [TestCase("^")]
        [TestCase("&")]
        [TestCase("|")]
        [TestCase("/")]
        [TestCase("+")]
        [TestCase("-")]
        [TestCase("<")]
        [TestCase(">")]
        [TestCase("=")]
        [TestCase("!")]
        [TestCase("%")]
        [TestCase("*")]
        [TestCase("?")]
        [TestCase("<<")]
        [TestCase(">>")]
        [TestCase("&=")]
        [TestCase("|=")]
        [TestCase("^=")]
        [TestCase("??")]
        [TestCase("*=")]
        [TestCase("+=")]
        [TestCase("/=")]
        [TestCase("%=")]
        [TestCase("++")]
        [TestCase("--")]
        [TestCase("-=")]
        [TestCase("!=")]
        [TestCase("<=")]
        [TestCase(">=")]
        public void t4_lexer_should_scan_operator(string value)
        {
            var token = _lexer.LexFile(value).First();
            Assert.That(token.Category, Is.EqualTo(TokenCategory.Operator));
        }

        [TestCase(".1")]
        [TestCase("1f")]
        [TestCase(".1f")]
        [TestCase(".1e0")]
        [TestCase("0.0e0")]
        [TestCase(".1e-32")]
        [TestCase("3.2e+10")]
        public void t5_lexer_should_scan_float(string value)
        {
            var token = _lexer.LexFile(value).First();
            Assert.That(token.Kind, Is.EqualTo(TokenKind.FloatLiteral));
        }

        [TestCase("1")]
        [TestCase("21")]
        [TestCase("0130")]
        public void t6_lexer_should_scan_integer(string value)
        {
            var token = _lexer.LexFile(value).First();
            Assert.That(token.Kind, Is.EqualTo(TokenKind.IntegerLiteral));
        }

        [TestCase("identifier1")]
        [TestCase("identifier_2")]
        [TestCase("_identifier3")]
        [TestCase("__identifier4")]
        public void t7_lexer_should_scan_identifier(string value)
        {
            var token = _lexer.LexFile(value).First();
            Assert.That(token.Kind, Is.EqualTo(TokenKind.Identifier));
        }

        [TestCase("[]{}()", TokenCategory.Grouping)]
        [TestCase(".,;:", TokenCategory.Punctuation)]
        [TestCase("var hello_09", TokenCategory.Identifier)]
        [TestCase("<>===-*/+^%&!|||&&", TokenCategory.Operator)]
        public void t8_lexer_should_identify_category(string value, TokenCategory category)
        {
            var tokens = _lexer.LexFile(value)
                .Where(token => token.Category != TokenCategory.WhiteSpace && token.Kind != TokenKind.EndOfFile);

            Assert.That(tokens.Select(token => token.Category), Is.All.EqualTo(category));
            CollectionAssert.DoesNotContain(tokens.Select(token => token.Category), TokenKind.Error);
        }

        [TestCase("\"")]
        [TestCase("1f.3")]
        [TestCase("2data")]
        [TestCase("9float")]
        public void t9_lexer_should_output_error(string value)
        {
            var token = _lexer.LexFile(value).First(tk => tk.Kind != TokenKind.EndOfFile);

            Assert.That(token.Kind, Is.EqualTo(TokenKind.Error));
            Assert.That(_lexer?.ErrorSink?.Count(), Is.EqualTo(1));

            _lexer?.ErrorSink?.Clear();
            Assert.That(_lexer?.ErrorSink?.Count(), Is.EqualTo(0));
        }
    }
}
