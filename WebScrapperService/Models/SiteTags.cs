using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebScrapperService.Models
{
    public class SiteTags
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string SitetagsId { get; set; }
        public  string url { get; set; }
        public string Title { get; set; }
        public List<string> MetaKeywords { get; set; }

        public List<string> MetaDescription{ get; set; }

        public string ImageUrl { get; set; }
        public string WebsiteContent { get; set; }
        public List<string> HyperLinks { get; set; }
        public string SocialMediaLinks { get; set; }

  
    }
}
