using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.DataAnalysis;
using Infosys.ATR.UIAutomation.Entities;
using System.Reflection;
using System.IO;

namespace DataExtractionTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnApp_Click(object sender, EventArgs e)
        {
            List<string> lstUseCase = new List<string>();
            //lstUseCase.Add("20150729105016.atrwb");
            //lstUseCase.Add("20150729105508.atrwb");
            //lstUseCase.Add("20150729105745.atrwb");

            lstUseCase.Add("20150729105016.atrwb");
            lstUseCase.Add("20150820091002.atrwb");

            // DataExtraction for type All 
            GeneratedData(DataExtractionType.All, lstUseCase, "D:\\DataExtraction_All.csv");
            // DataExtraction for type ControlPath
            GeneratedData(DataExtractionType.ControlPath, lstUseCase, "D:\\DataExtraction_ControlPath.csv");
            // DataExtraction for type ApplicationPath
            GeneratedData(DataExtractionType.ApplicationPath, lstUseCase, "D:\\DataExtraction_ApplicationPath.csv");
            // DataExtraction for type ApplicationsUsage
            GeneratedData(DataExtractionType.ApplicationsUsage, lstUseCase, "D:\\DataExtraction_ApplicationsUsage.csv");
            // DataExtraction for type ScreenPath
            GeneratedData(DataExtractionType.ScreenPath, lstUseCase, "D:\\DataExtraction_ScreenPath.csv");

           /* // DataExtraction for All type
            List<ExtractedData> data = DataExtraction.Extract(DataExtractionType.All, lstUseCase);
            StringBuilder csvL1 = new StringBuilder();
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(ExtractedData).GetProperties();
            var count = propertyInfos.Count();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name.Equals("OtherInfo"))
                {
                    propertyInfos = typeof(AllExtractData).GetProperties();
                    foreach (PropertyInfo propertyInfo1 in propertyInfos)
                    {
                        csvL1.Append(string.Format("{0},", propertyInfo1.Name));
                    }
                }
                else
                    csvL1.Append(string.Format("{0},", propertyInfo.Name));
            }
            csvL1.Append(Environment.NewLine);

            foreach (var extractedData in data)
            {
                foreach (var allExtractData in extractedData.OtherInfo)
                {
                    StringBuilder csvL2 = new StringBuilder();
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.Identifier) ? "" : extractedData.Identifier));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.IdentifierIncidentCount.ToString()) ? "" : extractedData.IdentifierIncidentCount.ToString()));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.UseCaseId) ? "" : extractedData.UseCaseId));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathId) ? "" : extractedData.ScreenPathId));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathSequence.ToString()) ? "" : extractedData.ScreenPathSequence.ToString()));
                    propertyInfos = typeof(AllExtractData).GetProperties();
                    foreach (PropertyInfo propertyInfo1 in propertyInfos)
                    {
                        csvL2.Append(string.Format("{0},", allExtractData.GetType().GetProperty(propertyInfo1.Name).GetValue(allExtractData, null)));
                    }
                    csvL2.Append(Environment.NewLine);
                    csvL1.Append(csvL2.ToString());
                }
            }
            File.WriteAllText("D:\\DataExtraction_All.csv", csvL1.ToString());


            // DataExtraction for ControlPath
            List<ExtractedData> data_ControlPath = DataExtraction.Extract(DataExtractionType.ControlPath, lstUseCase);
            foreach (var extractedData in data_ControlPath)
            {
                foreach (var allExtractData in extractedData.OtherInfo)
                {
                    StringBuilder csvL2 = new StringBuilder();
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.Identifier) ? "" : extractedData.Identifier));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.IdentifierIncidentCount.ToString()) ? "" : extractedData.IdentifierIncidentCount.ToString()));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.UseCaseId) ? "" : extractedData.UseCaseId));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathId) ? "" : extractedData.ScreenPathId));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathSequence.ToString()) ? "" : extractedData.ScreenPathSequence.ToString()));
                    propertyInfos = typeof(AllExtractData).GetProperties();
                    foreach (PropertyInfo propertyInfo1 in propertyInfos)
                    {
                        csvL2.Append(string.Format("{0},", allExtractData.GetType().GetProperty(propertyInfo1.Name).GetValue(allExtractData, null)));
                    }
                    csvL2.Append(Environment.NewLine);
                    csvL1.Append(csvL2.ToString());
                }
            }
            File.WriteAllText("D:\\DataExtraction_ControlPath.csv", csvL1.ToString());
            */

        }

        private void btnAppEv_Click(object sender, EventArgs e)
        {
            List<ExtractedData> data = DataExtraction.Extract(DataExtractionType.ControlPath, null);
        }

        private void btnAppRl_Click(object sender, EventArgs e)
        {
            List<ExtractedData> data = DataExtraction.Extract(DataExtractionType.ApplicationPath, null);
        }



        private void GeneratedData(DataExtractionType dataExtractionType, List<string> lstUseCase,string outputPath)
        {
            List<ExtractedData> data = DataExtraction.Extract(dataExtractionType, lstUseCase);
            StringBuilder csvL1 = new StringBuilder();
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(ExtractedData).GetProperties();
            var count = propertyInfos.Count();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name.Equals("OtherInfo"))
                {
                    if (data.Exists(x => x.OtherInfo!=null))
                    {
                        propertyInfos = typeof(AllExtractData).GetProperties();
                        foreach (PropertyInfo propertyInfo1 in propertyInfos)
                        {
                            csvL1.Append(string.Format("{0},", propertyInfo1.Name));
                        }
                    }
                }
                else
                    csvL1.Append(string.Format("{0},", propertyInfo.Name));
            }
            csvL1.Append(Environment.NewLine);

            foreach (var extractedData in data)
            {
                if (data.Exists(x => x.OtherInfo != null))
                {
                    foreach (var allExtractData in extractedData.OtherInfo)
                    {
                        StringBuilder csvL2 = new StringBuilder();
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.Identifier) ? "" : extractedData.Identifier));
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.IdentifierIncidentCount.ToString()) ? "" : extractedData.IdentifierIncidentCount.ToString()));
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.UseCaseId) ? "" : extractedData.UseCaseId));
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathId) ? "" : extractedData.ScreenPathId));
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathSequence.ToString()) ? "" : extractedData.ScreenPathSequence.ToString()));
                        propertyInfos = typeof(AllExtractData).GetProperties();
                        foreach (PropertyInfo propertyInfo1 in propertyInfos)
                        {
                            csvL2.Append(string.Format("{0},", allExtractData.GetType().GetProperty(propertyInfo1.Name).GetValue(allExtractData, null)));
                        }
                        csvL2.Append(Environment.NewLine);
                        csvL1.Append(csvL2.ToString());
                    }
                }
                else
                {
                    StringBuilder csvL2 = new StringBuilder();
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.Identifier) ? "" : extractedData.Identifier));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.IdentifierIncidentCount.ToString()) ? "" : extractedData.IdentifierIncidentCount.ToString()));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.UseCaseId) ? "" : extractedData.UseCaseId));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathId) ? "" : extractedData.ScreenPathId));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathSequence.ToString()) ? "" : extractedData.ScreenPathSequence.ToString()));
                    csvL2.Append(Environment.NewLine);
                    csvL1.Append(csvL2.ToString());
                }
            }
            File.WriteAllText(outputPath, csvL1.ToString());
        }
    }
}
