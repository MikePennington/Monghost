using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonGhost;
using MongoDB.Bson;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            IMongoHelper mongoHelper = new MongoHelper("mongo://127.0.0.1:27017", "monghost-samples");

            var book = new Book {Author = "George Orwell", Title = "1984"};
            mongoHelper.Save(book);
            mongoHelper.FindOne<Book>(book.Id);
        }
    }

    class Book : MongoEntity
    {
        public override ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
