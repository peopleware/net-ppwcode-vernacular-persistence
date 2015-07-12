// Copyright 2010-2015 by PeopleWare n.v..
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

using System.ComponentModel;

using NUnit.Framework;

namespace PPWCode.Vernacular.Persistence.I.Tests
{
    /// <summary>
    ///     This is a test class for AbstractPersistentObjectTest and is intended
    ///     to contain all AbstractPersistentObjectTest Unit Tests.
    /// </summary>
    [TestFixture]
    public class AbstractVersionedPersistentObjectTest
    {
        private sealed class VersionedPersistentObject :
            AbstractVersionedPersistentObject
        {
            private int m_TestProperty;

            public int TestProperty
            {
                get { return m_TestProperty; }
                set
                {
                    if (m_TestProperty != value)
                    {
                        m_TestProperty = value;
                        OnPropertyChanged("TestProperty");
                    }
                }
            }
        }

        [Test, NUnit.Framework.Description("AbstractVersionedPersistentObject Constructor")]
        public void TestAbstractVersionedPersistentObjectConstructor()
        {
            new VersionedPersistentObject();
        }

        private int m_NotifyPropertyChangedClass;

        [Test, NUnit.Framework.Description("AbstractVersionedPersistentObject NotifyPropertyChanged")]
        public void TestAbstractVersionedPersistentObjectNotifyPropertyChanged()
        {
            VersionedPersistentObject po = new VersionedPersistentObject();

            // Test if the methods works without an attached handler (no null exception)
            Assert.AreEqual(m_NotifyPropertyChangedClass, 0);
            po.TestProperty++;
            Assert.AreEqual(m_NotifyPropertyChangedClass, 0);

            // Test the attached event handler
            po.PropertyChanged += PropertyChanged;
            po.TestProperty++;
            Assert.AreEqual(m_NotifyPropertyChangedClass, 1);
            po.TestProperty++;
            po.TestProperty++;
            Assert.AreEqual(m_NotifyPropertyChangedClass, 3);

            // Test a detached event handler.  Method should not be triggered and null exceptions should occur
            po.PropertyChanged -= PropertyChanged;
            po.TestProperty++;
            Assert.AreEqual(m_NotifyPropertyChangedClass, 3);
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            m_NotifyPropertyChangedClass++;
        }
    }
}