/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : BalanceCommand                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build balances.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Command payload used to build balances.</summary>
  public class BalanceCommand {

    public TrialBalanceType TrialBalanceType {
      get; set;
    }


    public string AccountsChartUID {
      get; set;
    }


    public string FromAccount {
      get; set;
    } = string.Empty;


    public string SubledgerAccount {
      get; set;
    } = string.Empty;


    public bool WithSubledgerAccount {
      get; set;
    } = false;


    public string[] Ledgers {
      get; set;
    } = new string[0];


    public string[] Currencies {
      get; set;
    } = new string[0];


    public BalancesType BalancesType {
      get {
        return WithAllAccounts ? BalancesType.AllAccounts : BalancesType.WithCurrentBalance;
      }
    }


    public TrialBalanceCommandPeriod InitialPeriod {
      get; set;
    } = new TrialBalanceCommandPeriod();


    public bool WithAllAccounts {
      get; set;
    } = false;


    public bool UseCache {
      get; set;
    } = true;


  } // class BalanceCommand


} // Empiria.FinancialAccounting.BalanceEngine.Adapters
