/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Query payload                           *
*  Type     : TrialBalanceQuery                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to generate trial balances.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Query payload used to generate trial balances.</summary>
  public class TrialBalanceQuery {

    public TrialBalanceType TrialBalanceType {
      get; set;
    }

    public string AccountsChartUID {
      get; set;
    }


    public bool ShowCascadeBalances {
      get; set;
    } = true;


    public string[] Ledgers {
      get; set;
    } = new string[0];


    public string[] Currencies {
      get; set;
    } = new string[0];


    public string[] Sectors {
      get; set;
    } = new string[0];


    public string FromAccount {
      get; set;
    } = string.Empty;


    public string ToAccount {
      get; set;
    } = string.Empty;


    public string SubledgerAccount {
      get; set;
    } = string.Empty;


    public int Level {
      get; set;
    }


    public BalancesType BalancesType {
      get; set;
    } = BalancesType.AllAccounts;


    public BalancesPeriod InitialPeriod {
      get; set;
    } = new BalancesPeriod();


    public BalancesPeriod FinalPeriod {
      get; set;
    } = new BalancesPeriod();


    public bool ConsolidateBalancesToTargetCurrency {
      get; set;
    }

    public bool UseCache {
      get; set;
    } = true;


    public bool Consolidated {
      get {
        return !ShowCascadeBalances;
      }
    }


    public bool DoNotReturnSubledgerAccounts {
      get {
        return !this.WithSubledgerAccount;
      }
    }


    public bool WithSubledgerAccount {
      get; set;
    } = false;


    public bool UseDefaultValuation {
      get; set;
    } = false;


    public bool WithAverageBalance {
      get; set;
    } = false;


    public bool WithSectorization {
      get; set;
    } = false;


    public FileReportVersion ExportTo {
      get; set;
    } = FileReportVersion.V1;


    public bool UseNewSectorizationModel {
      get {
        return this.AccountsChartUID == "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      }
    }


    public bool ValuateBalances {
      get {
        return this.InitialPeriod.ValuateToCurrrencyUID.Length != 0 &&
               this.InitialPeriod.ExchangeRateTypeUID.Length != 0;
      }
    }


    public bool ValuateFinalBalances {
      get {
        return this.FinalPeriod.ValuateToCurrrencyUID.Length != 0 &&
               this.FinalPeriod.ExchangeRateTypeUID.Length != 0;
      }
    }


    public bool ReturnLedgerColumn {
      get {
        return (!this.Consolidated &&
                !this.ConsolidateBalancesToTargetCurrency);
      }
    }


    public bool IsOperationalReport {
      get; set;
    } = false;


    public override bool Equals(object obj) => this.Equals(obj as TrialBalanceQuery);

    public bool Equals(TrialBalanceQuery query) {
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


  } // class TrialBalanceQuery

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
