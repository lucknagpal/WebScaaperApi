using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebScrapperService.Models
{
    public class ScrapperDatabaseSettings: IScrapperDatabaseSettings
    {
        public string ScrapperCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        
    }

    public interface IScrapperDatabaseSettings
    {
        string ScrapperCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
