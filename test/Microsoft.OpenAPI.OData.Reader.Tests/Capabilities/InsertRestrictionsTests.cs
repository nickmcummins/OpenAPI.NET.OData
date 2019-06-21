// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.OData.Capabilities;
using Microsoft.OpenApi.OData.Tests;
using Xunit;

namespace Microsoft.OpenApi.OData.Reader.Capabilities.Tests
{
    public class InsertRestrictionsTests
    {
        [Fact]
        public void OkEdmModel()
        {
            // Arrange & Act
            EdmModel model = new EdmModel();
            EdmEntityType entity = new EdmEntityType("NS", "Entity");
            entity.AddKeys(entity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddElement(entity);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet set = container.AddEntitySet("Entities", entity);
            model.AddElement(container);

            IEdmComplexType securitySchemeType = model.FindType("Org.OData.Authorization.V1.SecurityScheme") as IEdmComplexType;
            IEdmRecordExpression securityScheme = new EdmRecordExpression(
                    new EdmComplexTypeReference(securitySchemeType, false),
                    new EdmPropertyConstructor("Authorization", new EdmStringConstant("authorizationName")),
                    new EdmPropertyConstructor("RequiredScopes", new EdmCollectionExpression(new EdmStringConstant("RequiredScopes1"), new EdmStringConstant("RequiredScopes2"))));

            IEdmComplexType scopeType = model.FindType("Org.OData.Capabilities.V1.ScopeType") as IEdmComplexType;
            IEdmRecordExpression scope1 = new EdmRecordExpression(
                    new EdmComplexTypeReference(scopeType, false),
                    new EdmPropertyConstructor("Scope", new EdmStringConstant("scopeName1")),
                    new EdmPropertyConstructor("RestrictedProperties", new EdmStringConstant("p1,p2")));

            IEdmRecordExpression scope2 = new EdmRecordExpression(
                    new EdmComplexTypeReference(scopeType, false),
                    new EdmPropertyConstructor("Scope", new EdmStringConstant("scopeName2")),
                    new EdmPropertyConstructor("RestrictedProperties", new EdmStringConstant("p3,p4")));

            IEdmComplexType permissionType = model.FindType("Org.OData.Capabilities.V1.PermissionType") as IEdmComplexType;
            IEdmRecordExpression permission = new EdmRecordExpression(
                    new EdmComplexTypeReference(permissionType, false),
                    new EdmPropertyConstructor("Scheme", securityScheme),
                    new EdmPropertyConstructor("Scopes", new EdmCollectionExpression(scope1, scope2)));


            IEdmComplexType complex = model.FindType("Org.OData.Capabilities.V1.InsertRestrictionsType") as IEdmComplexType;

            IEdmCollectionExpression nonInsertableProperties = new EdmCollectionExpression
            (
                new EdmPathExpression("abc"),
                new EdmPathExpression("xyz")
            );

            IEdmCollectionExpression nonInsertableNavigationProperties = new EdmCollectionExpression
            (
                new EdmNavigationPropertyPathExpression("nvabc"),
                new EdmNavigationPropertyPathExpression("nvxyz")
            );

            IEdmRecordExpression queryOptions = new EdmRecordExpression(
                    new EdmPropertyConstructor("ExpandSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("SelectSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("ComputeSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("FilterSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("SearchSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("SortSupported", new EdmBooleanConstant(false)),
                    new EdmPropertyConstructor("SortSupported", new EdmBooleanConstant(false)));

            IEdmRecordExpression primitiveExampleValue = new EdmRecordExpression(
                new EdmPropertyConstructor("Description", new EdmStringConstant("Description23")),
                new EdmPropertyConstructor("Value", new EdmStringConstant("Value1")));

            IEdmRecordExpression customHeaders1 = new EdmRecordExpression(
                    new EdmPropertyConstructor("Name", new EdmStringConstant("HeadName1")),
                    new EdmPropertyConstructor("Description", new EdmStringConstant("Description1")),
                    new EdmPropertyConstructor("ComputeSupported", new EdmStringConstant("http://any")),
                    new EdmPropertyConstructor("Required", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("ExampleValues", new EdmCollectionExpression(primitiveExampleValue)));

            IEdmRecordExpression customHeaders2 = new EdmRecordExpression(
                    new EdmPropertyConstructor("Name", new EdmStringConstant("HeadName2")),
                    new EdmPropertyConstructor("Description", new EdmStringConstant("Description2")),
                    new EdmPropertyConstructor("ComputeSupported", new EdmStringConstant("http://any")),
                    new EdmPropertyConstructor("Required", new EdmBooleanConstant(false)),
                    new EdmPropertyConstructor("ExampleValues", new EdmCollectionExpression(primitiveExampleValue)));

            IEdmRecordExpression insertRecord = new EdmRecordExpression(
                    new EdmComplexTypeReference(complex, false),
                    new EdmPropertyConstructor("Insertable", new EdmBooleanConstant(false)),
                    new EdmPropertyConstructor("NonInsertableProperties", nonInsertableProperties),
                    new EdmPropertyConstructor("NonInsertableNavigationProperties", nonInsertableNavigationProperties),
                    new EdmPropertyConstructor("MaxLevels", new EdmIntegerConstant(8)),
                    new EdmPropertyConstructor("Permission", permission),
                    new EdmPropertyConstructor("QueryOptions", queryOptions),
                    new EdmPropertyConstructor("CustomHeaders", new EdmCollectionExpression(customHeaders1, customHeaders2)),
                    new EdmPropertyConstructor("CustomQueryOptions", new EdmCollectionExpression(customHeaders1, customHeaders2))
                    );

            IEdmTerm term = model.FindTerm("Org.OData.Capabilities.V1.InsertRestrictions");
            var annotation = new EdmVocabularyAnnotation(set, term, insertRecord);

            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);


            // Assert
            string csdl = EdmModelHelper.GetCsdl(model);
            Assert.Equal("", csdl);
        }

        [Fact]
        public void KindPropertyReturnsInsertRestrictionsEnumMember()
        {
            // Arrange & Act
            InsertRestrictions insert = new InsertRestrictions();

            // Assert
            Assert.Equal(CapabilitesTermKind.InsertRestrictions, insert.Kind);
        }

        [Fact]
        public void KindPropertyReturnsInsertRestrictionsEnumMember2()
        {
            // Arrange
            IEdmModel model = GetModelWithInsertRestrictions();
            IEdmEntitySet entitySet = model.EntityContainer.FindEntitySet("Entities");

            // Act
            InsertRestrictions insert = new InsertRestrictions();
            insert.Load(model, entitySet);

            // Assert
            Assert.Equal(CapabilitesTermKind.InsertRestrictions, insert.Kind);
        }

        [Fact]
        public void UnknownAnnotatableTargetReturnsDefaultInsertRestrictionsValues()
        {
            // Arrange
            InsertRestrictions insert = new InsertRestrictions();
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");

            //  Act
            bool result = insert.Load(EdmCoreModel.Instance, entityType);

            // Assert
            Assert.False(result);
            Assert.True(insert.IsInsertable);
            Assert.Null(insert.Insertable);
            Assert.Null(insert.NonInsertableNavigationProperties);
            Assert.Null(insert.MaxLevels);
            Assert.Null(insert.Permission);
            Assert.Null(insert.CustomHeaders);
            Assert.Null(insert.CustomQueryOptions);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntityTypeReturnsCorrectInsertRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
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
            InsertRestrictions insert = new InsertRestrictions();
            bool result = insert.Load(model, calendars);

            // Assert
            Assert.True(result);
            VerifyInsertRestrictions(insert);
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void TargetOnEntitySetReturnsCorrectInsertRestrictionsValue(EdmVocabularyAnnotationSerializationLocation location)
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
            InsertRestrictions insert = new InsertRestrictions();
            bool result = insert.Load(model, calendars);

            // Assert
            Assert.True(result);
            VerifyInsertRestrictions(insert);
        }

        private static IEdmModel GetEdmModel(string template, EdmVocabularyAnnotationSerializationLocation location)
        {
            string countAnnotation = @"
                <Annotation Term=""Org.OData.Capabilities.V1.InsertRestrictions"" >
                  <Record>
                    <PropertyValue Property=""Insertable"" Bool=""false"" />
                    <PropertyValue Property=""NonInsertableNavigationProperties"" >
                      <Collection>
                        <NavigationPropertyPath>abc</NavigationPropertyPath>
                        <NavigationPropertyPath>RelatedEvents</NavigationPropertyPath>
                      </Collection>
                    </PropertyValue>
                    <PropertyValue Property=""Insertable"" Bool=""false"" />
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

        public IEdmModel GetModelWithInsertRestrictions()
        {
            EdmModel model = new EdmModel();

            IEdmRecordExpression securityScheme = new EdmRecordExpression(
                    new EdmPropertyConstructor("Authorization", new EdmStringConstant("authorizationName")),
                    new EdmPropertyConstructor("RequiredScopes", new EdmCollectionExpression(new EdmStringConstant("RequiredScopes1"), new EdmStringConstant("RequiredScopes2"))));

            IEdmRecordExpression scope1 = new EdmRecordExpression(
                    new EdmPropertyConstructor("Scope", new EdmStringConstant("scopeName1")),
                    new EdmPropertyConstructor("RestrictedProperties", new EdmStringConstant("p1,p2")));

            IEdmRecordExpression scope2 = new EdmRecordExpression(
                    new EdmPropertyConstructor("Scope", new EdmStringConstant("scopeName2")),
                    new EdmPropertyConstructor("RestrictedProperties", new EdmStringConstant("p3,p4")));

            IEdmRecordExpression permission = new EdmRecordExpression(
                    new EdmPropertyConstructor("Scheme", securityScheme),
                    new EdmPropertyConstructor("Scopes", new EdmCollectionExpression(scope1, scope2)));

            IEdmCollectionExpression nonInsertableProperties = new EdmCollectionExpression
            (
                new EdmPathExpression("abc"),
                new EdmPathExpression("xyz")
            );

            IEdmCollectionExpression nonInsertableNavigationProperties = new EdmCollectionExpression
            (
                new EdmNavigationPropertyPathExpression("nvabc"),
                new EdmNavigationPropertyPathExpression("nvxyz")
            );

            IEdmRecordExpression queryOptions = new EdmRecordExpression(
                    new EdmPropertyConstructor("ExpandSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("SelectSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("ComputeSupported", new EdmBooleanConstant(false)),
                    new EdmPropertyConstructor("FilterSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("SearchSupported", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("SortSupported", new EdmBooleanConstant(false)));

            IEdmRecordExpression primitiveExampleValue = new EdmRecordExpression(
                new EdmPropertyConstructor("Description", new EdmStringConstant("Description23")),
                new EdmPropertyConstructor("Value", new EdmStringConstant("Value1")));

            IEdmRecordExpression customHeaders1 = new EdmRecordExpression(
                    new EdmPropertyConstructor("Name", new EdmStringConstant("HeadName1")),
                    new EdmPropertyConstructor("Description", new EdmStringConstant("Description1")),
                    new EdmPropertyConstructor("ComputeSupported", new EdmStringConstant("http://any")),
                    new EdmPropertyConstructor("Required", new EdmBooleanConstant(true)),
                    new EdmPropertyConstructor("ExampleValues", new EdmCollectionExpression(primitiveExampleValue)));

            IEdmRecordExpression customHeaders2 = new EdmRecordExpression(
                    new EdmPropertyConstructor("Name", new EdmStringConstant("HeadName2")),
                    new EdmPropertyConstructor("Description", new EdmStringConstant("Description2")),
                    new EdmPropertyConstructor("ComputeSupported", new EdmStringConstant("http://any")),
                    new EdmPropertyConstructor("Required", new EdmBooleanConstant(false)),
                    new EdmPropertyConstructor("ExampleValues", new EdmCollectionExpression(primitiveExampleValue)));

            /*
<ComplexType Name="InsertRestrictionsType">
  <Property Name="Insertable" Type="Edm.Boolean" Nullable="false" DefaultValue="true" />
  <Property Name="NonInsertableProperties" Type="Collection(Edm.PropertyPath)" Nullable="false" />
  <Property Name="NonInsertableNavigationProperties" Type="Collection(Edm.NavigationPropertyPath)" Nullable="false" />
  <Property Name="MaxLevels" Type="Edm.Int32" Nullable="false" DefaultValue="-1" />
  <Property Name="Permission" Type="Capabilities.PermissionType" Nullable="true" />
  <Property Name="QueryOptions" Type="Capabilities.ModificationQueryOptionsType" Nullable="true" />
  <Property Name="CustomHeaders" Type="Collection(Capabilities.CustomParameter)" />
  <Property Name="CustomQueryOptions" Type="Collection(Capabilities.CustomParameter)" />
</ComplexType>
             */
            IEdmComplexType complex = model.FindType("Org.OData.Capabilities.V1.InsertRestrictionsType") as IEdmComplexType;
            Assert.NotNull(complex);

            EdmRecordExpression record = new EdmRecordExpression(
                    new EdmComplexTypeReference(complex, false),
                    new EdmPropertyConstructor("Insertable", new EdmBooleanConstant(false)),
                    new EdmPropertyConstructor("NonInsertableProperties", nonInsertableProperties),
                    new EdmPropertyConstructor("NonInsertableNavigationProperties", nonInsertableNavigationProperties),
                    new EdmPropertyConstructor("MaxLevels", new EdmIntegerConstant(8)),
                    new EdmPropertyConstructor("Permission", permission),
                    new EdmPropertyConstructor("QueryOptions", queryOptions),
                    new EdmPropertyConstructor("CustomHeaders", new EdmCollectionExpression(customHeaders1, customHeaders2)),
                    new EdmPropertyConstructor("CustomQueryOptions", new EdmCollectionExpression(customHeaders2)));

            EdmEntityType entity = new EdmEntityType("NS", "Entity");
            entity.AddKeys(entity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddElement(entity);
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet entities = container.AddEntitySet("Entities", entity);
            model.AddElement(container);

            IEdmTerm term = model.FindTerm("Org.OData.Capabilities.V1.InsertRestrictions");
            Assert.NotNull(term);

            var annotation = new EdmVocabularyAnnotation(entities, term, record);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            return model;
        }

        private static void VerifyInsertRestrictions(InsertRestrictions insert)
        {
            Assert.NotNull(insert);

            Assert.NotNull(insert.Insertable);
            Assert.False(insert.Insertable.Value);

            Assert.NotNull(insert.NonInsertableNavigationProperties);
            Assert.Equal(2, insert.NonInsertableNavigationProperties.Count);
            Assert.Equal("abc|RelatedEvents", String.Join("|", insert.NonInsertableNavigationProperties));

            Assert.True(insert.IsNonInsertableNavigationProperty("RelatedEvents"));
            Assert.False(insert.IsNonInsertableNavigationProperty("MyUnknownNavigationProperty"));
        }
    }
}
