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


    public DateTime FromDate {
      get; set;
    }


    public DateTime ToDate {
      get; set;
    }


    public DateTime FromDateComparative {
      get; set;
    }


    public DateTime ToDateComparative {
      get; set;
    }


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


    public string ValuateToCurrrencyUID {
      get; set;
    } = string.Empty;


    public string ExchangeRateTypeUID {
      get; set;
    } = string.Empty;


    public DateTime ExchangeRateDate {
      get; set;
    }


    public string ValuateToCurrrencyComparativeUID {
      get; set;
    } = string.Empty;


    public string ExchangeRateTypeComparativeUID {
      get; set;
    } = string.Empty;


    public DateTime ExchangeRateDateComparative {
      get; set;
    }


    public bool ConsolidateBalancesToTargetCurrency {
      get; set;
    }


    public bool Consolidated {
      get {
        return !ShowCascadeBalances;
      }
    }


    public bool DoNotReturnSubledgerAccounts {
      get {
        return !this.ReturnSubledgerAccounts;
      }
    }


    public bool ReturnSubledgerAccounts {
      get {
        return this.TrialBalanceType == TrialBalanceType.BalanzaConAuxiliares ||
               this.TrialBalanceType == TrialBalanceType.Saldos ||
               this.TrialBalanceType == TrialBalanceType.GeneracionDeSaldos ||
               this.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar ||
               this.TrialBalanceType == TrialBalanceType.SaldosPorCuenta;
      }
    }

    public bool ValuateBalances {
      get {
        return this.ValuateToCurrrencyUID.Length != 0 &&
               this.ExchangeRateTypeUID.Length != 0;
      }
    }


    public bool ReturnLedgerColumn {
      get {
        return (!this.Consolidated &&
                !this.ConsolidateBalancesToTargetCurrency &&
                this.Ledgers.Length > 1);
      }
    }

  } // class TrialBalanceCommand


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
