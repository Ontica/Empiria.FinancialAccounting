/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information Holder                      *
*  Type     : FinancialReportType                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial report.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Data;
using Empiria.Json;

namespace Empiria.FinancialAccounting.FinancialReports {

  public enum FinancialReportDesignType {

    ConceptsIntegration,

    FixedCells,

    FixedRows

  }


  public enum FinancialReportDataSource {

    AnaliticoCuentas,

    BalanzaEnColumnasPorMoneda

  }


  public class ExportTo {

    protected internal ExportTo() {
      // no-op
    }


    static internal ExportTo Parse(JsonObject json) {
      return new ExportTo {
        Name = json.Get<string>("name"),
        FileType = json.Get<string>("fileType"),
        FileName = json.Get<string>("fileName", string.Empty),
        TemplateId = json.Get<int>("templateId", -1)
      };
    }


    [DataField("Name")]
    public string Name {
      get; private set;
    }


    [DataField("FileType")]
    public string FileType {
      get; private set;
    }


    [DataField("FileName")]
    public string FileName {
      get; private set;
    }


    [DataField("TemplateId")]
    public int TemplateId {
      get; private set;
    }

  }  // class ExportTo


  /// <summary>Describes a financial report.</summary>
  public class FinancialReportType : GeneralObject {

    #region Constructors and parsers

    protected FinancialReportType() {
      // Required by Empiria Framework.
    }


    static public FinancialReportType Parse(int id) {
      return BaseObject.ParseId<FinancialReportType>(id);
    }


    static public FinancialReportType Parse(string uid) {
      return BaseObject.ParseKey<FinancialReportType>(uid);
    }


    static public FixedList<FinancialReportType> GetList() {
      return BaseObject.GetList<FinancialReportType>(string.Empty, "ObjectName")
                       .ToFixedList();
    }


    static public FixedList<FinancialReportType> GetList(AccountsChart accountsChart) {
      var fullList = GetList();

      return fullList.FindAll(x => x.AccountsChart.Equals(accountsChart));
    }


    static internal FixedList<FinancialReportType> GetListForDesign(AccountsChart accountsChart) {
      var fullList = GetList();

      return fullList.FindAll(x => x.AccountsChart.Equals(accountsChart) && x.IsDesignable);
    }


    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AccountsChart {
      get {
        return base.ExtendedDataField.Get<AccountsChart>("accountsChartId");
      }
    }


    public FinancialReportDataSource DataSource {
      get {
        return base.ExtendedDataField.Get<FinancialReportDataSource>("dataSource");
      }
    }


    public FinancialReportDesignType DesignType {
      get {
        return base.ExtendedDataField.Get<FinancialReportDesignType>("designType");
      }
    }


    public bool IsDesignable {
      get {
        return this.DesignType == FinancialReportDesignType.FixedRows;
      }
    }


    public bool RoundDecimals {
      get {
        return base.ExtendedDataField.Get("roundDecimals", false);
      }
    }


    public FixedList<ExportTo> ExportTo {
      get {
        return base.ExtendedDataField.GetFixedList<ExportTo>("exportTo", false);
      }
    }

    #endregion Properties

    #region Methods

    public FixedList<FinancialReportRow> GetRows() {
      return FinancialReportsRowData.GetRows(this);
    }


    internal FinancialReportRow GetRow(string rowUID) {
      return GetRows().Find(x => x.UID == rowUID);
    }

    #endregion Methods

  }  // class FinancialReportType

}  // namespace Empiria.FinancialAccounting.FinancialReports
