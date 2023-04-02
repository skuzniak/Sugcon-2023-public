using ResponsiveImages.Models;
using System.Collections.Generic;

namespace ResponsiveImages.Services
{
    public class ResponsiveConfigurationProvider : IResponsiveConfigurationProvider
    {
        public IEnumerable<ResponsiveImageModel> GetResponsiveConfiguration()
        {
            return new[]
            {
                new ResponsiveImageModel{ Transformation = "mobile", MediaQuery = "(max-width: 719px)" },
                new ResponsiveImageModel{ Transformation = "full", MediaQuery = "(min-width: 720px)" }
            };
        }
    }
}