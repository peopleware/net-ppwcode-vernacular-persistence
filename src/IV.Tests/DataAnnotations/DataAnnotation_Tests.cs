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

using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace PPWCode.Vernacular.Persistence.IV.Tests.DataAnnotations
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Tests")]
    public class DataAnnotation_Tests : BaseFixture
    {
        [Test]
        public void all_annotations_are_fullfilled()
        {
            AnnotatedFoo x =
                new AnnotatedFoo(1)
                {
                    Name = "Name",
                    Age = 0
                };

            Assert.That(x.IsCivilized, Is.True);
        }

        [Test]
        public void no_annotation_result_to_civilized()
        {
            NotAnnotatedFoo x = new NotAnnotatedFoo(1);
            Assert.That(x.IsCivilized, Is.True);
        }

        [Test]
        public void none_of_the_annotations_not_fullfilled()
        {
            AnnotatedFoo x = new AnnotatedFoo(1);

            Assert.That(x.IsCivilized, Is.False);
        }

        [Test]
        public void one_or_more_annotations_not_fullfilled()
        {
            AnnotatedFoo x =
                new AnnotatedFoo(1)
                {
                    Name = "1234567890"
                };

            Assert.That(x.IsCivilized, Is.False);
        }
    }
}
