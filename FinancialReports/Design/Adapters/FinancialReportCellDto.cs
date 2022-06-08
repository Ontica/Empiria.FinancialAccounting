/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : FinancialReportCellDto                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a row definition of a by-cell type financial report.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO used to return a row definition of a by-cell type financial report.</summary>
  public class FinancialReportCellDto {

    public string UID {
      get; internal set;
    }


    public string Column {
      get; internal set;
    }


    public int Row {
      get; internal set;
    }


    public string Label {
      get; internal set;
    }


    public string FinancialConceptUID {
      get; internal set;
    }


    public string Format {
      get; internal set;
    }

  }  // class FinancialReportCellDto

}  // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
