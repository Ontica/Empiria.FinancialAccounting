/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : AnaliticoCuentasReportEntryTotals          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : ReportEntryTotals for use in reports based on Analítico de cuentas (R01, R10, R12).            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.ExternalData;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>ReportEntryTotals for use in reports based on Analítico de cuentas (R01, R10, R12).</summary>
  internal class AnaliticoCuentasReportEntryTotals : ReportEntryTotals {

    #region Fields

    public decimal DomesticCurrencyTotal {
      get; private set;
    }


    public decimal ForeignCurrencyTotal {
      get; private set;
    }


    public decimal TotalBalance {
      get {
        return DomesticCurrencyTotal + ForeignCurrencyTotal;
      }
    }

    #endregion Fields

    #region Methods


    public override ReportEntryTotals AbsoluteValue() {
      return new AnaliticoCuentasReportEntryTotals {
        DomesticCurrencyTotal = Math.Abs(this.DomesticCurrencyTotal),
        ForeignCurrencyTotal = Math.Abs(this.ForeignCurrencyTotal)
      };
    }


    public override void CopyTotalsTo(FinancialReportEntry copyTo) {
      copyTo.SetTotalField(FinancialReportTotalField.domesticCurrencyTotal,
                           this.DomesticCurrencyTotal);
      copyTo.SetTotalField(FinancialReportTotalField.foreignCurrencyTotal,
                           this.ForeignCurrencyTotal);
      copyTo.SetTotalField(FinancialReportTotalField.total,
                           this.TotalBalance);
    }


    public override ReportEntryTotals Round() {
      return new AnaliticoCuentasReportEntryTotals {
        DomesticCurrencyTotal = Math.Round(this.DomesticCurrencyTotal, 0),
        ForeignCurrencyTotal = Math.Round(this.ForeignCurrencyTotal, 0)
      };
    }


    public override ReportEntryTotals Substract(ReportEntryTotals total, string dataColumn) {
      var castTotal = (AnaliticoCuentasReportEntryTotals) total;

      if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal - (castTotal.DomesticCurrencyTotal + castTotal.ForeignCurrencyTotal)
        };
      }
      return new AnaliticoCuentasReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - castTotal.DomesticCurrencyTotal,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal - castTotal.ForeignCurrencyTotal
      };
    }


    public override ReportEntryTotals Substract(ITrialBalanceEntryDto balance, string dataColumn) {
      var analiticoBalance = (AnaliticoDeCuentasEntryDto) balance;

      if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal - (analiticoBalance.DomesticBalance + analiticoBalance.ForeignBalance)
        };
      }

      return new AnaliticoCuentasReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - analiticoBalance.DomesticBalance,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal - analiticoBalance.ForeignBalance
      };
    }


    public override ReportEntryTotals Sum(ReportEntryTotals total, string dataColumn) {
      var castTotal = (AnaliticoCuentasReportEntryTotals) total;

      if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal + (castTotal.DomesticCurrencyTotal + castTotal.ForeignCurrencyTotal)
        };
      }

      return new AnaliticoCuentasReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + castTotal.DomesticCurrencyTotal,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal + castTotal.ForeignCurrencyTotal
      };
    }


    public override ReportEntryTotals Sum(ITrialBalanceEntryDto balance, string dataColumn) {
      var analiticoBalance = (AnaliticoDeCuentasEntryDto) balance;

      if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal + (analiticoBalance.DomesticBalance + analiticoBalance.ForeignBalance)
        };
      }

      return new AnaliticoCuentasReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + analiticoBalance.DomesticBalance,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal + analiticoBalance.ForeignBalance
      };
    }


    public override ReportEntryTotals Sum(ExternalValue value, string dataColumn) {
      if (dataColumn == "MonedaNacional") {
        return new AnaliticoCuentasReportEntryTotals {
          DomesticCurrencyTotal = value.DomesticCurrencyValue + value.ForeignCurrencyValue
        };

      } else if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          ForeignCurrencyTotal = value.DomesticCurrencyValue + value.ForeignCurrencyValue
        };

      } else {
        return new AnaliticoCuentasReportEntryTotals {
          DomesticCurrencyTotal = value.DomesticCurrencyValue,
          ForeignCurrencyTotal = value.ForeignCurrencyValue
        };
      }
    }


    public override ReportEntryTotals SumDebitsOrSubstractCredits(ITrialBalanceEntryDto balance, string dataColumn) {
      var analiticoBalance = (AnaliticoDeCuentasEntryDto) balance;

      if (analiticoBalance.DebtorCreditor == DebtorCreditorType.Deudora) {
        return Sum(balance, dataColumn);
      } else {
        return Substract(balance, dataColumn);
      }
    }


    #endregion Methods

  }  // class AnaliticoCuentasReportEntryTotals

}  // namespace Empiria.FinancialAccounting.FinancialReports
