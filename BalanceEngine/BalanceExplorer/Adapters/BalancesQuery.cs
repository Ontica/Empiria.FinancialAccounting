/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Query payload                           *
*  Type     : BalancesQuery                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to build balances.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters {

  /// <summary>Query payload used to generate balances.</summary>
  public class BalancesQuery {

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


    public FileReportVersion ExportTo {
      get; set;
    } = FileReportVersion.V1;


    public BalancesPeriod InitialPeriod {
      get; set;
    } = new BalancesPeriod();


    public bool WithAllAccounts {
      get; set;
    } = false;


    public bool UseCache {
      get; set;
    } = true;


    public override bool Equals(object obj) {
      return obj is BalancesQuery query &&
             query.GetHashCode() == this.GetHashCode();
    }


    public override int GetHashCode() {
      var json = JsonObject.Parse(this);

      return json.GetHashCode();
    }

  } // class BalancesQuery

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
