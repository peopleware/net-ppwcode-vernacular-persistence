using PPWCode.Vernacular.Semantics.I;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPWCode.Vernacular.Exceptions.I;

namespace PPWCode.Vernacular.Semantics.Test_I
{
    [TestClass()]
    public class AbstractRousseauObjectTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        private AbstractRousseauObject[] m_Subjects;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            m_Subjects = new AbstractRousseauObject[2];
            m_Subjects[0] = new RousseauObject() { MockWild = false };
            m_Subjects[1] = new RousseauObject() { MockWild = true };
        }
        
        [TestCleanup()]
        public void MyTestCleanup()
        {
            m_Subjects = null;
        }

        #endregion


        [TestMethod()]
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
        [TestMethod()]
        public void ThrowIfNotCivilizedTest()
        {
            foreach (AbstractRousseauObject aro in m_Subjects)
            {
                try
                {
                    aro.ThrowIfNotCivilized();
                }
                catch(CompoundSemanticException)
                {
                    // NOP
                }
            }
        }

        [TestMethod()]
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
        [TestMethod()]
        public void AbstractRousseauObjectConstructorTest()
        {
            AbstractRousseauObject target = new RousseauObject();
        }
    }
}
