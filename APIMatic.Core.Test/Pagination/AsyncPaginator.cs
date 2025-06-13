using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.Api;
using APIMatic.Core.Test.MockTypes.Models.Pagination;
using APIMatic.Core.Test.MockTypes.Pagination;
using APIMatic.Core.Test.MockTypes.Pagination.MetaData;
using APIMatic.Core.Test.Utilities;
using NUnit.Framework;

namespace APIMatic.Core.Test.Pagination
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

            var transactionOffsets = PaginationHelper.ChunkTransactions(PaginationHelper.AllTransactions, limit);
            handlerMock.MockOffsetPaginatedResponses(transactionOffsets, GetCompleteUrl(url));

            var asyncPaginator = PaginationFactory.CreateHandler(
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
            await foreach (var item in asyncPaginator)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(PaginationHelper.AllTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(PaginationHelper.AllTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryOffsetPagination_YieldsNoItems()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions/offset/noitems";

            var emptyTransactionOffsets = new List<TransactionsOffset> { new() };
            handlerMock.MockOffsetPaginatedResponses(emptyTransactionOffsets, GetCompleteUrl(url));

            var asyncPaginator = PaginationFactory.CreateHandler(
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
            await foreach (var item in asyncPaginator)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(0, collected.Count);
        }

        [Test]
        public async Task PaginateAsync_QueryCursorPaginationYieldsAllItems_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions";

            var transactionsWithCursor = PaginationHelper.ChunkTransactionsWithCursor(PaginationHelper.AllTransactions, limit);
            handlerMock.MockCursorPaginatedResponses(transactionsWithCursor, $"{GetCompleteUrl(url)}?limit={limit}");

            var asyncPaginator = PaginationFactory.CreateHandler(CreateApiCall<TransactionsOffset>()
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
            await foreach (var item in asyncPaginator)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(PaginationHelper.AllTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(PaginationHelper.AllTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryCursorPaginationInvalidPointer_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions";

            var transactionsWithCursor = PaginationHelper.ChunkTransactionsWithCursor(PaginationHelper.AllTransactions, limit);
            handlerMock.MockCursorPaginatedResponses(transactionsWithCursor, $"{GetCompleteUrl(url)}?limit={limit}");

            var asyncPaginator = PaginationFactory.CreateHandler(CreateApiCall<TransactionsOffset>()
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
            await foreach (var item in asyncPaginator)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(limit, collected.Count);
            var expected = PaginationHelper.AllTransactions.Take(limit).ToList();
            CollectionAssert.AreEquivalent(expected.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryLinkPaginationYieldsAllItems_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions";

            var linkedPages = PaginationHelper.ChunkTransactionsWithLinks(PaginationHelper.AllTransactions, limit, url);
            handlerMock.MockLinkedPaginatedResponses(linkedPages, GetCompleteUrl(string.Empty));

            var asyncPaginator = PaginationFactory.CreateHandler(CreateApiCall<TransactionsLinked>()
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
            await foreach (var item in asyncPaginator)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(PaginationHelper.AllTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(PaginationHelper.AllTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryPagePaginationYieldsAllItemsAcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/transactions/page";

            var pages = PaginationHelper.ChunkTransactions(PaginationHelper.AllTransactions, limit);
            handlerMock.MockPagePaginatedResponse(pages, GetCompleteUrl(url), limit);

            var asyncPaginator = PaginationFactory.CreateHandler(CreateApiCall<TransactionsOffset>()
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
            await foreach (var tx in asyncPaginator)
            {
                collected.Add(tx);
            }

            // Assert
            Assert.AreEqual(PaginationHelper.AllTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(PaginationHelper.AllTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QueryMultiPaginationYieldsAllItems_AcrossPages()
        {
            // Arrange
            const int limit = 2;
            const string url = "/multi/transactions/multi/pagination/strategy";

            var linkedPages = PaginationHelper.ChunkTransactionsWithLinksAndPages(PaginationHelper.AllTransactions, limit, url);
            handlerMock.MockMultiPaginatedResponses(linkedPages, GetCompleteUrl(string.Empty), url, limit);

            var asyncPaginator = PaginationFactory.CreateHandler(CreateApiCall<TransactionsLinked>()
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
            await foreach (var item in asyncPaginator)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(PaginationHelper.AllTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(PaginationHelper.AllTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }

        [Test]
        public async Task PaginateAsync_QuerySinglePagination_YieldsFirstPageItemsOnly()
        {
            // Arrange
            const int limit = 2;
            const string url = "/multi/transactions/single/pagination/strategy";

            var linkedPages = PaginationHelper.ChunkTransactionsWithLinksAndPages(PaginationHelper.AllTransactions, limit, url);
            handlerMock.MockMultiPaginatedResponses(linkedPages, GetCompleteUrl(string.Empty), url, limit);

            var asyncPaginator = PaginationFactory.CreateHandler(CreateApiCall<TransactionsLinked>()
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
                        new LinkPagination("$response.body#/links/next")
                    )
            ).Paginate();

            var collected = new List<Transaction>();

            // Act
            await foreach (var item in asyncPaginator)
            {
                collected.Add(item);
            }

            // Assert
            var firstPageItems = PaginationHelper.AllTransactions.Take(2).ToList();
            Assert.AreEqual(firstPageItems.Count, collected.Count);
            CollectionAssert.AreEquivalent(firstPageItems.Select(t => t.Id), collected.Select(t => t.Id));
        }
    }
}
