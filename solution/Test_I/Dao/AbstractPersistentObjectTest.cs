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

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PPWCode.Vernacular.Persistence.I;

namespace PPWCode.Vernacular.Persistence.Test_I
{
    [TestClass()]
    public class AbstractPersistentObjectTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        internal sealed class PersistentObject :
            AbstractPersistentObject
        {
        }

        [TestMethod, Description("AbstractPersistentObject Constructor")]
        public void TestAbstractPersistentObjectConstructor()
        {
            PersistentObject po = new PersistentObject();
        }

        [TestMethod, Description("AbstractPersistentObject HasSamePersistenceId")]
        public void TestAbstractPersistentObjectHasSamePersistenceId()
        {
            PersistentObject po1 = new PersistentObject { PersistenceId = null };
            PersistentObject po2 = new PersistentObject { PersistenceId = null };
            po1.HasSamePersistenceId(po2);
            po2.HasSamePersistenceId(po1);

            po1.PersistenceId = null;
            po2.PersistenceId = 1;
            po1.HasSamePersistenceId(po2);
            po2.HasSamePersistenceId(po1);

            po1.PersistenceId = 1;
            po2.PersistenceId = null;
            po1.HasSamePersistenceId(po2);
            po2.HasSamePersistenceId(po1);

            po1.PersistenceId = 1;
            po2.PersistenceId = 2;
            po1.HasSamePersistenceId(po2);
            po2.HasSamePersistenceId(po1);

            po1.PersistenceId = 2;
            po2.PersistenceId = 1;
            po1.HasSamePersistenceId(po2);
            po2.HasSamePersistenceId(po1);

            po1.PersistenceId = 1;
            po2.PersistenceId = 1;
            po1.HasSamePersistenceId(po2);
            po2.HasSamePersistenceId(po1);
        }
    }
}
