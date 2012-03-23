using System;
using System.IO;
using System.Web.Mvc;

namespace UploadWeb.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public ActionResult SaveImage(string data)
		{
			string[] mimeTypeBase64 = data.Split(new[] { ';' });

			string base64Data = null;
			string extension = null;

			foreach (string s in mimeTypeBase64)
			{
				int colonIndex = s.IndexOf(':');
				if (colonIndex == -1)
				{
					if (s.ToLower().StartsWith("base64,"))
						base64Data = s.Remove(0, 7);

					continue;
				}

				string[] header = s.Split(':');
				if (header[0].ToLower() != "data")
					continue;

				switch (header[1])
				{
					case "image/png":
						extension = ".png";
						break;

					case "image/jpeg":
						extension = ".jpg";
						break;

					case "image/gif":
						extension = ".gif";
						break;
				}
			}

			// unable to parse the data, do nothing
			if (string.IsNullOrEmpty(base64Data) || string.IsNullOrEmpty(extension))
				return View("Index");

			// TODO: error/exception handling
			// TODO: this code sucks... Proof of concept though -- improve!

			byte[] rawData = Convert.FromBase64String(base64Data);
			string filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + extension;

			using (FileStream fs = new FileStream(Path.Combine(Server.MapPath("~/-"), filename), FileMode.CreateNew))
			{
				fs.Write(rawData, 0, rawData.Length);
			}

			ViewBag.ImageUrl = Request.Url.Scheme + "://" + Request.Url.Host + Url.Content("~/-/" + filename);

			return View();
		}
    }
}
