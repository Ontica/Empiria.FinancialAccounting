/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : TrialBalanceCommand                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build trial balances                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Command payload used to build trial balances.</summary>
  public class TrialBalanceCommand {

    public TrialBalanceType TrialBalanceType {
      get; set;
    } = TrialBalanceType.Traditional;


    public bool Consolidated {
      get; set;
    } = true;


    public string[] Ledgers {
      get; set;
    } = new string[0];


    public DateTime FromDate {
      get; set;
    } = DateTime.Today;


    public DateTime ToDate {
      get; set;
    } = DateTime.Today.AddDays(1);


    public string[] Sectors {
      get; set;
    } = new string[0];


    public string FromAccount {
      get; set;
    } = string.Empty;


    public string ToAccount {
      get; set;
    } = string.Empty;


    public int Level {
      get; set;
    } = 0;


    public BalancesType BalancesType {
      get; set;
    } = BalancesType.AllAccounts;



  } // class TrialBalanceCommand


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
