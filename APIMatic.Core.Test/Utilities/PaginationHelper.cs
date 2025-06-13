using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using APIMatic.Core.Test.MockTypes.Models.Pagination;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Utilities
{
    internal static class PaginationHelper
    {
        internal static readonly IReadOnlyCollection<Transaction> AllTransactions = new List<Transaction>
        {
            new("id1", 100, DateTime.UtcNow),
            new("id2", 200, DateTime.UtcNow),
            new("id3", 300, DateTime.UtcNow),
            new("id4", 400, DateTime.UtcNow),
            new("id5", 500, DateTime.UtcNow)
        };
        
        public static List<TransactionsOffset> ChunkTransactions(IReadOnlyCollection<Transaction> transactions, int limit)
        {
            var chunks = transactions
                .Select((tx, index) => new { tx, index })
                .GroupBy(x => x.index / limit)
                .Select(g => new TransactionsOffset(g.Select(x => x.tx).ToList()))
                .ToList();

            chunks.Add(new TransactionsOffset(new List<Transaction>()));

            return chunks;
        }

        public static void MockOffsetPaginatedResponses(this MockHttpMessageHandler handlerMock,
            List<TransactionsOffset> pages, string baseUrl)
        {
            int offset = 0;
            foreach (var page in pages)
            {
                var pagedContent = JsonContent.Create(page);

                handlerMock.When($"{baseUrl}?offset={offset}")
                    .With(req =>
                    {
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        return true;
                    })
                    .Respond(HttpStatusCode.OK, pagedContent);

                offset += page.Data?.Count ?? 0;
            }
        }

        public static void MockPagePaginatedResponse(this MockHttpMessageHandler handlerMock,
            List<TransactionsOffset> pages, string baseUrl, int limit)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                var content = JsonContent.Create(pages[i]);
                var requestUrl = $"{baseUrl}?page={i}&limit={limit}";

                handlerMock.When(requestUrl)
                    .With(req =>
                    {
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        return true;
                    })
                    .Respond(HttpStatusCode.OK, content);
            }
        }

        public static List<TransactionsCursored> ChunkTransactionsWithCursor(IReadOnlyCollection<Transaction> transactions, int limit)
        {
            var chunks = transactions
                .Select((tx, index) => new { tx, index })
                .GroupBy(x => x.index / limit)
                .Select((g, i) =>
                    new TransactionsCursored(g.Select(x => x.tx).ToList(), $"cursor{(i * limit) + g.Count()}"))
                .ToList();

            // Final empty page with null cursor
            chunks.Add(new TransactionsCursored(new List<Transaction>(), null));
            return chunks;
        }

        public static void MockCursorPaginatedResponses(this MockHttpMessageHandler handlerMock,
            List<TransactionsCursored> pages, string baseUrl)
        {
            var cursor = "cursor0";
            foreach (var page in pages)
            {
                var pagedContent = JsonContent.Create(page);
                var url = $"{baseUrl}&cursor={cursor}";

                handlerMock.When(url)
                    .With(req =>
                    {
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        return true;
                    })
                    .Respond(HttpStatusCode.OK, pagedContent);

                cursor = page.NextCursor;
            }
        }

        public static List<TransactionsLinked> ChunkTransactionsWithLinks(IReadOnlyCollection<Transaction> transactions, int limit,
            string urlPath)
        {
            var chunks = transactions
                .Select((tx, index) => new { tx, index })
                .GroupBy(x => x.index / limit)
                .Select((g, i) =>
                {
                    var pageNumber = i + 1;
                    var nextLink = (i + 1) * limit < transactions.Count
                        ? $"{urlPath}?page={pageNumber + 1}&limit={limit}"
                        : null;

                    var links = new Links(next: nextLink);
                    return new TransactionsLinked(g.Select(x => x.tx).ToList(), links);
                })
                .ToList();

            return chunks;
        }

        public static void MockLinkedPaginatedResponses(this MockHttpMessageHandler handlerMock,
            List<TransactionsLinked> pages, string baseUrl)
        {
            var currentLink = "/transactions?page=1&limit=2";
            for (int i = 0; i < pages.Count; i++)
            {
                var page = pages[i];
                var currentUrl = $"{baseUrl}{currentLink}";

                handlerMock.When(currentUrl)
                    .With(req =>
                    {
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        return true;
                    })
                    .Respond(HttpStatusCode.OK, JsonContent.Create(page));
                currentLink = page.Links.Next;
            }
        }

        public static List<TransactionsLinked> ChunkTransactionsWithLinksAndPages(IReadOnlyCollection<Transaction> transactions,
            int limit, string urlPath)
        {
            var chunks = transactions
                .Select((tx, index) => new { tx, index })
                .GroupBy(x => x.index / limit)
                .Select((g, i) =>
                {
                    var links = new Links(next: null);
                    return new TransactionsLinked(g.Select(x => x.tx).ToList(), links);
                })
                .ToList();

            chunks.Add(new TransactionsLinked(new List<Transaction>(), null));

            return chunks;
        }

        public static void MockMultiPaginatedResponses(this MockHttpMessageHandler handlerMock,
            List<TransactionsLinked> pages, string baseUrl, string basePathUrl, int limit)
        {
            var currentLink = $"{basePathUrl}?page=1&limit={limit}";
            var linkPaginationPages = pages.Take(1).ToList();

            for (int i = 0; i < linkPaginationPages.Count; i++)
            {
                var page = linkPaginationPages[i];
                var currentUrl = $"{baseUrl}{currentLink}";

                handlerMock.When(currentUrl)
                    .With(req =>
                    {
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        return true;
                    })
                    .Respond(HttpStatusCode.OK, JsonContent.Create(page));
                currentLink = page.Links?.Next;
            }

            for (int i = 1; i < pages.Count; i++)
            {
                var content = JsonContent.Create(pages[i]);
                var requestUrl = $"{baseUrl}{basePathUrl}?page={i + 1}&limit={limit}";

                handlerMock.When(requestUrl)
                    .With(req =>
                    {
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        return true;
                    })
                    .Respond(HttpStatusCode.OK, content);
            }
        }
    }
}
