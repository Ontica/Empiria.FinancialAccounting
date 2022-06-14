/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : ReportDesignConfigDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that returns a financial report configuration.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO that returns financial report configuration.</summary>
  public class ReportDesignConfigDto {

    public FinancialReportDesignType DesignType {
      get; internal set;
    }

    public NamedEntityDto ReportType {
      get; internal set;
    }


    public NamedEntityDto AccountsChart {
      get; internal set;
    }


    public FixedList<NamedEntityDto> FinancialConceptGroups {
      get; internal set;
    }


    public FixedList<NamedEntityDto> DataFields {
      get; internal set;
    }

    public ReportGridDto Grid {
      get; internal set;
    }

  }  // class ReportDesignConfigDto



  /// <summary>Output DTO with a report grid configuration.</summary>
  public class ReportGridDto {

    public FixedList<string> Columns {
      get;
      internal set;
    }

    public int StartRow {
      get;
      internal set;
    }

    public int EndRow {
      get;
      internal set;
    }

  }  // class ReportGridDto

}  // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
