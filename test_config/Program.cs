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

namespace test_config
{
    class Program
    {
        [DataContract]
        public class lColInfo_config
        {
            public enum lColType
            {
                text,
                dateTime,
                num,
                currency
            };
            [DataMember(Name = "name", EmitDefaultValue = false)]
            public string m_field;
            [DataMember(Name = "alias", EmitDefaultValue = false)]
            public string m_alias;
            [DataMember(Name = "lookupTbl", EmitDefaultValue = false)]
            public string m_lookupTbl;
            [DataMember(Name = "type", EmitDefaultValue = false)]
            public int m_type;
        };

        [DataContract]
        class lColInfo:lColInfo_config { }

        [DataContract]
        class lTableInfo_config
        {
            [DataMember(Name = "cols", EmitDefaultValue = false)]
            public lColInfo[] m_cols;
            [DataMember(Name = "name", EmitDefaultValue = false)]
            public string m_tblName;
            [DataMember(Name = "alias", EmitDefaultValue = false)]
            public string m_tblAlias;
        }

        class lTableInfo:lTableInfo_config { }

        [DataContract]
        class lSearchCtrl_config
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
        class lSearchCtrl:lSearchCtrl_config { }

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
        class myPoint
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
        class mySize
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
        class lSearchPanel_config
        {
            [DataMember(Name = "tableName", EmitDefaultValue = false)]
            public string m_tableName;
            [DataMember(Name = "searchCtrls", EmitDefaultValue = false)]
            public lSearchCtrl_config[] m_searchCtrls;
        }

        [DataContract]
        class lSearchPanel : lSearchPanel_config { }

        [DataContract]
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

        static void Main(string[] args)
        {
            lTableInfo_config tbl1 = new lTableInfo_config();
            tbl1.m_tblName = "receipts";
            tbl1.m_tblAlias = "Bang thu";
            tbl1.m_cols = new lColInfo[]
            {
                new lColInfo {m_alias = "alias", m_field = "field", m_lookupTbl = "lookupTbl", m_type = 1 },
                new lColInfo {m_alias = "col2", m_field = "field", m_lookupTbl = "lookupTbl", m_type = 1 },
            };
            lTableInfo_config[] tbls = new lTableInfo_config[] { tbl1 };

            DataContractJsonSerializer jsonz = new DataContractJsonSerializer(typeof(lTableInfo_config[]));
            jsonz.WriteObject(Console.OpenStandardOutput(), tbls);

            lReceiptsSearchPanel panel = new lReceiptsSearchPanel();
            lSearchPanel[] panels = new lReceiptsSearchPanel[] { panel };
            jsonz = new DataContractJsonSerializer(typeof(lReceiptsSearchPanel[]));
            jsonz.WriteObject(Console.OpenStandardOutput(), panels);

            lSearchCtrl_config[] ctrls = new lSearchCtrl_config[]
            {
                new lSearchCtrl_config { m_fieldName = "date", m_pos = new myPoint(1, 1), m_size = new mySize(1, 1) },
                    new lSearchCtrl_config {m_fieldName = "receipt_number", m_pos = new myPoint (1,2), m_size = new mySize(1,1), m_searchMatch = true }
            };
            DataContractJsonSerializerSettings jsonsettings = new DataContractJsonSerializerSettings();
            
            jsonz = new DataContractJsonSerializer(typeof(lSearchCtrl_config[]));
            jsonz.WriteObject(Console.OpenStandardOutput(), ctrls);


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
