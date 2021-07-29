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


    public TrialBalanceCommandPeriod InitialPeriod {
      get; set;
    } = new TrialBalanceCommandPeriod();


    public TrialBalanceCommandPeriod FinalPeriod {
      get; set;
    } = new TrialBalanceCommandPeriod();


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
                !this.ConsolidateBalancesToTargetCurrency &&
                this.Ledgers.Length > 1);
      }
    }

  } // class TrialBalanceCommand

  public class TrialBalanceCommandPeriod {
    public DateTime FromDate {
      get; set;
    }

    public DateTime ToDate {
      get; set;
    }

    public DateTime ExchangeRateDate {
      get; set;
    } = DateTime.Now;

    public string ExchangeRateTypeUID {
      get; set;
    } = string.Empty;

    public string ValuateToCurrrencyUID {
      get; set;
    } = string.Empty;

    public bool UseDefaultValuation {
      get; set;
    } = false;

  }


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
