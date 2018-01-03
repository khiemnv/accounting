#define use_custom_font

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace test_binding
{
    [DataContract(Name = "config")]
    public class lConfigMng
    {
        static string m_cfgPath = @"..\..\config.xml";
        //string m_sqliteDbPath = @"..\..\appData.db";
        //string m_cnnStr = @"Data Source=DESKTOP-GOEF1DS\SQLEXPRESS;Initial Catalog=accounting;Integrated Security=true";

        [DataMember(Name = "printToPdf")]
        public bool m_printToPdf = true;
        [DataMember(Name = "font")]
        public string m_zFont = "";       //Arial,10

        [DataMember(Name = "dbSchema")]
        public lDbSchema m_dbSchema;
        [DataMember(Name = "panels")]
        public List<lBasePanel> m_panels;

        XmlObjectSerializer m_Serializer;

        static XmlObjectSerializer createSerializer()
        {
            Type[] knownTypes = new Type[] {
#if use_sqlite
                    typeof(lSQLiteDbSchema),
#else
                    typeof(lSqlDbSchema),
#endif
                    typeof(lReceiptsTblInfo),
                    typeof(lInternalPaymentTblInfo),
                    typeof(lExternalPaymentTblInfo),
                    typeof(lSalaryTblInfo),
                    typeof(lGroupNameTblInfo),
                    typeof(lBuildingTblInfo),
                    typeof(lReceiptsContentTblInfo),
                    typeof(lReceiptsViewInfo),
                    typeof(lInterPaymentViewInfo),
                    typeof(lExterPaymentViewInfo),
                    typeof(lSalaryViewInfo),

                    typeof(lReceiptsDataPanel),
                    typeof(lInterPaymentDataPanel),
                    typeof(lExternalPaymentDataPanel),
                    typeof(lSalaryDataPanel),

                    typeof(lReceiptsReport),
                    typeof(lCurReceiptsReport),
                    typeof(lInternalPaymentReport),
                    typeof(lCurInterPaymentReport),
                    typeof(lExternalPaymentReport),
                    typeof(lCurExterPaymentReport),
                    typeof(lSalaryReport),
                    typeof(lCurSalaryReport),

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
                    typeof(lSalaryPanel)
                };
#if false
                DataContractSerializerSettings settings = new DataContractSerializerSettings();
                settings.IgnoreExtensionDataObject = true;
                settings.KnownTypes = knownTypes;
                m_serializer = new DataContractSerializer(
                    typeof(List<lBasePanel>), settings);
#else
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
            settings.IgnoreExtensionDataObject = true;
            settings.EmitTypeInformation = EmitTypeInformation.AsNeeded;
            settings.KnownTypes = knownTypes;
            return new DataContractJsonSerializer(
                typeof(lConfigMng), settings);
#endif
        }
        static lConfigMng m_instance;
        public static lConfigMng crtInstance()
        {
            string cfgPath = m_cfgPath;
            if (m_instance == null)
            {
                XmlObjectSerializer sz = createSerializer();
                if (File.Exists(cfgPath))
                {
                    XmlReader xrd = XmlReader.Create(cfgPath);
                    xrd.Read();
                    xrd.ReadToFollowing("config");
                    var obj = sz.ReadObject(xrd, false);
                    xrd.Close();
                    m_instance = (lConfigMng)obj;
                }
                else
                {
                    m_instance = new lConfigMng();
                }
                m_instance.loadFont();
                m_instance.m_Serializer = sz;
            }
            return m_instance;
        }

        private Font m_font;
        public static Font getFont()
        {
#if use_custom_font
            return (m_instance != null) ? m_instance.m_font : new Font("Arial", 10);
#else
            return SystemFonts.DefaultFont;
#endif
        }

        private void loadFont()
        {
            do
            {
                if (m_zFont == null) break;

                var arr = m_zFont.Split(',');
                if (arr.Length < 2) break;

                float fontSize;
                if (!float.TryParse(arr[1], out fontSize)) break;

                var tFont = new Font(arr[0], fontSize);
                if (tFont.Name != arr[0]) break;

                m_font = tFont;
                return;
            } while (false);
            m_font = new Font("Arial", 10);
        }
        public static void setFont(Font newFont)
        {
            if (m_instance != null)
            {
                m_instance.m_zFont = string.Format("{0},{1}", newFont.Name, newFont.Size);
                m_instance.m_font = newFont;
                m_instance.UpdateConfig();
            }
        }
        public static string getCurrencyFormat() { return "#,0"; }
        public static string getDateFormat() { return "yyyy-MM-dd"; }
        lConfigMng()
        {
        }

        public void UpdateConfig()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.Encoding = Encoding.Unicode;

            XmlWriter xwriter;
            xwriter = XmlWriter.Create(m_cfgPath, settings);
            xwriter.WriteStartElement("config");
            m_Serializer.WriteObjectContent(xwriter, this);
            xwriter.WriteEndElement();
            xwriter.Close();
        }
        public lTableInfo getTable(string tblName)
        {
            List<lTableInfo> tbls = new List<lTableInfo>();
            tbls.AddRange(m_dbSchema.m_tables);
            tbls.AddRange(m_dbSchema.m_views);
            foreach (lTableInfo tbl in tbls)
            {
                if (tbl.m_tblName == tblName)
                    return tbl;
            }
            return null;
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

        public static Button crtButton()
        {
            var btn = new Button();
            btn.Font = getFont();
            return btn;
        }
        public static Label crtLabel()
        {
            var ctrl = new Label();
            ctrl.Font = getFont();
            return ctrl;
        }
        public static TextBox crtTextBox()
        {
            var ctrl = new TextBox();
            ctrl.Font = getFont();
            return ctrl;
        }
        public static CheckBox crtCheckBox()
        {
            var ctrl = new CheckBox();
            ctrl.Font = getFont();
            return ctrl;
        }
        public static ComboBox crtComboBox()
        {
            var ctrl = new ComboBox();
            ctrl.Font = getFont();
            return ctrl;
        }
        private static Size getSize(string txt)
        {
            var size = TextRenderer.MeasureText(txt, getFont());
            return size;
        }
        public static int getWidth(string txt)
        {
            var w = getSize(txt).Width;
            return w;
        }
    }
}
