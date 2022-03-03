/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information holder                      *
*  Type     : ReportEntryTotals                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a financial report entry total.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Holds information about a financial report entry total.</summary>
  internal class ReportEntryTotals {

    #region Fields

    public decimal DomesticCurrencyTotal {
      get; internal set;
    }


    public decimal ForeignCurrencyTotal {
      get; internal set;
    }


    public decimal TotalBalance {
      get {
        return DomesticCurrencyTotal + ForeignCurrencyTotal;
      }
    }

    #endregion Fields

    #region Methods

    internal void Round() {
      this.DomesticCurrencyTotal = Math.Round(this.DomesticCurrencyTotal, 0);
      this.ForeignCurrencyTotal = Math.Round(this.ForeignCurrencyTotal, 0);
    }


    internal ReportEntryTotals Substract(ReportEntryTotals total, string qualification) {
      if (qualification == "MonedaExtranjera") {
        return new ReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal - (total.DomesticCurrencyTotal + total.ForeignCurrencyTotal)
        };
      }
      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - total.DomesticCurrencyTotal,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal - total.ForeignCurrencyTotal
      };
    }


    internal ReportEntryTotals Substract(ITrialBalanceEntryDto balance, string qualification) {
      var analiticoBalance = (TwoColumnsTrialBalanceEntryDto) balance;

      if (qualification == "MonedaExtranjera") {
        return new ReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal - (analiticoBalance.DomesticBalance + analiticoBalance.ForeignBalance)
        };
      }

      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - analiticoBalance.DomesticBalance,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal - analiticoBalance.ForeignBalance
      };
    }


    internal ReportEntryTotals Sum(ReportEntryTotals total, string qualification) {
      if (qualification == "MonedaExtranjera") {
        return new ReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal + (total.DomesticCurrencyTotal + total.ForeignCurrencyTotal)
        };
      }

      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + total.DomesticCurrencyTotal,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal + total.ForeignCurrencyTotal
      };
    }


    internal ReportEntryTotals Sum(ITrialBalanceEntryDto balance, string qualification) {
      var analiticoBalance = (TwoColumnsTrialBalanceEntryDto) balance;

      if (qualification == "MonedaExtranjera") {
        return new ReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal + (analiticoBalance.DomesticBalance + analiticoBalance.ForeignBalance)
        };
      }

      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + analiticoBalance.DomesticBalance,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal + analiticoBalance.ForeignBalance
      };
    }


    internal ReportEntryTotals SumDebitsAndSubstractCredits(ITrialBalanceEntryDto balance, string qualification) {
      var analiticoBalance = (TwoColumnsTrialBalanceEntryDto) balance;

      if (analiticoBalance.DebtorCreditor == DebtorCreditorType.Deudora) {
        return Sum(balance, qualification);
      } else {
        return Substract(balance, qualification);
      }
    }

    #endregion Methods

  }  // class ReportEntryTotals

}  // namespace Empiria.FinancialAccounting.FinancialReports
