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
using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>ReportEntryTotals for use in reports based on BalanzaEnColumnasPorMoneda (ACLME).</summary>
  internal class BalanzaEnColumnasPorMonedaReportEntryTotals : ReportEntryTotals {

    #region Fields

    public decimal PesosTotal {
      get; private set;
    }


    public decimal DollarTotal {
      get; private set;
    }


    public decimal YenTotal {
      get; private set;
    }


    public decimal EuroTotal {
      get; private set;
    }


    public decimal UdisTotal {
      get; private set;
    }


    public decimal ForeignCurrencyTotal {
      get {
        return DollarTotal + YenTotal + EuroTotal + UdisTotal;
      }
    }

    public decimal TotalBalance {
      get {
        return PesosTotal + ForeignCurrencyTotal;
      }
    }

    #endregion Fields

    #region Methods

    public override ReportEntryTotals AbsoluteValue() {
      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        PesosTotal = Math.Abs(this.PesosTotal),
        DollarTotal = Math.Abs(this.DollarTotal),
        YenTotal = Math.Abs(this.YenTotal),
        EuroTotal = Math.Abs(this.EuroTotal),
        UdisTotal = Math.Abs(this.UdisTotal)
      };
    }

    public override void CopyTotalsTo(FinancialReportEntry copyTo) {
      copyTo.SetTotalField(FinancialReportTotalField.pesosTotal,
                           this.PesosTotal);

      copyTo.SetTotalField(FinancialReportTotalField.dollarTotal,
                           this.DollarTotal);

      copyTo.SetTotalField(FinancialReportTotalField.yenTotal,
                           this.YenTotal);

      copyTo.SetTotalField(FinancialReportTotalField.euroTotal,
                           this.EuroTotal);

      copyTo.SetTotalField(FinancialReportTotalField.udisTotal,
                           this.UdisTotal);
    }


    public override ReportEntryTotals Round() {
      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        PesosTotal = Math.Round(this.PesosTotal, 0),
        DollarTotal = Math.Round(this.DollarTotal, 0),
        YenTotal = Math.Round(this.YenTotal, 0),
        EuroTotal = Math.Round(this.EuroTotal, 0),
        UdisTotal = Math.Round(this.UdisTotal, 0)
      };
    }


    public override ReportEntryTotals Substract(ReportEntryTotals total, string dataColumn) {
      var castTotal = (BalanzaEnColumnasPorMonedaReportEntryTotals) total;

      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        PesosTotal = this.PesosTotal - castTotal.PesosTotal,
        DollarTotal = this.DollarTotal - castTotal.DollarTotal,
        YenTotal = this.YenTotal - castTotal.YenTotal,
        EuroTotal = this.EuroTotal - castTotal.EuroTotal,
        UdisTotal = this.UdisTotal - castTotal.UdisTotal,
      };
    }


    public override ReportEntryTotals Substract(ITrialBalanceEntryDto balance, string dataColumn) {
      var castBalance = (TrialBalanceByCurrencyDto) balance;

      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        PesosTotal = this.PesosTotal - castBalance.DomesticBalance,
        DollarTotal = this.DollarTotal - castBalance.DollarBalance,
        YenTotal = this.YenTotal - castBalance.YenBalance,
        EuroTotal = this.EuroTotal - castBalance.EuroBalance,
        UdisTotal = this.UdisTotal - castBalance.UdisBalance,
      };
    }


    public override ReportEntryTotals Sum(ReportEntryTotals total, string dataColumn) {
      var castTotal = (BalanzaEnColumnasPorMonedaReportEntryTotals) total;

      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        PesosTotal = this.PesosTotal + castTotal.PesosTotal,
        DollarTotal = this.DollarTotal + castTotal.DollarTotal,
        YenTotal = this.YenTotal + castTotal.YenTotal,
        EuroTotal = this.EuroTotal + castTotal.EuroTotal,
        UdisTotal = this.UdisTotal + castTotal.UdisTotal,
      };
    }


    public override ReportEntryTotals Sum(ITrialBalanceEntryDto balance, string dataColumn) {
      var castBalance = (TrialBalanceByCurrencyDto) balance;

      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        PesosTotal = this.PesosTotal + castBalance.DomesticBalance,
        DollarTotal = this.DollarTotal + castBalance.DollarBalance,
        YenTotal = this.YenTotal + castBalance.YenBalance,
        EuroTotal = this.EuroTotal + castBalance.EuroBalance,
        UdisTotal = this.UdisTotal + castBalance.UdisBalance,
      };
    }


    public override ReportEntryTotals Sum(ExternalValue value, string dataColumn) {
      return new BalanzaEnColumnasPorMonedaReportEntryTotals {
        PesosTotal = this.PesosTotal + value.DomesticCurrencyValue
      };
    }


    public override ReportEntryTotals SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance, string dataColumn) {
      throw new NotImplementedException();

      // var castBalance = (TrialBalanceByCurrencyDto) balance;

      // ToDo: Add DebtorCreditor property to TrialBalanceByCurrencyDto

      //if (castBalance.DebtorCreditor == DebtorCreditorType.Deudora) {
      //  return Sum(balance, dataColumn);
      //} else {
      //  return Substract(balance, dataColumn);
      //}
    }

    #endregion Methods

  }  // class BalanzaEnColumnasPorMonedaReportEntryTotals

}  // namespace Empiria.FinancialAccounting.FinancialReports
