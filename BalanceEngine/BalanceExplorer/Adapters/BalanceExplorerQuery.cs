/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Query payload                           *
*  Type     : BalanceExplorerQuery                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to build balances for the balances explorer.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters {

  /// <summary>Query payload used to build balances for the balances explorer.</summary>
  public class BalanceExplorerQuery {

    public TrialBalanceType TrialBalanceType {
      get; set;
    }


    public string AccountsChartUID {
      get; set;
    }


    public string FromAccount {
      get; set;
    } = string.Empty;


    public string[] Accounts {
      get; set;
    } = new string[0];


    public string SubledgerAccount {
      get; set;
    } = string.Empty;


    public int SubledgerAccountID {
      get; set;
    }


    public string[] SubledgerAccounts {
      get; set;
    } = new string[0];


    public bool WithSubledgerAccount {
      get; set;
    }


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
    }


    public bool UseCache {
      get; set;
    } = true;
    

    public override bool Equals(object obj) => Equals(obj as BalanceExplorerQuery);

    public bool Equals(BalanceExplorerQuery query) {
      if (query == null) {
        return false;
      }
      if (Object.ReferenceEquals(this, query)) {
        return true;
      }
      if (this.GetType() != query.GetType()) {
        return false;
      }

      return this.GetHashCode() == query.GetHashCode();
    }


    public override int GetHashCode() {
      var json = JsonObject.Parse(this);

      return json.GetHashCode();
    }

  } // class BalanceExplorerQuery

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
