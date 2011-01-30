using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

using PPWCode.Vernacular.Semantics.I;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Semantics.Test_I
{
    [TestClass()]
    public class AbstractSemanticObjectTest
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


        private static readonly string[] SOME_STRINGS = new string[] { "A string", "another string", "", null, "You name here" };

        public AbstractSemanticObject[] BuildBs()
        {
            AbstractSemanticObject[] result = new AbstractSemanticObject[5];
            SemanticObjectA a = new SemanticObjectA();
            SemanticObjectC lastC = null;
            for (int i = 0; i < 5; i++)
            {
                SemanticObjectB b = new SemanticObjectB()
                {
                    IntProperty = i,
                    StringProperty = SOME_STRINGS[i]
                };
                for (int j = 0; j < 10; j++)
                {
                    SemanticObjectC c = new SemanticObjectC();
                    c.B = b;
                    c.A = a;
                    lastC = c;
                }
                b.A = a;
                result[i] = b;
            }
            // this creates a loop, as it is downstream, but not a collection
            //a.C = lastC;
            return result;
        }

        public class CallMock
        {
            public object Obj;
            public PropertyChangedEventArgs PceArgs;
        }

        private CallMock m_DelegateMethodeCalled;

        public void DelegateMethod(object obj, PropertyChangedEventArgs pceArgs)
        {
            m_DelegateMethodeCalled = new CallMock()
            {
                Obj = obj,
                PceArgs = pceArgs
            };
        }

        [TestMethod()]
        public void IsSerializedTest()
        {
            AbstractSemanticObject[] subjects = BuildBs();
            foreach (AbstractSemanticObject so in subjects)
            {
                bool result = so.IsSerialized;
            }
        }

        [TestMethod()]
        public void ToStringTest()
        {
            AbstractSemanticObject[] subjects = BuildBs();
            foreach (AbstractSemanticObject so in subjects)
            {
                string result = so.ToString();
                Trace.WriteLine(result);
                Trace.Flush();
                SemanticObjectB b = (SemanticObjectB)so;
                result = b.A.ToString();
                Trace.WriteLine(result);
                Trace.Flush();
                foreach (AbstractSemanticObject c in b.Cs)
                {
                    result = c.ToString();
                    Trace.WriteLine(result);                    
                }
                Trace.Flush();
            }
        }

        [TestMethod()]
        public void OnPropertyChangedTest()
        {
            AbstractSemanticObject[] subjects = BuildBs();
            foreach (AbstractSemanticObject so in subjects)
            {
                SemanticObjectB b = so as SemanticObjectB;
                if (b != null)
                {
                    b.PropertyChanged += DelegateMethod;
                    m_DelegateMethodeCalled = null;
                    for (int i = -7; i < 7; i++)
                    {
                        b.IntProperty = i;
                    }
                    Assert.IsNotNull(m_DelegateMethodeCalled);
                    Assert.AreEqual(b, m_DelegateMethodeCalled.Obj);
                    Assert.IsNotNull(m_DelegateMethodeCalled.PceArgs);
                    Assert.AreEqual("IntProperty", m_DelegateMethodeCalled.PceArgs.PropertyName);
                    m_DelegateMethodeCalled = null;
                    foreach (string s in SOME_STRINGS)
                    {
                        b.StringProperty = s;
                    }
                    Assert.IsNotNull(m_DelegateMethodeCalled);
                    Assert.AreEqual(b, m_DelegateMethodeCalled.Obj);
                    Assert.IsNotNull(m_DelegateMethodeCalled.PceArgs);
                    Assert.AreEqual("StringProperty", m_DelegateMethodeCalled.PceArgs.PropertyName);
                }
            }
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            AbstractSemanticObject[] subjects = BuildBs();
            foreach (AbstractSemanticObject so in subjects)
            {
                int result = so.GetHashCode();
            }
        }

        [TestMethod()]
        public void CreateAbstractSemanticObject()
        {
            AbstractSemanticObject result = new SemanticObjectB();
            result = new SemanticObjectA();
            result = new SemanticObjectC();
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            List<AbstractSemanticObject> all = new List<AbstractSemanticObject>(BuildBs());
            AbstractSemanticObject aB = all[0];
            SemanticObjectB aBasB = (SemanticObjectB)aB;
            all.Add(aBasB.A);
            all.AddRange(aBasB.Cs.Cast<AbstractSemanticObject>());
            bool result;
            foreach (AbstractSemanticObject aso1 in all)
            {
                foreach (AbstractSemanticObject aso2 in all)
                {
                    result = aso1.Equals(aso2);
                }
                result = aso1.Equals(null);
            }
        }
    }
}
