using Sitecore.Abstractions;
using Sitecore.Connector.CMP;
using Sitecore.Connector.CMP.Conversion;
using Sitecore.Connector.CMP.Helpers;
using Sitecore.Connector.CMP.Pipelines.ImportEntity;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using System.Globalization;
using System;
using System.Linq;

namespace ExtendCmpConnector.Pipelines.ImportEntity
{
    public class SaveFieldValues : Sitecore.Connector.CMP.Pipelines.ImportEntity.SaveFieldValues
    {
        /// <summary>
        /// The ID of the template to process.
        /// </summary>
        public static readonly ID CultureAwareFieldMappingTemplateId = ID.Parse("0C3D8C9B-3B46-4627-A882-C599141FABA8");

        /// <summary>
        /// The ID of the field containing culture.
        /// </summary>
        public static readonly ID FieldMappingCmpCultureFieldId = ID.Parse("D5263155-E14E-47E7-A75F-578C4923595B");

        /// <summary>
        /// The mapper which is a private field in base class and needed to be duplicated.
        /// </summary>
        private ICmpConverterMapper _mapper;

        /// <summary>
        /// The constructor's parameters will be filled automatically. Thank you DI!
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cmpHelper">The helper.</param>
        /// <param name="settings">The settings.</param>
        public SaveFieldValues(
            ICmpConverterMapper mapper,
            BaseLog logger,
            CmpHelper cmpHelper,
            CmpSettings settings
            ) : base(mapper, logger, cmpHelper, settings)
        {
            this._mapper = mapper;
        }

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">Pipeline arguments.</param>
        /// <param name="logger">The logger</param>
        public override void Process(ImportEntityPipelineArgs args, BaseLog logger)
        {
            Assert.IsNotNull(args.Item, "The item is null.");
            Assert.IsNotNull(args.Language, "The language is null.");
            using (new SecurityDisabler())
            {
                using (new LanguageSwitcher(args.Language))
                {
                    bool success = false;
                    try
                    {
                        args.Item.Editing.BeginEdit();
                        success = this.TryMapConfiguredFields(args);
                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {
                        // In case of a failure (eg. incorrect culture) the editing process is cancelled.
                        if (success)
                        {
                            args.Item.Editing.EndEdit();
                        }
                        else
                        {
                            args.Item.Editing.CancelEdit();
                        }
                    }
                }
            }
            base.Process(args, logger);
        }

        /// <summary>
        /// This is the method that does all the hard work.
        /// </summary>
        /// <param name="args">Pipeline arguments.</param>
        /// <returns>true if the mapping is successful, false otherwise.</returns>
        internal virtual bool TryMapConfiguredFields(ImportEntityPipelineArgs args)
        {
            // I am only interested in items inheriting from the new template
            foreach (Item obj in args.EntityMappingItem.Children.Where(i => i.TemplateID == CultureAwareFieldMappingTemplateId))
            {
                // Here I'm getting all the data from the mapping
                string fieldName = obj[Constants.FieldMappingSitecoreFieldNameFieldId];
                string chFieldName = obj[Constants.FieldMappingCmpFieldNameFieldId];
                string chCulture = obj[FieldMappingCmpCultureFieldId];

                // I only want to progress if all the data is not empty.
                if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(chFieldName) && !string.IsNullOrEmpty(chCulture))
                {
                    try
                    {
                        // This is a potential problematic point - getting culture info can throw exception.
                        var culture = CultureInfo.GetCultureInfo(chCulture);

                        // Finally, I'm using the <code>GetPropertyValue</code> method with culture.
                        args.Item[fieldName] = this._mapper.Convert(
                            args.EntityDefinition,
                            chFieldName,
                            args.Entity.GetPropertyValue(chFieldName, culture)
                            );
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}