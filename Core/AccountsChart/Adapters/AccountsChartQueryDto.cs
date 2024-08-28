/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : AccountsChartQueryDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO used for accounts chart's financial accounts searching.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>DTO used for accounts chart's financial accounts searching.</summary>
  public class AccountsChartQueryDto {

    public AccountsChart AccountsChart {
      get; set;
    } = AccountsChart.IFRS;


    public FixedList<AccountRange> Accounts {
      get; set;
    } = new FixedList<AccountRange>();


    public DateTime FromDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime ToDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public bool IncludeSummaryAccounts {
      get; set;
    } = true;

  }  // class AccountsChartQueryDto

}  // namespace Empiria.FinancialAccounting.Adapters
