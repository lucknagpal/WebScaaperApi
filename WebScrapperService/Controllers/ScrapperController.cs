using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebScrapperService.Models;
using WebScrapperService.Service;

namespace WebScrapperService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapperController : ControllerBase
    {

        private readonly ScrapperService _bookService;

        public ScrapperController(ScrapperService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/Scrapper
        [HttpGet]
        public ActionResult<List<SiteTags>> Get() =>
             _bookService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        public ActionResult<SiteTags> Get(string id)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        public ActionResult<SiteTags> Create(SiteTags book)
        {
            _bookService.Create(book);

            return CreatedAtRoute("GetBook", new { id = book.SitetagsId.ToString() }, book);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, SiteTags bookIn)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _bookService.Update(id, bookIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _bookService.Remove(book.SitetagsId);

            return NoContent();
        }
    }
}
