// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.OData.Common;
using Microsoft.OpenApi.OData.Vocabulary;

namespace Microsoft.OpenApi.OData.Edm
{
    /// <summary>
    /// Vocabulary Annotation Extension methods for <see cref="IEdmModel"/>
    /// </summary>
    internal static class EdmVocabularyAnnotationExtensions
    {
        private static IDictionary<IEdmVocabularyAnnotatable, IDictionary<string, object>> _cachedAnnotations;
        private static IEdmModel _savedModel = null; // if diffenent model, the cache will be cleaned.
        private static object _objectLock = new object();

        public static bool? GetBoolean(this IEdmModel model, IEdmVocabularyAnnotatable target, string qualifiedName)
        {
            Utils.CheckArgumentNull(model, nameof(model));
            Utils.CheckArgumentNull(target, nameof(target));
            Utils.CheckArgumentNull(qualifiedName, nameof(qualifiedName));

            return GetOrAddCached<bool?>(model, target, qualifiedName, () =>
            {
                IEdmTerm term = model.FindTerm(qualifiedName);
                if (term != null)
                {
                    IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
                    if (annotation != null && annotation.Value != null && annotation.Value.ExpressionKind == EdmExpressionKind.StringConstant)
                    {
                        IEdmBooleanConstantExpression boolConstant = (IEdmBooleanConstantExpression)annotation.Value;
                        if (boolConstant != null)
                        {
                            return boolConstant.Value;
                        }
                    }
                }

                return null;
            });
        }

        /// <summary>
        /// Gets the string term value for the given <see cref="IEdmVocabularyAnnotatable"/>.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="target">The Edm target.</param>
        /// <param name="qualifiedName">The Term qualified name.</param>
        /// <returns>Null or the string value for this annotation.</returns>
        public static string GetString(this IEdmModel model, IEdmVocabularyAnnotatable target, string qualifiedName)
        {
            Utils.CheckArgumentNull(model, nameof(model));
            Utils.CheckArgumentNull(target, nameof(target));
            Utils.CheckArgumentNull(qualifiedName, nameof(qualifiedName));

            return GetOrAddCached(model, target, qualifiedName, () =>
            {
                IEdmTerm term = model.FindTerm(qualifiedName);
                if (term != null)
                {
                    IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
                    if (annotation != null && annotation.Value != null && annotation.Value.ExpressionKind == EdmExpressionKind.StringConstant)
                    {
                        IEdmStringConstantExpression stringConstant = (IEdmStringConstantExpression)annotation.Value;
                        return stringConstant.Value;
                    }
                }

                return null;
            });
        }

        /// <summary>
        /// Gets the collection of string term value for the given <see cref="IEdmVocabularyAnnotatable"/>.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="target">The Edm target.</param>
        /// <param name="qualifiedName">The Term qualified name.</param>
        /// <returns>Null or the collection of string value for this annotation.</returns>
        public static IEnumerable<string> GetCollection(this IEdmModel model, IEdmVocabularyAnnotatable target, string qualifiedName)
        {
            Utils.CheckArgumentNull(model, nameof(model));
            Utils.CheckArgumentNull(target, nameof(target));
            Utils.CheckArgumentNull(qualifiedName, nameof(qualifiedName));

            return GetOrAddCached(model, target, qualifiedName, () =>
            {
                IEdmTerm term = model.FindTerm(qualifiedName);
                if (term != null)
                {
                    IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
                    if (annotation != null && annotation.Value != null && annotation.Value.ExpressionKind == EdmExpressionKind.Collection)
                    {
                        IEdmCollectionExpression collection = (IEdmCollectionExpression)annotation.Value;
                        if (collection.Elements != null)
                        {
                            return collection.Elements.Select(e => ((IEdmStringConstantExpression)e).Value);
                        }
                    }
                }

                return null;
            });
        }

        /// <summary>
        /// Gets the record value (a complex type) for the given <see cref="IEdmVocabularyAnnotatable"/>.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="target">The Edm target.</param>
        /// <param name="qualifiedName">The Term qualified name.</param>
        /// <returns>Null or the record value (a complex type) for this annotation.</returns>
        public static T GetRecord<T>(this IEdmModel model, IEdmVocabularyAnnotatable target, string qualifiedName)
            where T : IRecord, new()
        {
            Utils.CheckArgumentNull(model, nameof(model));
            Utils.CheckArgumentNull(target, nameof(target));
            Utils.CheckArgumentNull(qualifiedName, nameof(qualifiedName));

            return GetOrAddCached(model, target, qualifiedName, () =>
            {
                IEdmTerm term = model.FindTerm(qualifiedName);
                if (term != null)
                {
                    IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
                    if (annotation != null && annotation.Value != null && annotation.Value.ExpressionKind == EdmExpressionKind.Record)
                    {
                        IEdmRecordExpression recordExpression = (IEdmRecordExpression)annotation.Value;
                        T newRecord = new T();
                        newRecord.Initialize(recordExpression);
                        return newRecord;
                    }
                }

                return default;
            });
        }

        /// <summary>
        /// Gets the collection of record value (a complex type) for the given <see cref="IEdmVocabularyAnnotatable"/>.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="target">The Edm target.</param>
        /// <param name="qualifiedName">The Term qualified name.</param>
        /// <returns>Null or the colllection of record value (a complex type) for this annotation.</returns>
        public static IEnumerable<T> GetCollection<T>(this IEdmModel model, IEdmVocabularyAnnotatable target, string qualifiedName)
            where T : IRecord, new()
        {
            Utils.CheckArgumentNull(model, nameof(model));
            Utils.CheckArgumentNull(target, nameof(target));
            Utils.CheckArgumentNull(qualifiedName, nameof(qualifiedName));

            return GetOrAddCached<IEnumerable<T>>(model, target, qualifiedName, () =>
            {
                IEdmTerm term = model.FindTerm(qualifiedName);
                if (term != null)
                {
                    IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
                    if (annotation != null && annotation.Value != null && annotation.Value.ExpressionKind == EdmExpressionKind.Collection)
                    {
                        IEdmCollectionExpression collection = (IEdmCollectionExpression)annotation.Value;
                        if (collection.Elements != null)
                        {
                            return collection.Elements.Select(e =>
                            {
                                Debug.Assert(e.ExpressionKind == EdmExpressionKind.Record);

                                IEdmRecordExpression recordExpression = (IEdmRecordExpression)e;
                                T newRecord = new T();
                                newRecord.Initialize(recordExpression);
                                return newRecord;
                            });
                        }
                    }
                }

                return null;
            });
        }

        /// <summary>
        /// Gets the vocabulary annotations from a target annotatable.
        /// </summary>
        /// <param name="model">The model referenced to.</param>
        /// <param name="target">The target Annotatable to find annotation</param>
        /// <returns>The annotations or null.</returns>
        public static IEnumerable<IEdmVocabularyAnnotation> GetVocabularyAnnotations(this IEdmModel model,
            IEdmVocabularyAnnotatable target, string qualifiedName)
        {
            Utils.CheckArgumentNull(model, nameof(model));
            Utils.CheckArgumentNull(target, nameof(target));
            Utils.CheckArgumentNull(qualifiedName, nameof(qualifiedName));

            IEdmTerm term = model.FindTerm(qualifiedName);
            if (term != null)
            {
                return model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term);
            }

            return Enumerable.Empty<IEdmVocabularyAnnotation>();
        }

        /// <summary>
        /// Gets the vocabulary annotation from a target annotatable.
        /// </summary>
        /// <param name="model">The model referenced to.</param>
        /// <param name="target">The target Annotatable to find annotation</param>
        /// <returns>The annotation or null.</returns>
        public static IEdmVocabularyAnnotation GetVocabularyAnnotation(this IEdmModel model, IEdmVocabularyAnnotatable target, string qualifiedName)
        {
            Utils.CheckArgumentNull(model, nameof(model));
            Utils.CheckArgumentNull(target, nameof(target));
            Utils.CheckArgumentNull(qualifiedName, nameof(qualifiedName));

            IEdmTerm term = model.FindTerm(qualifiedName);
            if (term != null)
            {
                IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
                if (annotation != null)
                {
                    return annotation;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the vocabulary annotation from a target annotatable.
        /// </summary>
        /// <param name="model">The model referenced to.</param>
        /// <param name="target">The target Annotatable to find annotation</param>
        /// <returns>The annotation or null.</returns>
        public static IEdmVocabularyAnnotation GetVocabularyAnnotation(this IEdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term)
        {
            Utils.CheckArgumentNull(model, nameof(model));
            Utils.CheckArgumentNull(target, nameof(target));
            Utils.CheckArgumentNull(term, nameof(term));

            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
            if (annotation != null)
            {
                return annotation;
            }

            return null;
        }

        private static T GetOrAddCached<T>(this IEdmModel model, IEdmVocabularyAnnotatable target, string qualifiedName, Func<T> createFunc)
        {
            if (model == null || target == null)
            {
                return default;
            }

            lock (_objectLock)
            {
                if (!ReferenceEquals(_savedModel, model))
                {
                    if (_cachedAnnotations != null)
                    {
                        _cachedAnnotations.Clear();
                    }

                    _savedModel = model;
                }

                if (_cachedAnnotations == null)
                {
                    _cachedAnnotations = new Dictionary<IEdmVocabularyAnnotatable, IDictionary<string, object>>();
                }

                object restriction;
                if (_cachedAnnotations.TryGetValue(target, out IDictionary<string, object> value))
                {
                    // Here means we visited target before and we are sure that the value is not null.
                    if (value.TryGetValue(qualifiedName, out restriction))
                    {
                        T ret = (T)restriction;
                        return ret;
                    }
                    else
                    {
                        T ret = createFunc();
                        value[qualifiedName] = ret;
                        return ret;
                    }
                }

                // It's first time to query this target, create new dictionary and restriction.
                value = new Dictionary<string, object>();
                _cachedAnnotations[target] = value;
                T newAnnotation = createFunc();
                value[qualifiedName] = newAnnotation;
                return newAnnotation;
            }
        }
    }
}
