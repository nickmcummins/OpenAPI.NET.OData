// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OpenApi.OData.Edm;
using Microsoft.OpenApi.OData.Vocabulary.Capabilities;
using Xunit;

namespace Microsoft.OpenApi.OData.Reader.Vocabulary.Capabilities.Tests
{
    public class FilterRestrictionsTypeTests
    {
        [Fact]
        public void KindPropertyReturnsFilterRestrictionsEnumMember()
        {
            // Arrange & Act
            FilterRestrictionsType filter = new FilterRestrictionsType();

            // Assert
            // Assert.Equal(CapabilitesTermKind.FilterRestrictions, filter.Kind);
        }

        [Fact]
        public void UnknownAnnotatableTargetReturnsDefaultFilterRestrictionsValues()
        {
            // Arrange
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");

            //  Act
            FilterRestrictionsType filter = EdmCoreModel.Instance.GetRecord<FilterRestrictionsType>(entityType);

            // Assert
            Assert.Null(filter);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntityTypeReturnsCorrectfilterRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
        {
            // Arrange
            const string template = @"
                <Annotations Target=""NS.Calendar"">
                  {0}
                </Annotations>";

            IEdmModel model = GetEdmModel(template, location);
            Assert.NotNull(model); // guard

            IEdmEntitySet calendars = model.EntityContainer.FindEntitySet("Calendars");
            Assert.NotNull(calendars); // guard

            // Act
            FilterRestrictionsType filter = model.GetRecord<FilterRestrictionsType>(calendars);

            // Assert
            VerifyFilterRestrictions(filter);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntitySetReturnsCorrectFilterRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
        {
            // Arrange
            const string template = @"
                <Annotations Target=""NS.Default/Calendars"">
                  {0}
                </Annotations>";

            IEdmModel model = GetEdmModel(template, location);
            Assert.NotNull(model); // guard

            IEdmEntitySet calendars = model.EntityContainer.FindEntitySet("Calendars");
            Assert.NotNull(calendars); // guard

            // Act
            FilterRestrictionsType filter = model.GetRecord<FilterRestrictionsType>(calendars);

            // Assert
            VerifyFilterRestrictions(filter);
        }

        private static IEdmModel GetEdmModel(string template, EdmVocabularyAnnotationSerializationLocation location)
        {
            string countAnnotation = @"
                <Annotation Term=""Org.OData.Capabilities.V1.FilterRestrictions"" >
                  <Record>
                    <PropertyValue Property=""Filterable"" Bool=""false"" />
                    <PropertyValue Property=""RequiresFilter"" Bool=""false"" />
                    <PropertyValue Property=""RequiredProperties"" >
                      <Collection>
                        <PropertyPath>Id</PropertyPath>
                      </Collection>
                    </PropertyValue>
                    <PropertyValue Property=""NonFilterableProperties"" >
                      <Collection>
                        <PropertyPath>Emails</PropertyPath>
                      </Collection>
                    </PropertyValue>
                  </Record>
                </Annotation>";

            if (location == EdmVocabularyAnnotationSerializationLocation.OutOfLine)
            {
                countAnnotation = string.Format(template, countAnnotation);
                return CapabilitiesModelHelper.GetEdmModelOutline(countAnnotation);
            }
            else
            {
                return CapabilitiesModelHelper.GetEdmModelTypeInline(countAnnotation);
            }
        }

        private static void VerifyFilterRestrictions(FilterRestrictionsType filter)
        {
            Assert.NotNull(filter);

            Assert.NotNull(filter.Filterable);
            Assert.False(filter.Filterable.Value);

            Assert.NotNull(filter.RequiresFilter);
            Assert.False(filter.RequiresFilter.Value);

            Assert.NotNull(filter.RequiredProperties);
            Assert.Single(filter.RequiredProperties);
            Assert.Equal("Id", filter.RequiredProperties.First());

            Assert.NotNull(filter.NonFilterableProperties);
            Assert.Single(filter.NonFilterableProperties);
            Assert.Equal("Emails", filter.NonFilterableProperties.First());

            Assert.True(filter.IsRequiredProperty("Id"));
            Assert.False(filter.IsRequiredProperty("ID"));

            Assert.True(filter.IsNonFilterableProperty("Emails"));
            Assert.False(filter.IsNonFilterableProperty("ID"));
        }
    }
}
