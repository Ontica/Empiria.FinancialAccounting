/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Domain Layer                         *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Information Holder                   *
*  Type     : ReportType                                    License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Describes a report.                                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Describes a report.</summary>
  internal class ReportType: BaseReportType {

    #region Constructors and parsers

    protected ReportType() {
      // no-op
    }

    static internal new FixedList<ReportType> GetList() {
      return BaseObject.GetList<ReportType>()
                       .ToFixedList();
    }

    #endregion Constructors and parsers

    #region Properties

    public FixedList<AccountsChart> AccountsCharts {
      get {
        return base.ExtendedDataField.GetFixedList<AccountsChart>("accountsCharts");
      }
      private set {
        base.ExtendedDataField.Set("accountsCharts", value);
      }
    }


    public FixedList<FileType> ExportTo {
      get {
        return base.ExtendedDataField.GetFixedList<FileType>("exportTo");
      }
      private set {
        base.ExtendedDataField.Set("exportTo", value);
      }
    }


    public string Group {
      get {
        return base.ExtendedDataField.Get<string>("group");
      }
      private set {
        base.ExtendedDataField.Set("group", value);
      }
    }


    public string PayloadType {
      get {
        return base.ExtendedDataField.Get<string>("payloadType");
      }
      private set {
        base.ExtendedDataField.Set("payloadType", value);
      }
    }


    public ReportTypeActions Show {
      get {
        return base.ExtendedDataField.Get<ReportTypeActions>("show", new ReportTypeActions());
      }
      private set {
        base.ExtendedDataField.Set("show", value);
      }
    }


    public FixedList<OutputTypeObject> OutputType {
      get {
        return base.ExtendedDataField.GetFixedList<OutputTypeObject>("outputType", false);
      }
      private set {
        base.ExtendedDataField.Set("outputType", value);
      }
    }


    #endregion Properties

  }  // class ReportType


  public class OutputTypeObject {

    public string Uid {
      get; set;
    }


    public string Name {
      get; set;
    }

  }


  public class ReportTypeActions {


    public bool SingleDate {
      get; set;
    } = false;


    public bool DatePeriod {
      get; set;
    } = false;


    public bool Ledgers {
      get; set;
    } = false;


    public bool Account {
      get; set;
    } = false;


    public bool WithSubledgerAccount {
      get; set;
    } = false;


    public bool ElaboratedBy {
      get; set;
    } = false;


    public bool VerificationNumbers {
      get; set;
    } = false;


    public bool SendType {
      get; set;
    } = false;


    public bool OutputType {
      get; set;
    } = false;


  } // class ReportTypeActions


}  // namespace Empiria.FinancialAccounting.Reporting
