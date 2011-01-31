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

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PPWCode.Vernacular.Exceptions.I;
using PPWCode.Vernacular.Semantics.I;

#endregion

namespace PPWCode.Vernacular.Semantics.Test_I
{
    [TestClass]
    public class AbstractRousseauObjectTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}

        private AbstractRousseauObject[] m_Subjects;

        [TestInitialize]
        public void MyTestInitialize()
        {
            m_Subjects = new AbstractRousseauObject[2];
            m_Subjects[0] = new RousseauObject
            {
                MockWild = false
            };
            m_Subjects[1] = new RousseauObject
            {
                MockWild = true
            };
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            m_Subjects = null;
        }

        #endregion

        [TestMethod]
        public void WildExceptionsTest()
        {
            foreach (AbstractRousseauObject aro in m_Subjects)
            {
                CompoundSemanticException cpe = aro.WildExceptions();
            }
        }

        /// <summary>
        ///A test for ThrowIfNotCivilized
        ///</summary>
        [TestMethod]
        public void ThrowIfNotCivilizedTest()
        {
            foreach (AbstractRousseauObject aro in m_Subjects)
            {
                try
                {
                    aro.ThrowIfNotCivilized();
                }
                catch (CompoundSemanticException)
                {
                    // NOP
                }
            }
        }

        [TestMethod]
        public void IsCivilizedTest()
        {
            foreach (AbstractRousseauObject aro in m_Subjects)
            {
                aro.IsCivilized();
            }
        }

        /// <summary>
        ///A test for AbstractRousseauObject Constructor
        ///</summary>
        [TestMethod]
        public void AbstractRousseauObjectConstructorTest()
        {
            AbstractRousseauObject target = new RousseauObject();
        }
    }
}