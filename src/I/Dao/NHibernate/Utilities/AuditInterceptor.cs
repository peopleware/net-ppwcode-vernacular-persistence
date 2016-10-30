﻿// Copyright 2010-2016 by PeopleWare n.v..
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

using NHibernate;
using NHibernate.Type;

using PPWCode.Vernacular.Persistence.I.Dao.NHibernate.Interfaces;

namespace PPWCode.Vernacular.Persistence.I.Dao.NHibernate.Utilities
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Castle Windsor usage")]
    [Serializable]
    public class AuditInterceptor : EmptyInterceptor
    {
        private const string CreatedAtPropertyName = "CreatedAt";
        private const string CreatedByPropertyName = "CreatedBy";
        private const string LastModifiedAtPropertyName = "LastModifiedAt";
        private const string LastModifiedByPropertyName = "LastModifiedBy";

        private readonly IIdentityProvider m_IdentityProvider;
        private readonly ConcurrentDictionary<Property, int> m_IndexCache = new ConcurrentDictionary<Property, int>();
        private readonly ITimeProvider m_TimeProvider;
        private readonly bool m_UseUtc;

        public AuditInterceptor(IIdentityProvider identityProvider, ITimeProvider timeProvider, bool useUtc)
        {
            Contract.Requires(identityProvider != null);
            Contract.Requires(timeProvider != null);
            Contract.Ensures(IdentityProvider == identityProvider);
            Contract.Ensures(TimeProvider == timeProvider);

            m_IdentityProvider = identityProvider;
            m_TimeProvider = timeProvider;
            m_UseUtc = useUtc;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(IdentityProvider != null);
            Contract.Invariant(TimeProvider != null);
        }

        private IIdentityProvider IdentityProvider
        {
            get
            {
                Contract.Ensures(Contract.Result<IIdentityProvider>() != null);

                return m_IdentityProvider;
            }
        }

        private ITimeProvider TimeProvider
        {
            get
            {
                Contract.Ensures(Contract.Result<ITimeProvider>() != null);

                return m_TimeProvider;
            }
        }

        public bool UseUtc
        {
            get { return m_UseUtc; }
        }

        private ConcurrentDictionary<Property, int> IndexCache
        {
            get { return m_IndexCache; }
        }

        private void Set(Type entityType, string[] propertyNames, object[] state, string propertyName, object value)
        {
            int index = IndexCache.GetOrAdd(new Property(entityType, propertyName), k => Array.IndexOf(propertyNames, propertyName));
            if (index >= 0)
            {
                state[index] = value;
            }
        }

        private bool SetAuditInfo(object entity, object[] currentState, string[] propertyNames, bool onSave)
        {
            IPersistentObject persistentObject = entity as IPersistentObject;
            if (persistentObject == null)
            {
                return false;
            }

            IInsertAuditableObject insertAuditable = entity as IInsertAuditableObject;
            IAuditableObject updateAuditable = entity as IAuditableObject;
            if ((insertAuditable == null) && (updateAuditable == null))
            {
                return false;
            }

            DateTime time = UseUtc ? TimeProvider.UtcNow : TimeProvider.LocalNow;
            string identityName = IdentityProvider.IdentityName;
            if (identityName == null)
            {
                throw new InvalidOperationException("Unknown IdentityName");
            }

            Type entityType = entity.GetType();

            if (onSave || persistentObject.IsTransient)
            {
                Set(entityType, propertyNames, currentState, CreatedAtPropertyName, time);
                Set(entityType, propertyNames, currentState, CreatedByPropertyName, identityName);

                if (insertAuditable != null)
                {
                    insertAuditable.CreatedAt = time;
                    insertAuditable.CreatedBy = identityName;
                }
                else
                {
                    updateAuditable.CreatedAt = time;
                    updateAuditable.CreatedBy = identityName;
                }
            }

            if (updateAuditable != null)
            {
                Set(entityType, propertyNames, currentState, LastModifiedAtPropertyName, time);
                Set(entityType, propertyNames, currentState, LastModifiedByPropertyName, identityName);

                updateAuditable.LastModifiedAt = time;
                updateAuditable.LastModifiedBy = identityName;
            }

            return true;
        }

        /// <summary>
        ///     Called when an object is detected to be dirty, during a flush.
        /// </summary>
        /// <param name="entity">The given entity.</param>
        /// <param name="id">The id of the given entity.</param>
        /// <param name="currentState">The current state of the entity.</param>
        /// <param name="previousState">The previous state of the entity.</param>
        /// <param name="propertyNames">The property names.</param>
        /// <param name="types">The types.</param>
        /// <remarks>
        ///     The interceptor may modify the detected <c>currentState</c>, which will be propagated to
        ///     both the database and the persistent object. Note that all flushes end in an actual
        ///     synchronization with the database, in which as the new <c>currentState</c> will be propagated
        ///     to the object, but not necessarily (immediately) to the database. It is strongly recommended
        ///     that the interceptor <b>not</b> modify the <c>previousState</c>.
        /// </remarks>
        /// <returns>
        ///     A boolean indicating whether the user modified the  <paramref name="currentState" /> in any way.
        /// </returns>
        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            return SetAuditInfo(entity, currentState, propertyNames, false);
        }

        /// <summary>
        ///     Called before an object is saved.
        /// </summary>
        /// <param name="entity">The given entity.</param>
        /// <param name="id">The id of the given entity.</param>
        /// <param name="state">The state of the entity.</param>
        /// <param name="propertyNames">The property names.</param>
        /// <param name="types">The types.</param>
        /// <remarks>
        ///     The interceptor may modify the <c>state</c>, which will be used for the SQL <c>INSERT</c>
        ///     and propagated to the persistent object.
        /// </remarks>
        /// <returns>
        ///     A boolean indicating whether the user modified the <c>state</c> in any way.
        /// </returns>
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return SetAuditInfo(entity, state, propertyNames, true);
        }

        private struct Property : IEquatable<Property>
        {
            private readonly Type m_EntityType;
            private readonly string m_PropertyName;

            public static bool operator ==(Property left, Property right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Property left, Property right)
            {
                return !left.Equals(right);
            }

            public Property(Type entityType, string propertyName)
            {
                m_EntityType = entityType;
                m_PropertyName = propertyName;
            }

            /// <summary>
            ///     Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            ///     A boolean indicating whether the current object is equal to the <paramref name="other" /> parameter.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(Property other)
            {
                return ReferenceEquals(m_EntityType, other.m_EntityType)
                       && string.Equals(m_PropertyName, other.m_PropertyName);
            }

            /// <summary>
            ///     Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <returns>
            ///     A boolean indicating whether <paramref name="obj" /> and this instance are the same type and represent the same
            ///     value.
            /// </returns>
            /// <param name="obj">Another object to compare to. </param>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                return obj is Property && Equals((Property)obj);
            }

            /// <summary>
            ///     Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            ///     A 32-bit signed integer that is the hash code for this instance.
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return (m_EntityType.GetHashCode() * 397) ^ m_PropertyName.GetHashCode();
                }
            }
        }
    }
}