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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Persistence.II.Tests.DataAnnotations
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Tests")]
    public class DataAnnotation_Tests : BaseFixture
    {
        [Test]
        public void no_annotation_result_to_civilized()
        {
            var x = new NotAnnotatedFoo(1);
            Assert.That(x.IsCivilized, Is.True);
        }

        [Test]
        public void required_annotation_not_fullfilled()
        {
            var x = new AnnotatedFoo(1);

            CompoundSemanticException result = new CompoundSemanticException();
            ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (Validator.TryValidateObject(this, new ValidationContext(this), validationResults))
            {
                foreach (System.ComponentModel.DataAnnotations.ValidationResult validationResult in validationResults)
                {
                    result.AddElement(new ValidationResult(validationResult));
                }
            }

            Assert.That(x.IsCivilized, Is.False);
        }
    }
}