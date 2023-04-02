using Microsoft.Ajax.Utilities;
using ResponsiveImages.Models;
using ResponsiveImages.Services;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.Resources.Media;
using Sitecore.XA.Foundation.Mvc.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace ResponsiveImages.Controllers
{
    public class ArticleImageController : StandardController
    {
        private IResponsiveConfigurationProvider _responsiveConfigurationProvider;

        public ArticleImageController()
        {
            _responsiveConfigurationProvider = new ResponsiveConfigurationProvider();
        }

        public ActionResult ArticleImage()
        {
            var model = new ArticleImageModel();

            var currentItem = Sitecore.Context.Item;

            if (currentItem != null )
            {
                model.MainImage = GetDamImageUrl(currentItem, "Main Image");

                model.Images = _responsiveConfigurationProvider.GetResponsiveConfiguration();

                model.IsExperienceEditor = Sitecore.Context.PageMode.IsExperienceEditor;
            }

            return View(model);
        }

        private string GetDamImageUrl(Item item, string fieldName)
        {
            var imageField = (ImageField) item.Fields[fieldName];

            if (string.IsNullOrEmpty(imageField?.Value))
            {
                return string.Empty;
            }

            var imageValue = XElement.Parse(imageField.Value);
            return imageValue.GetAttributeValue("src");
        }
    }
}