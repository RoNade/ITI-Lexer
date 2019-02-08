using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace ITI.Lexer.Tests
{
    [TestFixture]
    public class PublicModelChecker
    {
        [Test]
        [Explicit]
        public void Write_current_API_to_console_with_double_quotes()
        {
            Console.WriteLine(GetPublicApi(typeof(Lexer).Assembly).ToString().Replace("\"", "\"\""));
        }

        [Test]
        public void Public_API_is_not_modified()
        {
            var model = XElement.Parse(@"<Assembly Name=""ITI.Lexer.Impl"">
                                            <Types>
                                                <Type Name=""ITI.Lexer.Lexer"">
                                                    <Member Type=""Constructor"" Name="".ctor"" />
                                                    <Member Type=""Constructor"" Name="".ctor"" />
                                                    <Member Type=""Method"" Name=""Equals"" />
                                                    <Member Type=""Property"" Name=""ErrorSink"" />
                                                    <Member Type=""Method"" Name=""get_ErrorSink"" />
                                                    <Member Type=""Method"" Name=""GetHashCode"" />
                                                    <Member Type=""Method"" Name=""GetType"" />
                                                    <Member Type=""Method"" Name=""LexFile"" />
                                                    <Member Type=""Method"" Name=""LexFile"" />
                                                    <Member Type=""Method"" Name=""Peek"" />
                                                    <Member Type=""Method"" Name=""ToString"" />
                                                </Type>
                                            </Types>
                                        </Assembly>");

            var current = GetPublicApi(typeof(Lexer).Assembly);
            if (!XNode.DeepEquals(model, current))
            {
                var m = model.ToString(SaveOptions.DisableFormatting);
                var c = current.ToString(SaveOptions.DisableFormatting);
                Assert.That(c, Is.EqualTo(m));
            }
        }

        private XElement GetPublicApi(Assembly a)
        {
            return new XElement("Assembly",
                                new XAttribute("Name", a.GetName().Name),
                                new XElement("Types",
                                             AllNestedTypes(a.GetExportedTypes())
                                            .OrderBy(t => t.FullName)
                                            .Select(t => new XElement("Type",
                                                                      new XAttribute("Name", t.FullName),
                                                                      t.GetMembers()
                                                                      .OrderBy(m => m.Name)
                                                                      .Select(m => new XElement("Member",
                                                                                                new XAttribute("Type", m.MemberType),
                                                                                                new XAttribute("Name", m.Name)))))));
        }

        private IEnumerable<Type> AllNestedTypes(IEnumerable<Type> types)
        {
            foreach (var t in types)
            {
                yield return t;
                foreach (var nestedType in AllNestedTypes(t.GetNestedTypes()))
                {
                    yield return nestedType;
                }
            }
        }
    }
}
