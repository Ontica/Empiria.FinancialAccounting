/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Output Data Transfer Object             *
*  Type     : FinancialReportTypeDto                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output data transfer object for financial report types.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output data transfer object for financial report types.</summary>
  public class FinancialReportTypeDto {

    internal FinancialReportTypeDto() {
      // no-op
    }


    public string UID {
      get;
      internal set;
    }


    public string Name {
      get;
      internal set;
    }


    public string[] ExportTo {
      get;
      internal set;
    } = new string[0];


  }  // class FinancialReportTypeDto

}  // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
