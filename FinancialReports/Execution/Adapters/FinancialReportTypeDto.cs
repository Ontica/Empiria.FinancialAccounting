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

  public class ShowFields {

    public bool GetAccountsIntegration {
      get; set;
    } = true;

    public bool DatePeriod {
      get; set;
    }

    public bool SingleDate {
      get; set;
    } = true;

  }


  /// <summary>Output data transfer object for financial report types.</summary>
  public class FinancialReportTypeDto {

    internal FinancialReportTypeDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public bool HasAccountsIntegration {
      get; internal set;
    }


    public FixedList<ExportToDto> ExportTo {
      get;
      internal set;
    }


    public ShowFields Show {
      get;
      internal set;
    } = new ShowFields();

  }  // class FinancialReportTypeDto

}  // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
