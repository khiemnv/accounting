
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace test_binding
{
    public partial class Form1 : Form
    {
        class lConfigMng
        {
            public string m_cfgPath = "config.xml";
            DataContractSerializer m_serializer;
            public lConfigMng()
            {
                m_serializer = new DataContractSerializer(typeof(List<lBasePanel>), new Type[] {
                    typeof(lReceiptsTblInfo),
                    typeof(lInternalPaymentTblInfo),
                    typeof(lExternalPaymentTblInfo),
                    typeof(lSalaryTblInfo),

                    typeof(lReceiptsDataPanel),
                    typeof(lInterPaymentDataPanel),
                    typeof(lExternalPaymentDataPanel),
                    typeof(lSalaryDataPanel),

                    typeof(lReceiptsReport),
                    typeof(lInternalPaymentReport),
                    typeof(lExternalPaymentReport),
                    typeof(lSalaryReport),

                    typeof(lSearchCtrlText),
                    typeof(lSearchCtrlDate),
                    typeof(lSearchCtrlNum),
                    typeof(lSearchCtrlCurrency),

                    typeof(lReceiptsSearchPanel),
                    typeof(lInterPaymentSearchPanel),
                    typeof(lExternalPaymentSearchPanel),
                    typeof(lSalarySearchPanel),

                    typeof(lReceiptsPanel),
                    typeof(lInterPaymentPanel),
                    typeof(lExternalPaymentPanel),
                    typeof(lSalaryPanel),
                });
            }
            public List<lBasePanel> LoadConfig()
            {
                //panels = new List<lBasePanel>();
                if (File.Exists(m_cfgPath))
                {
                    XmlReader xrd = XmlReader.Create(m_cfgPath);
                    xrd.Read();

                    xrd.ReadToFollowing("panels");

                    var objs1 = m_serializer.ReadObject(xrd, false);
                    return (List<lBasePanel>)objs1;
                } else
                {
                    return null;
                }
            }
            public void UpdateConfig(List<lBasePanel> panels)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.Encoding = Encoding.Unicode;

                XmlWriter xwriter;
                xwriter = XmlWriter.Create(m_cfgPath, settings);
                xwriter.WriteStartElement("config");
                xwriter.WriteStartElement("panels");

                //write panels to config file
                m_serializer.WriteObjectContent(xwriter, panels);

                xwriter.WriteEndElement();
                xwriter.WriteEndElement();
                xwriter.Close();
            }

            public void test(lReceiptsPanel receiptsPanel)
            {
                DataContractSerializer sz;
                //sz = new DataContractSerializer(typeof(lTableInfo), new Type[] { typeof(lReceiptsTblInfo) });
                //sz.WriteObject(Console.OpenStandardOutput(), m_receiptsPanel.m_tblInfo);
                //sz = new DataContractSerializer(typeof(lDataPanel), new Type[] {
                //    typeof(lReceiptsTblInfo),
                //    typeof(lReceiptsDataPanel),
                //});
                //sz.WriteObject(Console.OpenStandardOutput(), m_receiptsPanel.m_dataPanel);
                //sz = new DataContractSerializer(typeof(lBaseReport), 
                //    new Type[] {
                //        typeof(lReceiptsReport),
                //    }
                //);
                //sz.WriteObject(Console.OpenStandardOutput(), m_receiptsPanel.m_report);
                //sz = new DataContractSerializer(typeof(lSearchPanel),
                //    new Type[] {
                //        typeof(lSearchCtrlText),
                //        typeof(lSearchCtrlDate),
                //        typeof(lSearchCtrlNum),
                //        typeof(lSearchCtrlCurrency),
                //        typeof(lReceiptsSearchPanel),
                //    }
                //);
                //sz.WriteObject(Console.OpenStandardOutput(), m_receiptsPanel.m_searchPanel);
                sz = new DataContractSerializer(typeof(lBasePanel), new Type[] {
                    typeof(lReceiptsTblInfo),
                    typeof(lReceiptsDataPanel),
                    typeof(lReceiptsReport),
                    typeof(lSearchCtrlText),
                    typeof(lSearchCtrlDate),
                    typeof(lSearchCtrlNum),
                    typeof(lSearchCtrlCurrency),
                    typeof(lReceiptsSearchPanel),
                    typeof(lReceiptsPanel)
                });
                sz.WriteObject(Console.OpenStandardOutput(), receiptsPanel);
            }
        }
    }
}
