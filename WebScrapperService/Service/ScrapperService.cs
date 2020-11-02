using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebScrapperService.Models;

namespace WebScrapperService.Service
{
    public class ScrapperService
    {
        private readonly IMongoCollection<SiteTags> _books;

        public ScrapperService(IScrapperDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _books = database.GetCollection<SiteTags>(settings.ScrapperCollectionName);
        }

        public List<SiteTags> Get() =>
            _books.Find(book => true).ToList();

        public SiteTags Get(string id) =>
            _books.Find<SiteTags>(book => book.SitetagsId == id).FirstOrDefault();

        public SiteTags Create(SiteTags book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, SiteTags bookIn) =>
            _books.ReplaceOne(book => book.SitetagsId == id, bookIn);

        public void Remove(SiteTags bookIn) =>
            _books.DeleteOne(book => book.SitetagsId == bookIn.SitetagsId);

        public void Remove(string id) =>
            _books.DeleteOne(book => book.SitetagsId == id);
    }
}
