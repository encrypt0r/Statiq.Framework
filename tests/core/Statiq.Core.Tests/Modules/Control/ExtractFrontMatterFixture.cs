﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Statiq.Common;
using Statiq.Testing;

namespace Statiq.Core.Tests.Modules.Control
{
    [TestFixture]
    public class ExtractFrontMatterFixture : BaseFixture
    {
        public class ExecuteTests : ExtractFrontMatterFixture
        {
            [Test]
            public async Task DefaultCtorSplitsAtDashes()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"FM1
FM2
---
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter(new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"FM1
FM2
", frontMatterContent);
                Assert.AreEqual(
                    @"Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task EmptyFirstLineWithDelimiterTreatsAsFrontMatter()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"
---
FM1
FM2
---
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter(new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"
", frontMatterContent);
                Assert.AreEqual(
                    @"FM1
FM2
---
Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task EmptyFirstLineWithoutDelimiterTreatsAsFrontMatter()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"
FM1
FM2
---
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter(new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"
FM1
FM2
", frontMatterContent);
                Assert.AreEqual(
                    @"Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task DashStringDoesNotSplitAtNonmatchingDashes()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"FM1
FM2
---
Content1
Content2")
                };
                bool executed = false;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter("-", new ExecuteConfig(Config.FromDocument(x =>
                {
                    executed = true;
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.IsFalse(executed);
                Assert.AreEqual(
                    @"FM1
FM2
---
Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task MatchingStringSplitsAtCorrectLocation()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"FM1
FM2
ABC
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter("ABC", new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"FM1
FM2
", frontMatterContent);
                Assert.AreEqual(
                    @"Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task SingleCharWithRepeatedDelimiterSplitsAtCorrectLocation()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"FM1
FM2
!!!!
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter('!', new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"FM1
FM2
", frontMatterContent);
                Assert.AreEqual(
                    @"Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task SingleCharWithRepeatedDelimiterWithTrailingSpacesSplitsAtCorrectLocation()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"FM1
FM2
!!!!  
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter('!', new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"FM1
FM2
", frontMatterContent);
                Assert.AreEqual(
                    @"Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task SingleCharWithRepeatedDelimiterWithLeadingSpacesDoesNotSplit()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"FM1
FM2
  !!!!
Content1
Content2")
                };
                bool executed = false;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter('!', new ExecuteConfig(Config.FromDocument(x =>
                {
                    executed = true;
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.IsFalse(executed);
                Assert.AreEqual(
                    @"FM1
FM2
  !!!!
Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task SingleCharWithRepeatedDelimiterWithExtraLinesSplitsAtCorrectLocation()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                     new TestDocument(@"FM1
FM2

!!!!

Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter('!', new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"FM1
FM2

", frontMatterContent);
                Assert.AreEqual(
                    @"
Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task SingleCharWithSingleDelimiterSplitsAtCorrectLocation()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"FM1
FM2
!
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter('!', new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"FM1
FM2
", frontMatterContent);
                Assert.AreEqual(
                    @"Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task MultipleInputDocumentsResultsInMultipleOutputs()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"AA
-
XX"),
                    new TestDocument(@"BB
-
YY")
                };
                string frontMatterContent = string.Empty;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter(new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent += await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(2, documents.Count());
                Assert.AreEqual(
                    @"AA
BB
", frontMatterContent);
                Assert.AreEqual("XX", await documents.First().GetContentStringAsync());
                Assert.AreEqual("YY", await documents.Skip(1).First().GetContentStringAsync());
            }

            [Test]
            public async Task DefaultCtorIgnoresDelimiterOnFirstLine()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"---
FM1
FM2
---
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter(new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                })));

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual(
                    @"FM1
FM2
", frontMatterContent);
                Assert.AreEqual(
                    @"Content1
Content2", await documents.First().GetContentStringAsync());
            }

            [Test]
            public async Task NoIgnoreDelimiterOnFirstLine()
            {
                // Given
                TestExecutionContext context = new TestExecutionContext();
                TestDocument[] inputs =
                {
                    new TestDocument(@"---
FM1
FM2
---
Content1
Content2")
                };
                string frontMatterContent = null;
                ExtractFrontMatter frontMatter = new ExtractFrontMatter(new ExecuteConfig(Config.FromDocument(async x =>
                {
                    frontMatterContent = await x.GetContentStringAsync();
                    return new[] { x };
                }))).IgnoreDelimiterOnFirstLine(false);

                // When
                IEnumerable<IDocument> documents = await ExecuteAsync(inputs, context, frontMatter);

                // Then
                Assert.AreEqual(1, documents.Count());
                Assert.AreEqual("\n", frontMatterContent);
                Assert.AreEqual(
                    @"FM1
FM2
---
Content1
Content2", await documents.First().GetContentStringAsync());
            }
        }
    }
}
