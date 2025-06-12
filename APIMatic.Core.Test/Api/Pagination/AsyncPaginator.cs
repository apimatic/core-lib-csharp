using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Models.Pagination;
using APIMatic.Core.Test.MockTypes.Pagination;
using APIMatic.Core.Test.MockTypes.Pagination.MetaData;
using APIMatic.Core.Test.Utilities;
using NUnit.Framework;

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

            var asyncPageable = PaginationFactory.CreateHandler(
                    CreateApiCall<TransactionsOffset>()
                        .RequestBuilder(rb => rb
                            .Setup(HttpMethod.Get, url)
                            .Parameters(p => p
                                .Query(q => q.Setup("offset", 0))
                                .Query(q => q.Setup("limit", limit)))),
                    (page, strategy) =>
                        OffsetPagedResponseFactory.Create(
                            page,
                            strategy,
                            tPage => tPage.Data
                        ),
                    (func, builder) =>
                        AsyncPaginatorFactory.Create(
                            func,
                            builder,
                            pagedResponse => pagedResponse.Items,
                            new OffsetPagination("$request.query#/offset")
                        )
                )
                .Paginate();

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

            var asyncPageable = PaginationFactory.CreateHandler(CreateApiCall<TransactionsOffset>()
                    .RequestBuilder(rb => rb
                        .Setup(HttpMethod.Get, url)
                        .Parameters(p => p
                            .Query(q => q.Setup("cursor", "cursor0"))
                            .Query(q => q.Setup("limit", limit)))),
                (page, strategy) =>
                    CursorPagedResponseFactory.Create(
                        page,
                        strategy,
                        tPage => tPage.Data
                    ),
                (func, builder) =>
                    AsyncPaginatorFactory.Create(
                        func,
                        builder,
                        pagedResponse => pagedResponse.Items,
                        new CursorPagination("$response.body#/nextCursor", "$request.query#/cursor")
                    )
            ).Paginate();

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

            var asyncPageable = PaginationFactory.CreateHandler(CreateApiCall<TransactionsOffset>()
                    .RequestBuilder(rb => rb
                        .Setup(HttpMethod.Get, url)
                        .Parameters(p => p
                            .Query(q => q.Setup("cursor", "cursor0"))
                            .Query(q => q.Setup("limit", limit)))),
                (page, strategy) =>
                    CursorPagedResponseFactory.Create(
                        page,
                        strategy,
                        tPage => tPage.Data
                    ),
                (func, builder) =>
                    AsyncPaginatorFactory.Create(
                        func,
                        builder,
                        pagedResponse => pagedResponse.Items,
                        new CursorPagination("$response.body/nextCursor", "$request.query#/cursor")
                    )
            ).Paginate();

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

            var asyncPageable = PaginationFactory.CreateHandler(CreateApiCall<TransactionsLinked>()
                    .RequestBuilder(rb => rb
                        .Setup(HttpMethod.Get, url)
                        .Parameters(p => p
                            .Query(q => q.Setup("page", "1"))
                            .Query(q => q.Setup("limit", limit)))),
                (page, strategy) =>
                    LinkPagedResponseFactory.Create(
                        page,
                        strategy,
                        tPage => tPage.Data
                    ),
                (func, builder) =>
                    AsyncPaginatorFactory.Create(
                        func,
                        builder,
                        pagedResponse => pagedResponse.Items,
                        new LinkPagination("$response.body#/links/next")
                    )
            ).Paginate();

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

            var asyncPageable = PaginationFactory.CreateHandler(CreateApiCall<TransactionsOffset>()
                    .RequestBuilder(rb => rb
                        .Setup(HttpMethod.Get, url)
                        .Parameters(p => p
                            .Query(q => q.Setup("page", 0)) // Initial page
                            .Query(q => q.Setup("limit", limit)))),
                (page, strategy) =>
                    NumberPagedResponseFactory.Create(
                        page,
                        strategy,
                        tPage => tPage.Data
                    ),
                (func, builder) =>
                    AsyncPaginatorFactory.Create(
                        func,
                        builder,
                        pagedResponse => pagedResponse.Items,
                        new NumberPagination("$request.query#/page")
                    )
            ).Paginate();

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

            var asyncPageable = PaginationFactory.CreateHandler(CreateApiCall<TransactionsLinked>()
                    .RequestBuilder(rb => rb
                        .Setup(HttpMethod.Get, url)
                        .Parameters(p => p
                            .Query(q => q.Setup("page", "1"))
                            .Query(q => q.Setup("limit", limit)))),
                (page, strategy) =>
                    BasePagedResponseFactory.Create(
                        page,
                        strategy,
                        tPage => tPage.Data
                    ),
                (func, builder) =>
                    AsyncPaginatorFactory.Create(
                        func,
                        builder,
                        pagedResponse => pagedResponse.Items,
                        new LinkPagination("$response.body#/links/next"),
                        new NumberPagination("$request.query#/page")
                    )
            ).Paginate();

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
