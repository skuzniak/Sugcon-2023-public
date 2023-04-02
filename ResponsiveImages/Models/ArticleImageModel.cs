using Sitecore.Data.Items;
using System.Collections.Generic;

namespace ResponsiveImages.Models
{
    public class ArticleImageModel
    {
        public string MainImage { get; set; }

        public IEnumerable<ResponsiveImageModel> Images { get; set; }

        public bool IsExperienceEditor { get; set; }
    }

    public class ResponsiveImageModel
    {
        public string Transformation { get; set; }

        public string MediaQuery { get; set; }
    }
}