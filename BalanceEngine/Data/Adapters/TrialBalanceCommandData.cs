/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : TrialBalanceCommandData                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build trial balances.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {
  internal class TrialBalanceCommandData {

    /// <summary>Command payload used to build trial balances.</summary>
    public DateTime StartDate {
      get; set;
    } = DateTime.Now;


    public DateTime EndDate {
      get; set;
    } = DateTime.Now.AddDays(1);


    public int BalanceGroupId {
      get; set;
    } = 1;


    public string Fields {
      get; set;
    } = string.Empty;


    public string Grouping {
      get; set;
    } = string.Empty;


    public string Having {
      get; set;
    } = string.Empty;


    public string Ordering {
      get; set;
    } = string.Empty;


  } // class TrialBalanceCommandData

} // namespace Empiria.FinancialAccounting.BalanceEngine 
