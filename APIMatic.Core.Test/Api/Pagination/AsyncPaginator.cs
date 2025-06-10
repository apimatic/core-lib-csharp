using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Models.Pagination;
using APIMatic.Core.Test.MockTypes.Pagination;
using APIMatic.Core.Test.MockTypes.Pagination.MetaData;
using APIMatic.Core.Test.Utilities;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Api.Pagination
{
    [TestFixture]
    public class AsyncPaginator : ApiCallTest
    {
        [Test]
        public async Task PaginateAsync_QueryOffsetPaginationYieldsAllItems_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions/offset";

            var allTransactions = new List<Transaction>
            {
                new("id1", 100, DateTime.UtcNow),
                new("id2", 200, DateTime.UtcNow),
                new("id3", 300, DateTime.UtcNow),
                new("id4", 400, DateTime.UtcNow),
                new("id5", 500, DateTime.UtcNow)
            };

            var transactionOffsets = PaginationHelper.ChunkTransactions(allTransactions, limit);
            handlerMock.MockOffsetPaginatedResponses(transactionOffsets, GetCompleteUrl(url));

            var asyncPageable = CreateApiCall<TransactionsOffset>()
                .RequestBuilder(rb => rb
                    .Setup(HttpMethod.Get, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("offset", 0))
                        .Query(q => q.Setup("limit", limit))))
                .PaginateAsync(
                    res => res.Data.Data,
                    OffsetPagedResponseFactory.Create,
                    page => page.Items,
                    PaginatorFactory.Create,
                    new OffsetPagination("$request.query#/offset")
                );

            var collected = new List<Transaction>();

            // Act
            await foreach (var item in asyncPageable)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(allTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(allTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryCursorPaginationYieldsAllItems_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions";

            var allTransactions = new List<Transaction>
            {
                new("id1", 100, DateTime.UtcNow),
                new("id2", 200, DateTime.UtcNow),
                new("id3", 300, DateTime.UtcNow),
                new("id4", 400, DateTime.UtcNow),
                new("id5", 500, DateTime.UtcNow)
            };

            var transactionsWithCursor = PaginationHelper.ChunkTransactionsWithCursor(allTransactions, limit);
            handlerMock.MockCursorPaginatedResponses(transactionsWithCursor, $"{GetCompleteUrl(url)}?limit={limit}");

            var asyncPageable = CreateApiCall<TransactionsOffset>()
                .RequestBuilder(rb => rb
                    .Setup(HttpMethod.Get, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("cursor", "cursor0"))
                        .Query(q => q.Setup("limit", limit))))
                .PaginateAsync(
                    res => res.Data.Data,
                    CursorPagedResponseFactory.Create,
                    page => page.Items,
                    PaginatorFactory.Create,
                    new CursorPagination("$response.body#/nextCursor", "$request.query#/cursor")
                );

            var collected = new List<Transaction>();

            // Act
            await foreach (var item in asyncPageable)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(allTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(allTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryCursorPaginationInvalidPointer_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions";

            var allTransactions = new List<Transaction>
            {
                new("id1", 100, DateTime.UtcNow),
                new("id2", 200, DateTime.UtcNow),
                new("id3", 300, DateTime.UtcNow),
                new("id4", 400, DateTime.UtcNow),
                new("id5", 500, DateTime.UtcNow)
            };

            var transactionsWithCursor = PaginationHelper.ChunkTransactionsWithCursor(allTransactions, limit);
            handlerMock.MockCursorPaginatedResponses(transactionsWithCursor, $"{GetCompleteUrl(url)}?limit={limit}");

            var asyncPageable = CreateApiCall<TransactionsOffset>()
                .RequestBuilder(rb => rb
                    .Setup(HttpMethod.Get, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("cursor", "cursor0"))
                        .Query(q => q.Setup("limit", limit))))
                .PaginateAsync(
                    res => res.Data.Data,
                    CursorPagedResponseFactory.Create,
                    page => page.Items,
                    PaginatorFactory.Create,
                    new CursorPagination("$response.body/nextCursor", "$request.query#/cursor")
                );

            var collected = new List<Transaction>();

            // Act
            await foreach (var item in asyncPageable)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(limit, collected.Count);
            var expected = allTransactions.Take(limit).ToList();
            CollectionAssert.AreEquivalent(expected.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryLinkPaginationYieldsAllItems_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions";

            var allTransactions = new List<Transaction>
            {
                new("id1", 100, DateTime.UtcNow),
                new("id2", 200, DateTime.UtcNow),
                new("id3", 300, DateTime.UtcNow),
                new("id4", 400, DateTime.UtcNow),
                new("id5", 500, DateTime.UtcNow)
            };

            var linkedPages = PaginationHelper.ChunkTransactionsWithLinks(allTransactions, limit, url);
            handlerMock.MockLinkedPaginatedResponses(linkedPages, GetCompleteUrl(string.Empty));

            var asyncPageable = CreateApiCall<TransactionsLinked>()
                .RequestBuilder(rb => rb
                    .Setup(HttpMethod.Get, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("page", "1"))
                        .Query(q => q.Setup("limit", limit))))
                .PaginateAsync(
                    res => res.Data.Data,
                    LinkPagedResponseFactory.Create,
                    page => page.Items,
                    PaginatorFactory.Create,
                    new LinkPagination("$response.body#/links/next")
                );

            var collected = new List<Transaction>();

            // Act
            await foreach (var item in asyncPageable)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(allTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(allTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryPagePaginationYieldsAllItemsAcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions/page";

            var allTransactions = new List<Transaction>
            {
                new("id1", 100, DateTime.UtcNow),
                new("id2", 200, DateTime.UtcNow),
                new("id3", 300, DateTime.UtcNow),
                new("id4", 400, DateTime.UtcNow),
                new("id5", 500, DateTime.UtcNow)
            };

            var pages = PaginationHelper.ChunkTransactions(allTransactions, limit);
            handlerMock.MockPagePaginatedResponse(pages, GetCompleteUrl(url), limit);

            var asyncPageable = CreateApiCall<TransactionsOffset>()
                .RequestBuilder(rb => rb
                    .Setup(HttpMethod.Get, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("page", 0)) // Initial page
                        .Query(q => q.Setup("limit", limit))))
                .PaginateAsync(
                    res => res.Data.Data,
                    NumberPagedResponseFactory.Create,
                    page => page.Items,
                    PaginatorFactory.Create,
                    new NumberPagination("$request.query#/page")
                );

            var collected = new List<Transaction>();

            // Act
            await foreach (var tx in asyncPageable)
            {
                collected.Add(tx);
            }

            // Assert
            Assert.AreEqual(allTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(allTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryMultiPaginationYieldsAllItems_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions";

            var allTransactions = new List<Transaction>
            {
                new("id1", 100, DateTime.UtcNow),
                new("id2", 200, DateTime.UtcNow),
                new("id3", 300, DateTime.UtcNow),
                new("id4", 400, DateTime.UtcNow),
                new("id5", 500, DateTime.UtcNow)
            };

            var linkedPages = PaginationHelper.ChunkTransactionsWithLinksAndPages(allTransactions, limit, url);
            handlerMock.MockMultiPaginatedResponses(linkedPages, GetCompleteUrl(string.Empty), limit);

            var asyncPageable = CreateApiCall<TransactionsLinked>()
                .RequestBuilder(rb => rb
                    .Setup(HttpMethod.Get, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("page", "1"))
                        .Query(q => q.Setup("limit", limit))))
                .PaginateAsync(
                    res => res.Data.Data,
                    PagedResponseFactory.Create,
                    page => page.Items,
                    PaginatorFactory.Create,
                    new LinkPagination("$response.body#/links/next"),
                    new NumberPagination("$request.query#/page")
                );

            var collected = new List<Transaction>();

            // Act
            await foreach (var item in asyncPageable)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(allTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(allTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }
    }
}
