// Copyright 2019 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.IV;

namespace PPWCode.Vernacular.Persistence.IV
{
    [Serializable]
    public class EmptyResultException : SemanticException
    {
        /// <inheritdoc cref="SemanticException" />
        public EmptyResultException()
        {
        }

        /// <inheritdoc cref="SemanticException" />
        public EmptyResultException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="SemanticException" />
        public EmptyResultException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc cref="SemanticException" />
        [ExcludeFromCodeCoverage]
        protected EmptyResultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
