using System;
using System.Collections;
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
    public class Paginator : ApiCallTest
    {
        [Test]
        public void Paginate_QueryOffsetPaginationYieldsAllItems_AcrossPages()
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

            var paginator = CreateApiCall<TransactionsOffset>()
                .RequestBuilder(rb => rb
                    .Setup(HttpMethod.Get, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("offset", 0))
                        .Query(q => q.Setup("limit", limit))))
                .Paginate(
                    res => res.Data.Data,
                    OffsetPagedResponseFactory.Create,
                    page => page.Items,
                    PaginatorFactory.Create,
                    new OffsetPagination("$request.query#/offset")
                );

            var enumerator = ((IEnumerable)paginator).GetEnumerator();
            var collected = new List<Transaction>();
            while (enumerator.MoveNext())
            {
                collected.Add((Transaction)enumerator.Current);
            }

            // Assert
            Assert.AreEqual(allTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(allTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }
        
        [Test]
        public void Paginate_QueryLinkPaginationYieldsAllItems_AcrossPages()
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

            var paginator = CreateApiCall<TransactionsLinked>()
                .RequestBuilder(rb => rb
                    .Setup(HttpMethod.Get, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("page", "1"))
                        .Query(q => q.Setup("limit", limit))))
                .Paginate(
                    res => res.Data.Data,
                    LinkPagedResponseFactory.Create,
                    page => page.Items,
                    PaginatorFactory.Create,
                    new LinkPagination("$response.body#/links/next")
                );

            var collected = new List<Transaction>();

            // Act
            foreach (var item in paginator)
            {
                collected.Add(item);
            }

            // Assert
            Assert.AreEqual(allTransactions.Count, collected.Count);
            CollectionAssert.AreEquivalent(allTransactions.Select(t => t.Id), collected.Select(t => t.Id));
        }
    }
}
