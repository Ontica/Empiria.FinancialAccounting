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

    private decimal MonedaNacional {
      get; set;
    }


    private decimal MonedaExtranjera {
      get; set;
    }


    public decimal TotalBalance {
      get {
        return MonedaNacional + MonedaExtranjera;
      }
    }

    #endregion Fields

    #region Methods


    public override ReportEntryTotals AbsoluteValue() {
      return new AnaliticoCuentasReportEntryTotals {
        MonedaNacional = Math.Abs(this.MonedaNacional),
        MonedaExtranjera = Math.Abs(this.MonedaExtranjera)
      };
    }


    public override void CopyTotalsTo(FinancialReportEntry copyTo) {
      copyTo.SetTotalField(FinancialReportTotalField.monedaNacional,
                           this.MonedaNacional);
      copyTo.SetTotalField(FinancialReportTotalField.monedaExtranjera,
                           this.MonedaExtranjera);
      copyTo.SetTotalField(FinancialReportTotalField.total,
                           this.TotalBalance);
    }


    public override ReportEntryTotals Round() {
      return new AnaliticoCuentasReportEntryTotals {
        MonedaNacional = Math.Round(this.MonedaNacional, 0),
        MonedaExtranjera = Math.Round(this.MonedaExtranjera, 0)
      };
    }


    public override ReportEntryTotals Substract(ReportEntryTotals total, string dataColumn) {
      var castTotal = (AnaliticoCuentasReportEntryTotals) total;

      if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          MonedaNacional = this.MonedaNacional,
          MonedaExtranjera = this.MonedaExtranjera - (castTotal.MonedaNacional + castTotal.MonedaExtranjera)
        };
      }
      return new AnaliticoCuentasReportEntryTotals {
        MonedaNacional = this.MonedaNacional - castTotal.MonedaNacional,
        MonedaExtranjera = this.MonedaExtranjera - castTotal.MonedaExtranjera
      };
    }


    public override ReportEntryTotals Substract(ITrialBalanceEntryDto balance, string dataColumn) {
      var analiticoBalance = (AnaliticoDeCuentasEntryDto) balance;

      if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          MonedaNacional = this.MonedaNacional,
          MonedaExtranjera = this.MonedaExtranjera - (analiticoBalance.DomesticBalance + analiticoBalance.ForeignBalance)
        };
      }

      return new AnaliticoCuentasReportEntryTotals {
        MonedaNacional = this.MonedaNacional - analiticoBalance.DomesticBalance,
        MonedaExtranjera = this.MonedaExtranjera - analiticoBalance.ForeignBalance
      };
    }


    public override ReportEntryTotals Sum(ReportEntryTotals total, string dataColumn) {
      var castTotal = (AnaliticoCuentasReportEntryTotals) total;

      if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          MonedaNacional = this.MonedaNacional,
          MonedaExtranjera = this.MonedaExtranjera + (castTotal.MonedaNacional + castTotal.MonedaExtranjera)
        };
      }

      return new AnaliticoCuentasReportEntryTotals {
        MonedaNacional = this.MonedaNacional + castTotal.MonedaNacional,
        MonedaExtranjera = this.MonedaExtranjera + castTotal.MonedaExtranjera
      };
    }


    public override ReportEntryTotals Sum(ITrialBalanceEntryDto balance, string dataColumn) {
      var analiticoBalance = (AnaliticoDeCuentasEntryDto) balance;

      if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          MonedaNacional = this.MonedaNacional,
          MonedaExtranjera = this.MonedaExtranjera + (analiticoBalance.DomesticBalance + analiticoBalance.ForeignBalance)
        };
      }

      return new AnaliticoCuentasReportEntryTotals {
        MonedaNacional = this.MonedaNacional + analiticoBalance.DomesticBalance,
        MonedaExtranjera = this.MonedaExtranjera + analiticoBalance.ForeignBalance
      };
    }


    public override ReportEntryTotals Sum(ExternalValue value, string dataColumn) {
      if (dataColumn == "MonedaNacional") {
        return new AnaliticoCuentasReportEntryTotals {
          MonedaNacional = value.GetTotalField("monedaNacional") + value.GetTotalField("monedaExtranjera")
        };

      } else if (dataColumn == "MonedaExtranjera") {
        return new AnaliticoCuentasReportEntryTotals {
          MonedaExtranjera = value.GetTotalField("monedaNacional") + value.GetTotalField("monedaExtranjera")
        };

      } else {
        return new AnaliticoCuentasReportEntryTotals {
          MonedaNacional = value.GetTotalField("monedaNacional"),
          MonedaExtranjera = value.GetTotalField("monedaExtranjera")
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
