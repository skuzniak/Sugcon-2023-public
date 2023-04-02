using ResponsiveImages.Models;
using System.Collections.Generic;

namespace ResponsiveImages.Services
{
    public interface IResponsiveConfigurationProvider
    {
        IEnumerable<ResponsiveImageModel> GetResponsiveConfiguration();
    }
}
