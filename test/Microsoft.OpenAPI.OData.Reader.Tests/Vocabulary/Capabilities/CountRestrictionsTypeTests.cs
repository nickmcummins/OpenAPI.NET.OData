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
    public class CountRestrictionsTypeTests
    {
        [Fact]
        public void CountRestrictionsTypeAsTermQualifiedName()
        {
            // Arrange & Act
            
        }

        [Fact]
        public void PropertiesAsNullForDefaultCountRestrictionsType()
        {
            // Arrange & Act
            CountRestrictionsType count = new CountRestrictionsType();

            // Assert
            Assert.Null(count.Countable);
            Assert.Null(count.NonCountableProperties);
            Assert.Null(count.NonCountableNavigationProperties);
        }

        [Fact]
        public void UnknownAnnotatableTargetReturnsDefaultPropertyValues()
        {
            // Arrange & Act
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");

            //  Act
            CountRestrictionsType count = EdmCoreModel.Instance.GetRecord<CountRestrictionsType>(entityType);

            // Assert
            Assert.Null(count);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntityTypeReturnsCorrectCountRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
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
            CountRestrictionsType count = model.GetRecord<CountRestrictionsType>(calendars);

            // Assert
            VerifyCountRestrictions(count);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntitySetReturnsCorrectCountRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
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
            CountRestrictionsType count = model.GetRecord<CountRestrictionsType>(calendars);

            // Assert
            VerifyCountRestrictions(count);
        }

        private static IEdmModel GetEdmModel(string template, EdmVocabularyAnnotationSerializationLocation location)
        {
            string countAnnotation = @"
                <Annotation Term=""Org.OData.Capabilities.V1.CountRestrictions"" >
                  <Record>
                    <PropertyValue Property=""Countable"" Bool=""false"" />
                    <PropertyValue Property=""NonCountableProperties"">
                      <Collection>
                        <PropertyPath>Emails</PropertyPath>
                        <PropertyPath>mij</PropertyPath>
                      </Collection>
                    </PropertyValue>
                    <PropertyValue Property=""NonCountableNavigationProperties"" >
                      <Collection>
                        <NavigationPropertyPath>RelatedEvents</NavigationPropertyPath>
                        <NavigationPropertyPath>abc</NavigationPropertyPath>
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

        private static void VerifyCountRestrictions(CountRestrictionsType count)
        {
            Assert.NotNull(count);

            Assert.NotNull(count.Countable);
            Assert.False(count.Countable.Value);
            Assert.False(count.IsCountable);

            Assert.NotNull(count.NonCountableProperties);
            Assert.Equal(2, count.NonCountableProperties.Count);
            Assert.Equal("Emails|mij", String.Join("|", count.NonCountableProperties));

            Assert.NotNull(count.NonCountableNavigationProperties);
            Assert.Equal(2, count.NonCountableNavigationProperties.Count);
            Assert.Equal("RelatedEvents,abc", String.Join(",", count.NonCountableNavigationProperties));

            Assert.False(count.IsNonCountableProperty("id"));
            Assert.True(count.IsNonCountableProperty("Emails"));
            Assert.True(count.IsNonCountableNavigationProperty("RelatedEvents"));
        }
    }
}
