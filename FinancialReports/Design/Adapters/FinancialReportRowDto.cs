/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : FinancialReportRowDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a row definition of financial report.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO used to return a row definition of financial report.</summary>
  public class FinancialReportRowDto {

    public string UID {
      get; internal set;
    }

    public string Code {
      get; internal set;
    }


    public string Label {
      get; internal set;
    }


    public string Format {
      get; internal set;
    }


    public int Position {
      get; internal set;
    }


    public string FinancialConceptUID {
      get; internal set;
    }

  } // class FinancialReportRowDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
