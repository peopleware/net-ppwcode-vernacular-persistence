/*
 * Copyright 2004 - $Date: 2008-11-15 23:58:07 +0100 (za, 15 nov 2008) $ by PeopleWare n.v..
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Using

using System.ComponentModel;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PPWCode.Vernacular.Persistence.I;

#endregion

namespace PPWCode.Vernacular.Persistence.Test_I
{
    /// <summary>
    /// This is a test class for AbstractPersistentObjectTest and is intended
    /// to contain all AbstractPersistentObjectTest Unit Tests
    /// </summary>
    [TestClass]
    public class AbstractVersionedPersistentObjectTest
    {
        private sealed class VersionedPersistentObject :
            AbstractVersionedPersistentObject
        {
            private int m_TestProperty;

            public int TestProperty
            {
                get
                {
                    return m_TestProperty;
                }
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

        [TestMethod, Microsoft.VisualStudio.TestTools.UnitTesting.Description("AbstractVersionedPersistentObject Constructor")]
        public void TestAbstractVersionedPersistentObjectConstructor()
        {
            new VersionedPersistentObject();
        }

        #region NotifyPropertyChanged

        private int m_NotifyPropertyChangedClass;

        [TestMethod, Microsoft.VisualStudio.TestTools.UnitTesting.Description("AbstractVersionedPersistentObject NotifyPropertyChanged")]
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

        #endregion
    }
}
