/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                             Component : Domain Layer                         *
*  Assembly : FinancialAccounting.FinancialReports.dll      Pattern   : Service provider                     *
*  Type     : BalanzaEnColumnasPorMonedaReportEntryTotals   License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : ReportEntryTotals for use in reports based on BalanzaEnColumnasPorMoneda (ACLME).              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Rules;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>ReportEntryTotals for use in reports based on BalanzaEnColumnasPorMoneda (ACLME).</summary>
  internal class BalanzaEnColumnasPorMonedaReportEntryTotals : ReportEntryTotals {

    #region Fields

    public decimal DomesticCurrencyTotal {
      get; private set;
    }


    public decimal DollarBalance {
      get; private set;
    }


    public decimal YenBalance {
      get; private set;
    }


    public decimal EuroBalance {
      get; private set;
    }


    public decimal UdisBalance {
      get; private set;
    }


    public decimal ForeignCurrencyTotal {
      get {
        return DollarBalance + YenBalance + EuroBalance + UdisBalance;
      }
    }

    public decimal TotalBalance {
      get {
        return DomesticCurrencyTotal + ForeignCurrencyTotal;
      }
    }

    #endregion Fields

    #region Methods

    public override ReportEntryTotals Round() {
      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        DomesticCurrencyTotal = Math.Round(this.DomesticCurrencyTotal, 0),
        DollarBalance = Math.Round(this.DollarBalance, 0),
        YenBalance = Math.Round(this.YenBalance, 0),
        EuroBalance = Math.Round(this.EuroBalance, 0),
        UdisBalance = Math.Round(this.UdisBalance, 0)
      };
    }


    public override ReportEntryTotals Substract(ReportEntryTotals total, string qualification) {
      var castTotal = (BalanzaEnColumnasPorMonedaReportEntryTotals) total;

      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - castTotal.DomesticCurrencyTotal,
        DollarBalance = this.DollarBalance - castTotal.DollarBalance,
        YenBalance = this.YenBalance - castTotal.YenBalance,
        EuroBalance = this.EuroBalance - castTotal.EuroBalance,
        UdisBalance = this.UdisBalance - castTotal.UdisBalance,
      };
    }


    public override ReportEntryTotals Substract(ITrialBalanceEntryDto balance, string qualification) {
      var castBalance = (TrialBalanceByCurrencyDto) balance;

      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - castBalance.DomesticBalance,
        DollarBalance = this.DollarBalance - castBalance.DollarBalance,
        YenBalance = this.YenBalance - castBalance.YenBalance,
        EuroBalance = this.EuroBalance - castBalance.EuroBalance,
        UdisBalance = this.UdisBalance - castBalance.UdisBalance,
      };
    }


    public override ReportEntryTotals Sum(ReportEntryTotals total, string qualification) {
      var castTotal = (BalanzaEnColumnasPorMonedaReportEntryTotals) total;

      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + castTotal.DomesticCurrencyTotal,
        DollarBalance = this.DollarBalance + castTotal.DollarBalance,
        YenBalance = this.YenBalance + castTotal.YenBalance,
        EuroBalance = this.EuroBalance + castTotal.EuroBalance,
        UdisBalance = this.UdisBalance + castTotal.UdisBalance,
      };
    }


    public override ReportEntryTotals Sum(ITrialBalanceEntryDto balance, string qualification) {
      var castBalance = (TrialBalanceByCurrencyDto) balance;

      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + castBalance.DomesticBalance,
        DollarBalance = this.DollarBalance + castBalance.DollarBalance,
        YenBalance = this.YenBalance + castBalance.YenBalance,
        EuroBalance = this.EuroBalance + castBalance.EuroBalance,
        UdisBalance = this.UdisBalance + castBalance.UdisBalance,
      };
    }


    public override ReportEntryTotals Sum(ExternalValue value, string qualification) {
      throw new NotImplementedException();
    }


    public override ReportEntryTotals SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance, string qualification) {
      throw new NotImplementedException();

      // var castBalance = (TrialBalanceByCurrencyDto) balance;

      // ToDo: Add DebtorCreditor property to TrialBalanceByCurrencyDto

      //if (castBalance.DebtorCreditor == DebtorCreditorType.Deudora) {
      //  return Sum(balance, qualification);
      //} else {
      //  return Substract(balance, qualification);
      //}
    }


    public override void CopyTotalsTo(FinancialReportEntry copyTo) {
      copyTo.SetTotalField(FinancialReportTotalField.DomesticCurrencyTotal,
                           this.DomesticCurrencyTotal);
      copyTo.SetTotalField(FinancialReportTotalField.Total,
                           this.TotalBalance);
    }


    #endregion Methods

  }  // class BalanzaEnColumnasPorMonedaReportEntryTotals

}  // namespace Empiria.FinancialAccounting.FinancialReports
