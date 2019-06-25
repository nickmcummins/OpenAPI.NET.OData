// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OpenApi.OData.Capabilities;
using Microsoft.OpenApi.OData.Edm;
using Microsoft.OpenApi.OData.Vocabulary.Capabilities;
using Xunit;

namespace Microsoft.OpenApi.OData.Reader.Vocabulary.Capabilities.Tests
{
    public class NavigationRestrictionsTypeTests
    {
        [Fact]
        public void KindPropertyReturnsNavigationRestrictionsEnumMember()
        {
            // Arrange & Act
            NavigationRestrictionsType navigation = new NavigationRestrictionsType();

            // Assert
           // Assert.Equal(CapabilitesTermKind.NavigationRestrictions, navigation.Kind);
        }

        [Fact]
        public void UnknownAnnotatableTargetReturnsDefaultPropertyValues()
        {
            // Arrange
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");

            // Act
            NavigationRestrictionsType navigation = EdmCoreModel.Instance.GetRecord<NavigationRestrictionsType>(entityType);

            // Assert
            Assert.Null(navigation);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntityTypeReturnsCorrectNavigationRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
        {
            // Arrange
            const string outOfLineTemplate = @"
                <Annotations Target=""NS.Calendar"">
                  {0}
                </Annotations>";

            string navigationAnnotation = @"
                <Annotation Term=""Org.OData.Capabilities.V1.NavigationRestrictions"" >
                  <Record>
                    <PropertyValue Property=""Navigability"" >
                      <EnumMember>Org.OData.Capabilities.V1.NavigationType/Recursive</EnumMember>
                    </PropertyValue>
                    <PropertyValue Property=""RestrictedProperties"" >
                      <Collection>
                        <Record>
                          <PropertyValue Property=""Navigability"" >
                            <EnumMember>Org.OData.Capabilities.V1.NavigationType/Single</EnumMember>
                          </PropertyValue>
                          <PropertyValue Property=""NavigationProperty"" NavigationPropertyPath=""abc"" />
                         </Record>
                       </Collection>
                     </PropertyValue>
                   </Record>
                 </Annotation>";

            IEdmModel model;
            if (location == EdmVocabularyAnnotationSerializationLocation.OutOfLine)
            {
                navigationAnnotation = string.Format(outOfLineTemplate, navigationAnnotation);
                model = CapabilitiesModelHelper.GetEdmModelOutline(navigationAnnotation);
            }
            else
            {
                model = CapabilitiesModelHelper.GetEdmModelTypeInline(navigationAnnotation);
            }

            IEdmEntitySet calendars = model.EntityContainer.FindEntitySet("Calendars");

            // Act
            NavigationRestrictionsType navigation = model.GetRecord<NavigationRestrictionsType>(calendars);

            // Assert
            Assert.NotNull(navigation.Navigability);
            Assert.Equal(NavigationType.Recursive, navigation.Navigability.Value);

            Assert.NotNull(navigation.RestrictedProperties);
            NavigationPropertyRestriction navRestriction = Assert.Single(navigation.RestrictedProperties);
            Assert.NotNull(navRestriction.Navigability);
            Assert.Equal(NavigationType.Single, navRestriction.Navigability.Value);
            Assert.Equal("abc", navRestriction.NavigationProperty);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntitySetReturnsCorrectNavigationRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
        {
            // Arrange
            const string outOfLineTemplate = @"
                <Annotations Target=""NS.Default/Calendars"">
                  {0}
                </Annotations>";

            string navigationAnnotation = @"
                <Annotation Term=""Org.OData.Capabilities.V1.NavigationRestrictions"" >
                  <Record>
                    <PropertyValue Property=""Navigability"" >
                      <EnumMember>Org.OData.Capabilities.V1.NavigationType/Recursive</EnumMember>
                    </PropertyValue>
                    <PropertyValue Property=""RestrictedProperties"" >
                      <Collection>
                        <Record>
                          <PropertyValue Property=""Navigability"" >
                            <EnumMember>Org.OData.Capabilities.V1.NavigationType/Single</EnumMember>
                          </PropertyValue>
                          <PropertyValue Property=""NavigationProperty"" NavigationPropertyPath=""abc"" />
                         </Record>
                         <Record>
                          <PropertyValue Property=""Navigability"" >
                            <EnumMember>Org.OData.Capabilities.V1.NavigationType/None</EnumMember>
                          </PropertyValue>
                          <PropertyValue Property=""NavigationProperty"" NavigationPropertyPath=""xyz"" />
                         </Record>
                       </Collection>
                     </PropertyValue>
                   </Record>
                 </Annotation>";

            IEdmModel model;
            if (location == EdmVocabularyAnnotationSerializationLocation.OutOfLine)
            {
                navigationAnnotation = string.Format(outOfLineTemplate, navigationAnnotation);
                model = CapabilitiesModelHelper.GetEdmModelOutline(navigationAnnotation);
            }
            else
            {
                model = CapabilitiesModelHelper.GetEdmModelTypeInline(navigationAnnotation);
            }

            IEdmEntitySet calendars = model.EntityContainer.FindEntitySet("Calendars");

            // Act
            NavigationRestrictionsType navigation = model.GetRecord<NavigationRestrictionsType>(calendars);

            // Assert
            VerifyNavigationRestrictions(navigation);

            NavigationPropertyRestriction navRestriction = navigation.RestrictedProperties.Last();
            Assert.NotNull(navRestriction.Navigability);
            Assert.Equal(NavigationType.None, navRestriction.Navigability.Value);
            Assert.Equal("xyz", navRestriction.NavigationProperty);
            Assert.True(navigation.IsRestrictedProperty("xyz"));
        }

        [Fact]
        public void TargetWithUnknownEnumMemberDoesnotReturnsNavigationRestrictionsValue()
        {
            // Arrange
            const string navigationAnnotation = @"
              <Annotations Target=""NS.Calendar"" >
                  <Annotation Term=""Org.OData.Capabilities.V1.NavigationRestrictions"">
                    <Record>
                      <PropertyValue Property=""Navigability"" >
                        <EnumMember>Org.OData.Capabilities.V1.NavigationType/Unknown</EnumMember>
                    </PropertyValue>
                    </Record>
                  </Annotation>
              </Annotations>";

            IEdmModel model = CapabilitiesModelHelper.GetEdmModelOutline(navigationAnnotation);
            IEdmEntityType calendar = model.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Calendar");

            // Act
            NavigationRestrictionsType navigation = model.GetRecord<NavigationRestrictionsType>(calendar);

            // Assert
            Assert.Null(navigation.Navigability);
            Assert.Null(navigation.RestrictedProperties);
        }

        private static void VerifyNavigationRestrictions(NavigationRestrictionsType navigation)
        {
            Assert.NotNull(navigation);
            Assert.True(navigation.IsNavigable);

            Assert.NotNull(navigation.Navigability);
            Assert.Equal(NavigationType.Recursive, navigation.Navigability.Value);

            Assert.NotNull(navigation.RestrictedProperties);
            Assert.Equal(2, navigation.RestrictedProperties.Count);

            NavigationPropertyRestriction navRestriction = navigation.RestrictedProperties.First();
            Assert.NotNull(navRestriction.Navigability);
            Assert.Equal(NavigationType.Single, navRestriction.Navigability.Value);
            Assert.Equal("abc", navRestriction.NavigationProperty);
            Assert.False(navigation.IsRestrictedProperty("abc"));
        }
    }
}
