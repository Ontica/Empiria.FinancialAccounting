/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : VouchersByAccountCommandData               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build vouchers by account.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {
  
  internal class VouchersByAccountCommandData {


    public int AccountsChartId {
      get;
      internal set;
    }


    public DateTime FromDate {
      get; internal set;
    }


    public DateTime ToDate {
      get; internal set;
    }


    public string Fields {
      get; internal set;
    } = string.Empty;


    public string Filters {
      get; set;
    } = string.Empty;


    public string Grouping {
      get; internal set;
    } = string.Empty;


    public string Ordering {
      get; internal set;
    } = string.Empty;
    
  } // class VouchersByAccountCommandData

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data.Adapters
