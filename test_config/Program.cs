#define col_class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace test_config
{
    public class Program
    {
        [DataContract]
        public class lColInfo_config
        {
            [DataContract(Name = "ColType")]
            public enum lColType
            {
                [EnumMember]
                text,
                [EnumMember]
                dateTime,
                [EnumMember]
                num,
                [EnumMember]
                currency
            };
            [DataMember(Name = "name", EmitDefaultValue = false)]
            public string m_field;
            [DataMember(Name = "alias", EmitDefaultValue = false)]
            public string m_alias;
            [DataMember(Name = "lookupTbl", EmitDefaultValue = false)]
            public string m_lookupTbl;
            //[DataMember(Name = "type", EmitDefaultValue = false)]
            //public int m_type;
            [DataMember(Name = "type", EmitDefaultValue = false)]
            public lColType m_type;
            //{
            //    get { return (lColType)m_type; }
            //    set { e_type = value; m_type = (int)e_type; }
            //}
        };

        [DataContract]
        public class lColInfo :lColInfo_config { }

        [DataContract]
        public class lTableInfo_config
        {
            [DataMember(Name = "cols", EmitDefaultValue = false)]
            public lColInfo[] m_cols;
            [DataMember(Name = "name", EmitDefaultValue = false)]
            public string m_tblName;
            [DataMember(Name = "alias", EmitDefaultValue = false)]
            public string m_tblAlias;
        }

        public class lTableInfo :lTableInfo_config { }

        [DataContract]
        public class lSearchCtrl_config
        {
            [DataMember(Name = "name", EmitDefaultValue = false)]
            public string m_fieldName;
            [DataMember(Name = "pos", EmitDefaultValue = false)]
            public myPoint m_pos;
            [DataMember(Name = "mySize", EmitDefaultValue = false)]
            public mySize m_size;
            [DataMember(Name = "searchMath", EmitDefaultValue = false)]
            public bool m_searchMatch;
        };

        [DataContract]
        public class lSearchCtrl :lSearchCtrl_config { }

        [CollectionDataContract(Name = "Custom{0}List", ItemName = "CustomItem")]
        public class CustomList<T> : List<T>
        {
            public CustomList()
                : base()
            {
            }

            public CustomList(T[] items)
                : base()
            {
                foreach (T item in items)
                {
                    Add(item);
                }
            }
        }

        [DataContract]
        public class myPoint
        {
            [DataMember(Name = "X", EmitDefaultValue = false)]
            public int X;
            [DataMember(Name = "Y", EmitDefaultValue = false)]
            public int Y;

            public myPoint(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }

        [DataContract]
        public class mySize
        {
            [DataMember(Name = "width", EmitDefaultValue = false)]
            public int width;
            [DataMember(Name = "height", EmitDefaultValue = false)]
            public int height;

            public mySize(int w, int h) {
                width = w;
                height = h;
                }
        }

        [DataContract]
        public class lSearchPanel_config
        {
            [DataMember(Name = "tableName", EmitDefaultValue = false)]
            public string m_tableName;
            [DataMember(Name = "searchCtrls", EmitDefaultValue = false)]
            public lSearchCtrl_config[] m_searchCtrls;
        }

        //[DataContract]
        class lSearchPanel : lSearchPanel_config { }

        //[DataContract]
        class lReceiptsSearchPanel : lSearchPanel
        {
            public lReceiptsSearchPanel()
            {
                m_searchCtrls = new lSearchCtrl_config[] {
                    new lSearchCtrl_config { m_fieldName = "date", m_pos = new myPoint(1, 1), m_size = new mySize(1, 1) },
                    new lSearchCtrl_config {m_fieldName = "receipt_number", m_pos = new myPoint (1,2), m_size = new mySize(1,1), m_searchMatch = true }
                };
            }
        }

        static void test_list()
        {
                List<lColInfo> cols = new List<lColInfo>();
                cols.Add(new lColInfo { m_field = "field", m_alias = "alias" });
                cols.Add(new lColInfo { m_field = "field2", m_alias = "alias2" });
                DataContractSerializer sz = new DataContractSerializer(typeof(List<lColInfo>));
                sz.WriteObject(Console.OpenStandardOutput(), cols);
        }

        [DataContract]
        class testStruct_w_List
        {
            [DataMember]
            public List<lColInfo> cols = new List<lColInfo>();
        }
        static void test_struct_w_list() {
            testStruct_w_List tcls = new testStruct_w_List();
            tcls.cols.Add(new lColInfo { m_field = "field", m_alias = "alias" });
            tcls.cols.Add(new lColInfo { m_field = "field2", m_alias = "alias2" });
            DataContractSerializer sz = new DataContractSerializer(typeof(testStruct_w_List));
            sz.WriteObject(Console.OpenStandardOutput(), tcls);
        }

        [DataContract(Name ="colInfo")]
        class colInfo:lColInfo_config
        {

        }
        static void test_w_knowtype() {
            colInfo col = new colInfo {m_alias = "alias", m_field = "field" };

            DataContractSerializer sz = new DataContractSerializer(typeof(lColInfo_config), new Type[] { typeof(colInfo) });
            sz.WriteObject(Console.OpenStandardOutput(), col);

            sz = new DataContractSerializer(typeof(colInfo));
            sz.WriteObject(Console.OpenStandardOutput(), col);
            //sz = new DataContractSerializer(typeof(lColInfo_config)); -> error
        }


        class MyDataContractResolver : DataContractResolver
        {
            private Dictionary<string, XmlDictionaryString> dictionary = new Dictionary<string, XmlDictionaryString>();
            Assembly assembly;

            // Definition of the DataContractResolver
            public MyDataContractResolver(Assembly assembly)
            {
                this.assembly = assembly;
            }

            // Used at deserialization
            // Allows users to map xsi:type name to any Type 
            public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
            {
                XmlDictionaryString tName;
                XmlDictionaryString tNamespace;
                if (dictionary.TryGetValue(typeName, out tName) && dictionary.TryGetValue(typeNamespace, out tNamespace))
                {
                    return this.assembly.GetType(tNamespace.Value + "." + tName.Value);
                }
                else
                {
                    return null;
                }
            }
            // Used at serialization
            // Maps any Type to a new xsi:type representation
            public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
            {
                string name = type.Name;
                string namesp = type.Namespace;
                typeName = new XmlDictionaryString(XmlDictionary.Empty, name, 0);
                typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, namesp, 0);
                if (!dictionary.ContainsKey(type.Name))
                {
                    dictionary.Add(name, typeName);
                }
                if (!dictionary.ContainsKey(type.Namespace))
                {
                    dictionary.Add(namesp, typeNamespace);
                }
                return true;
            }
        }
        class t_struct_w_resolver
        {
            public int x;
        }
        static void test_w_resolver()
        {
            t_struct_w_resolver tcls = new t_struct_w_resolver { x = 1 };
            DataContractSerializerSettings s = new DataContractSerializerSettings();
            s.DataContractResolver = new MyDataContractResolver(typeof(t_struct_w_resolver).Assembly);
            DataContractSerializer sz = new DataContractSerializer(typeof(t_struct_w_resolver), s);
            
            sz.WriteObject(Console.OpenStandardOutput(), tcls);
        }

        static void test_w_point_size()
        {
            Size s = new Size(1, 2);
            Point p = new Point(1, 2);
            DataContractSerializer sz = new DataContractSerializer(typeof(Size));
            sz.WriteObject(Console.OpenStandardOutput(), s);
            sz = new DataContractSerializer(typeof(Point));
            sz.WriteObject(Console.OpenStandardOutput(), p);
        }

        static void Main(string[] args)
        {

            test_w_knowtype();
            //test_w_resolver();

            lTableInfo_config tbl1 = new lTableInfo_config();
            tbl1.m_tblName = "receipts";
            tbl1.m_tblAlias = "Bang thu";
            tbl1.m_cols = new lColInfo[]
            {
                new lColInfo {m_alias = "alias", m_field = "field", m_lookupTbl = "lookupTbl", m_type = lColInfo.lColType.text },
                new lColInfo {m_alias = "col2", m_field = "field", m_lookupTbl = "lookupTbl", m_type = lColInfo.lColType.text },
            };
            lTableInfo_config[] tbls = new lTableInfo_config[] { tbl1 };

            DataContractJsonSerializer jsonz = new DataContractJsonSerializer(typeof(lTableInfo_config[]));
            jsonz.WriteObject(Console.OpenStandardOutput(), tbls);

            lReceiptsSearchPanel panel = new lReceiptsSearchPanel();
            lSearchPanel_config panel_config = (lSearchPanel_config)panel;
            lSearchPanel_config[] panels = new lSearchPanel_config[] { panel_config };
            //jsonz = new DataContractJsonSerializer(typeof(lSearchPanel_config[]));
            //jsonz.WriteObject(Console.OpenStandardOutput(), panels);
            XmlSerializer xs = new XmlSerializer(typeof(lColInfo));
            xs.Serialize(Console.OpenStandardOutput(), tbl1.m_cols[0]);
            //xs.Deserialize()

            lSearchCtrl_config[] ctrls = new lSearchCtrl_config[]
            {
                new lSearchCtrl_config { m_fieldName = "date", m_pos = new myPoint(1, 1), m_size = new mySize(1, 1) },
                    new lSearchCtrl_config {m_fieldName = "receipt_number", m_pos = new myPoint (1,2), m_size = new mySize(1,1), m_searchMatch = true }
            };
            DataContractJsonSerializerSettings jsonsettings = new DataContractJsonSerializerSettings();
            
            jsonz = new DataContractJsonSerializer(typeof(lSearchCtrl_config[]));
            jsonz.WriteObject(Console.OpenStandardOutput(), ctrls);

            {
                DataContractSerializerSettings settings = new DataContractSerializerSettings();
                XmlDictionary dic = new XmlDictionary();
                settings.RootName = new XmlDictionaryString(dic, "tableInfo", 1);
            
                DataContractSerializer tmpz = new DataContractSerializer(typeof(lTableInfo_config), settings);
                tmpz.WriteObject(Console.OpenStandardOutput(), tbl1);
            }

            DataContractSerializer xmlz = new DataContractSerializer(typeof(lTableInfo_config[]));
            DataContractSerializer xmlz2 = new DataContractSerializer(typeof(lSearchCtrl_config[]));

            string pathXml = "config.xml";
            XmlDocument doc = new XmlDocument();
            XmlWriter wrt;
            //XmlReader rd;
            if (!System.IO.File.Exists(pathXml)) {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";

                wrt = XmlWriter.Create("config.xml", settings);
                wrt.WriteStartElement("config");

                wrt.WriteStartElement("dataPanel");
                xmlz.WriteObjectContent(wrt, tbls);
                wrt.WriteEndElement();

                wrt.WriteStartElement("searchPanel");
                //xmlz2.WriteObjectContent(wrt, ctrls);
                //jsonz.WriteObject(wrt, ctrls);
                jsonz.WriteObjectContent(wrt, ctrls);
                wrt.WriteEndElement();

                wrt.WriteEndElement();
                wrt.Close();
            }

            //doc.Load(pathXml);
            //XmlNode node = doc.SelectSingleNode("searchPanel");
            XmlReader rd = XmlReader.Create(pathXml);
            rd.Read();

            rd.ReadToFollowing("dataPanel");
            var objs1 = xmlz.ReadObject(rd, false);

            rd.ReadToFollowing("searchPanel");
#if false
            rd.ReadToDescendant("root");
            rd.MoveToContent();
            rd.MoveToElement();
            rd.ReadEndElement();
            rd.Skip();
            Console.WriteLine(rd.Name);
            
            var objs = jsonz.ReadObject(rd);
#else
            var objs = jsonz.ReadObject(rd, false);
#endif
            //XmlNode node = doc.FirstChild;
            //doc.SelectSingleNode("dataPanel");
        }
    }
}
