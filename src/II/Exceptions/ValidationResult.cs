// Copyright 2016 by PeopleWare n.v..
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable]
    public class ValidationResult : SemanticException
    {
        private const string Membernameskey = "MemberNamesKey";

        protected ValidationResult(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ValidationResult(System.ComponentModel.DataAnnotations.ValidationResult validationResult)
            : this(validationResult != null ? validationResult.ErrorMessage : null, validationResult != null ? validationResult.MemberNames : Enumerable.Empty<string>())
        {
        }

        public ValidationResult(string errorMessage, IEnumerable<string> memberNames)
            : base(errorMessage)
        {
            MemberNames = memberNames;
        }

        public IEnumerable<string> MemberNames
        {
            get { return (IEnumerable<string>)Data[Membernameskey]; }
            private set { Data[Membernameskey] = value; }
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            var otherVar = other as ValidationResult;
            return result
                   && otherVar != null
                   && MemberNames.SequenceEqual(otherVar.MemberNames);
        }

        public override string ToString()
        {
            return string.Format("{0}, MemberNames: {1}", base.ToString(), string.Join(", ", MemberNames));
        }
    }
}