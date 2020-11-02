using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreHtmlToImage;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebScrapperService.Models;
using WebScrapperService.Service;
using System.Drawing;

namespace WebScrapperService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: api/Home

        private readonly ScrapperService _bookService;

        public HomeController(ScrapperService bookService)
        {
            _bookService = bookService;
        }

        public static Image GetImageFromUrl(string url)
        {
            using (var webClient = new WebClient())
            {
                return ByteArrayToImage(webClient.DownloadData(url));
            }
        }

        public static Image ByteArrayToImage(byte[] fileBytes)
        {
            using (var stream = new MemoryStream(fileBytes))
            {
                return Image.FromStream(stream);
            }
        }

        public byte[] CropFile(byte[]  imageBytes, Int32 X, Int32 Y, Int32 Width, Int32 Height)
        {
            //get byte array from profile
            //byte[] imageBytes = profile.Avatar.ToArray();
            //stuff this byte array into a memory stream
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                //write the memory stream out for use
                ms.Write(imageBytes, 0, imageBytes.Length);

                //stuff the memory stream into an image to work with
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms, true);

                //create the destination (cropped) bitmap
                Bitmap bmpCropped = new Bitmap(500, 250);

                //create the graphics object to draw with
                Graphics g = Graphics.FromImage(bmpCropped);

                Rectangle rectDestination = new Rectangle(0, 0, bmpCropped.Width, bmpCropped.Height);
                Rectangle rectCropArea = new Rectangle(X, Y, Width, Height);

                //draw the rectCropArea of the original image to the rectDestination of bmpCropped
                g.DrawImage(img, rectDestination, rectCropArea, GraphicsUnit.Pixel);

                //release system resources
                g.Dispose();

                MemoryStream stream = new MemoryStream();
                bmpCropped.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                Byte[] bytes = stream.ToArray();

                return bytes;
            }
           
        }

        [HttpGet]
        public ActionResult<List<SiteTags>> GetSitedata(string url)
        {
            byte[] image=null;
            string imreBase64Data = "";
            var converter = new HtmlConverter();
            var bytes = converter.FromUrl(url);
            if (bytes.Length > 0)
            {
                 image = CropFile(bytes, 0, 0, 1000, 400);
                 imreBase64Data = Convert.ToBase64String(image);
            }
           
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            List<SiteTags> list = new List<SiteTags>();
            WebClient x = new WebClient();
            string sourcedata = x.DownloadString(url);
            var Title = Regex.Match(sourcedata, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            var getHtmlDoc = new HtmlWeb();
            var document = getHtmlDoc.Load(url);
            var metaTags = document.DocumentNode.SelectNodes("//meta");


            var metadata= GetMetaTagValue(metaTags);
            var UrlLinks = GetUrlLinks(document);
            var metaKeywords = GetMetaKeywords(metaTags);
            SiteTags siteTags = new SiteTags();
            siteTags.Title = Title;
            siteTags.HyperLinks = UrlLinks;
            siteTags.MetaKeywords = metaKeywords;
            siteTags.MetaDescription = metadata;
            siteTags.ImageUrl = imgDataURL;
            //siteTags.ImageData = image;
            list.Add(siteTags);
            return Ok(list);
            
        }

        //// GET: api/Home/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Home
        [HttpPost]
        public void Post([FromBody] List<SiteTags> siteTags)
        {
            _bookService.Create(siteTags[0]);
        }

        // PUT: api/Home/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private List<string> GetUrlLinks(HtmlAgilityPack.HtmlDocument document)
        {
            List<string> list = new List<string>();
          
            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                var attributes = link.Attributes;

                foreach (var item in attributes)
                {
                    if (item.Name == "href" && item.Value.Contains("http"))
                    {
                        list.Add(item.Value);
                       
                    }
                }
                
            }

         

            return list;
        }
        
        private List<string> GetMetaTagValue(HtmlNodeCollection metaTags)
        {
            List<string> list = new List<string>();
           
            
            if (metaTags != null)
            {
                foreach (var sitetag in metaTags)
                {
                    if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value == "description")

                    {
                        list.Add(sitetag.Attributes["content"].Value);
                       
                    }
                }
            }

            return list;
        }

        private List<string> GetMetaKeywords(HtmlNodeCollection metaTags)
        {
            List<string> list = new List<string>();
            if (metaTags != null)
            {
                foreach (var sitetag in metaTags)
                {
                    if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value == "keywords")

                    {
                        list.Add(sitetag.Attributes["content"].Value);

                    }
                }
            }

            return list;
        }
    }
}
