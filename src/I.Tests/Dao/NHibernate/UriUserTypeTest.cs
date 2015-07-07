#region Using

using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;

using PPWCode.Vernacular.Persistence.I.Dao.NHibernate;

#endregion

namespace PPWCode.Vernacular.Persistence.I.Tests.Dao.NHibernate
{
    [TestFixture]
    public class UriUserTypeTest
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        #endregion

        [Test]
        public void NullSafeSetTest()
        {
            UriUserType target = new UriUserType();
            IDbCommand cmd = new SqlCommand(@"SELECT * FROM TUUT WHERE a = @a");
            SqlParameter param = new SqlParameter(@"a", null);
            cmd.Parameters.Add(param);
            foreach (Uri uri in s_Subjects)
            {
                target.NullSafeSet(cmd, uri, 0);
                if (uri != null)
                {
                    Assert.AreEqual(param.Value.ToString(), uri.AbsoluteUri);
                }
                else
                {
                    Assert.IsTrue(param.Value is DBNull);
                }
                Console.WriteLine("To DB: " + param.Value);
            }
        }

        [Test]
        public void NullSafeSetTest2()
        {
            UriUserType target = new UriUserType();
            IDbCommand cmd = new SqlCommand(@"SELECT * FROM TUUT WHERE a = @a");
            SqlParameter param = new SqlParameter(@"a", null);
            cmd.Parameters.Add(param);
            foreach (Uri uri in s_Subjects)
            {
                if (uri != null)
                {
                    string original = uri.AbsoluteUri;
                    Uri subject = new Uri(uri.AbsoluteUri + UriUserType.UriLikeWildcard);
                    target.NullSafeSet(cmd, subject, 0);
                    string expected = original.Replace(@"%", @"[%]");
                    expected = expected.Replace(@"_", @"[_]");
                    expected = expected.Replace(@"?", @"[?]");
                    Assert.IsTrue(param.Value.ToString().StartsWith(expected));
                    Assert.IsTrue(param.Value.ToString().EndsWith(@"%"));
                    Console.WriteLine("Search predicate DB: " + param.Value);
                }
            }
        }

        [Test]
        public void NullSafeSetTest3()
        {
            UriUserType target = new UriUserType();
            Uri subject = new Uri("http://my.domain.com/dit is ee" + UriUserType.UriLikeWildcard + "n test % met _/een underscore/../bla./bla?");
            string original = subject.AbsoluteUri;
            IDbCommand cmd = new SqlCommand(@"SELECT * FROM TUUT WHERE a = @a");
            SqlParameter param = new SqlParameter(@"a", null);
            cmd.Parameters.Add(param);
            target.NullSafeSet(cmd, subject, 0);
            string expected = original.Replace(@"%", @"[%]");
            expected = expected.Replace(@"_", @"[_]");
            expected = expected.Replace(@"?", @"[?]");
            expected = expected.Replace(UriUserType.UriLikeWildcard, @"%");
            Assert.AreEqual(expected, param.Value.ToString());
            Console.WriteLine("Search predicate DB: " + param.Value);
        }

        private static readonly Uri[] s_Subjects =
        {
            new Uri("http://my.domain.com/dit is een test % met _/een underscore/../bla./bla?"),
            new Uri("http://my.domain.com/"),
            new Uri("http://my.domain.com"),
            new Uri("http://my.domain.com/dit is een test % met _/een underscore/../bla./bla/"),
            new Uri("http://my.domain.com/dit is een test % met _/een underscore/../bla./bla/foo#bar?x=2"),
            new Uri("http://my.domain.com/dit is een test % met _/een underscore/../bla./bla/foo#bar?x=2/"),
            null,
        };

        [Test]
        public void NullSafeGetTest()
        {
            UriUserType target = new UriUserType();
            DataReaderMock rs = new DataReaderMock();
            string[] names = { @"a" };
            object owner = new object();
            foreach (Uri uri in s_Subjects)
            {
                rs.VarChar = uri == null ? null : uri.AbsoluteUri;
                object actual = target.NullSafeGet(rs, names, owner);
                if (uri != null)
                {
                    Assert.IsNotNull(actual);
                }
                else
                {
                    Assert.IsNull(actual);
                }
                Assert.AreEqual(uri, actual);
            }
        }

        [Test]
        public void RoundtripTest()
        {
            // writer
            UriUserType target = new UriUserType();
            IDbCommand cmd = new SqlCommand(@"SELECT * FROM TUUT WHERE a = @a");
            SqlParameter param = new SqlParameter(@"a", null);
            cmd.Parameters.Add(param);
            // reader
            DataReaderMock rs = new DataReaderMock();
            string[] names = { @"a" };
            object owner = new object();
            // test
            foreach (Uri subject in s_Subjects)
            {
                target.NullSafeSet(cmd, subject, 0);
                string dbField = param.Value is DBNull ? null : param.Value.ToString();
                rs.VarChar = dbField;
                object reading = target.NullSafeGet(rs, names, owner);
                Assert.AreEqual(subject, reading);
                target.NullSafeSet(cmd, subject, 0);
                string dbField2 = param.Value is DBNull ? null : param.Value.ToString();
                Assert.AreEqual(dbField, dbField2);
            }
        }

        public class DataReaderMock : IDataReader
        {
            public string VarChar { get; set; }

            #region Implementation of IDisposable

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region Implementation of IDataRecord

            public string GetName(int i)
            {
                throw new NotImplementedException();
            }

            public string GetDataTypeName(int i)
            {
                throw new NotImplementedException();
            }

            public Type GetFieldType(int i)
            {
                throw new NotImplementedException();
            }

            public object GetValue(int i)
            {
                throw new NotImplementedException();
            }

            public int GetValues(object[] values)
            {
                throw new NotImplementedException();
            }

            public int GetOrdinal(string name)
            {
                return 0;
            }

            public bool GetBoolean(int i)
            {
                throw new NotImplementedException();
            }

            public byte GetByte(int i)
            {
                throw new NotImplementedException();
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public char GetChar(int i)
            {
                throw new NotImplementedException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public Guid GetGuid(int i)
            {
                throw new NotImplementedException();
            }

            public short GetInt16(int i)
            {
                throw new NotImplementedException();
            }

            public int GetInt32(int i)
            {
                throw new NotImplementedException();
            }

            public long GetInt64(int i)
            {
                throw new NotImplementedException();
            }

            public float GetFloat(int i)
            {
                throw new NotImplementedException();
            }

            public double GetDouble(int i)
            {
                throw new NotImplementedException();
            }

            public string GetString(int i)
            {
                throw new NotImplementedException();
            }

            public decimal GetDecimal(int i)
            {
                throw new NotImplementedException();
            }

            public DateTime GetDateTime(int i)
            {
                throw new NotImplementedException();
            }

            public IDataReader GetData(int i)
            {
                throw new NotImplementedException();
            }

            public bool IsDBNull(int i)
            {
                return VarChar == null;
            }

            public int FieldCount
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            object IDataRecord.this[int i]
            {
                get
                {
                    return VarChar;
                }
            }

            object IDataRecord.this[string name]
            {
                get
                {
                    return VarChar;
                }
            }

            #endregion

            #region Implementation of IDataReader

            public void Close()
            {
                throw new NotImplementedException();
            }

            public DataTable GetSchemaTable()
            {
                throw new NotImplementedException();
            }

            public bool NextResult()
            {
                throw new NotImplementedException();
            }

            public bool Read()
            {
                throw new NotImplementedException();
            }

            public int Depth
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsClosed
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int RecordsAffected
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            #endregion
        }
    }
}