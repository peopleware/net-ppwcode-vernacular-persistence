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

using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace PPWCode.Vernacular.Persistence.II.Tests.IsSame
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Tests")]
    public class Persistent_IsSame_Tests : BaseFixture
    {
        [Test]
        public void transient_is_not_equal_to_null()
        {
            var x = new PersistentFoo();
            Assert.That(x.IsSame(null), Is.False);
        }

        [Test]
        public void persistent_is_not_equal_to_null()
        {
            var x = new PersistentFoo(1);
            Assert.That(x.IsSame(null), Is.False);
        }

        [Test]
        public void transient_is_equal_to_himself()
        {
            var x = new PersistentFoo();
            Assert.That(x.IsSame(x), Is.True);
        }

        [Test]
        public void persistent_is_equal_to_himself()
        {
            var x = new PersistentFoo(1);
            Assert.That(x.IsSame(x), Is.True);
        }

        [Test]
        public void transient_is_not_equal_to_transient()
        {
            var x = new PersistentFoo();
            var y = new PersistentFoo();
            Assert.That(x.IsSame(y), Is.False);
        }

        [Test]
        public void persistent_is_equal_to_persistent_with_same_identity()
        {
            var x = new PersistentFoo(1);
            var y = new PersistentFoo(1);
            Assert.That(x.IsSame(y), Is.True);
        }

        [Test]
        public void persistent_is_equal_to_persistent_with_same_identity_subclass()
        {
            var x = new PersistentFoo(1);
            var y = new PersistentSpecializedFoo(1);
            Assert.That(x.IsSame(y), Is.True);
        }
        [Test]
        public void persistent_is_not_equal_to_persistent_with_differrent_identity()
        {
            var x = new PersistentFoo(1);
            var y = new PersistentFoo(2);
            Assert.That(x.IsSame(y), Is.False);
        }
    }
}