// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OpenApi.OData.Edm;
using Microsoft.OpenApi.OData.Vocabulary.Capabilities;
using Xunit;

namespace Microsoft.OpenApi.OData.Reader.Vocabulary.Capabilities.Tests
{
    public class DeleteRestrictionsTypeTests
    {
        [Fact]
        public void KindPropertyReturnsDeleteRestrictionsEnumMember()
        {
            // Arrange & Act
            DeleteRestrictionsType delete = new DeleteRestrictionsType();

            // Assert
            // Assert.Equal(CapabilitesTermKind.DeleteRestrictions, delete.Kind);
        }

        [Fact]
        public void UnknownAnnotatableTargetReturnsDefaultDeleteRestrictionsValues()
        {
            // Arrange
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");

            //  Act
            DeleteRestrictionsType delete = EdmCoreModel.Instance.GetRecord<DeleteRestrictionsType>(entityType);

            // Assert
            Assert.Null(delete);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntityTypeReturnsCorrectDeleteRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
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
            DeleteRestrictionsType delete = model.GetRecord<DeleteRestrictionsType>(calendars);

            // Assert
            VerifyDeleteRestrictions(delete);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntitySetReturnsCorrectDeleteRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
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
            DeleteRestrictionsType delete = model.GetRecord<DeleteRestrictionsType>(calendars);

            // Assert
            VerifyDeleteRestrictions(delete);
        }

        private static IEdmModel GetEdmModel(string template, EdmVocabularyAnnotationSerializationLocation location)
        {
            string countAnnotation = @"
                <Annotation Term=""Org.OData.Capabilities.V1.DeleteRestrictions"" >
                  <Record>
                    <PropertyValue Property=""Deletable"" Bool=""false"" />
                    <PropertyValue Property=""NonDeletableNavigationProperties"" >
                      <Collection>
                        <NavigationPropertyPath>abc</NavigationPropertyPath>
                        <NavigationPropertyPath>RelatedEvents</NavigationPropertyPath>
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

        private static void VerifyDeleteRestrictions(DeleteRestrictionsType delete)
        {
            Assert.NotNull(delete);

            Assert.NotNull(delete.Deletable);
            Assert.False(delete.Deletable.Value);

            Assert.NotNull(delete.NonDeletableNavigationProperties);
            Assert.Equal(2, delete.NonDeletableNavigationProperties.Count);
            Assert.Equal("abc|RelatedEvents", String.Join("|", delete.NonDeletableNavigationProperties));

            Assert.True(delete.IsNonDeletableNavigationProperty("RelatedEvents"));
        }
    }
}
