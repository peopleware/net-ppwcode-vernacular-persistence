// Copyright 2018 by PeopleWare n.v..
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.IV;

namespace PPWCode.Vernacular.Persistence.III
{
    [Serializable]
    public class ValidationViolationException : SemanticException
    {
        private const string Membernameskey = "MemberNamesKey";

        protected ValidationViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ValidationViolationException(ValidationResult validationResult)
            : this(validationResult?.ErrorMessage, validationResult != null ? validationResult.MemberNames : Enumerable.Empty<string>())
        {
        }

        public ValidationViolationException(string errorMessage, IEnumerable<string> memberNames)
            : base(errorMessage)
        {
            MemberNames = memberNames;
        }

        public IEnumerable<string> MemberNames
        {
            get => (IEnumerable<string>)Data[Membernameskey];
            private set => Data[Membernameskey] = value;
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            return result
                   && other is ValidationViolationException otherVar
                   && MemberNames.SequenceEqual(otherVar.MemberNames);
        }

        public override string ToString()
            => $"{base.ToString()}, MemberNames: {string.Join(", ", MemberNames)}";
    }
}
