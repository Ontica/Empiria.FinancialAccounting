/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : ResumenAjusteEntry                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for resumen de ajuste anual entry.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for resumen de ajuste anual entry</summary>
  internal class ResumenAjusteAnualEntry : ITrialBalanceEntry {

    public DateTime FiscalYearDate {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }


    public decimal AverageBalanceCredit {
      get; internal set;
    }


    public decimal AverageBalanceDebit {
      get; internal set;
    }


    /// <summary>Debit > Credit = promedios Credito - Deuda</summary>
    public decimal CumulativeAdjustment {
      get; internal set;
    }


    /// <summary>Credit > Debit = promedios Deuda - Credito</summary>
    public decimal DeductibleAdjustment {
      get; internal set;
    }


    /// <summary>Cumulative/Deductible</summary>
    public decimal InflationAdjustmentMonths {
      get; internal set;
    }


    /// <summary>Cumulative/Deductible</summary>
    public decimal InflationAdjustment12Months {
      get; internal set;
    }


    public decimal INPCLastMonthPreviousYear {
      get; internal set;
    }


    public decimal INPCPreviousYear {
      get; internal set;
    }


    public decimal INPC {
      get; internal set;
    }


    public decimal FactorMonthsElapsed {
      get; internal set;
    }


    public decimal Factor12Months {
      get; internal set;
    }


    #region Public methods

    internal void CalculateFields(FixedList<ResumenAjusteEntry> balancesByDate,
                                  FixedList<ResumenAjusteEntry> entries, DateTime date) {
      this.FiscalYearDate = date;

      this.Credit = CalculateCreditField(balancesByDate);
      this.Debit = CalculateDebitField(balancesByDate);

      CalculateAverageBalanceCreditField(entries, date);
      CalculateAverageBalanceDebitField(entries, date);

      CalculateCumulativeAdjustment();
      CalculateDeductibleAdjustment();

      CalculateINPC();
      CalculateFactorFields();

      CalculateInflationAdjustment12Months();
      CalculateInflationAdjustmentMonths();
    }

    

    #endregion Public methods


    #region Private methods

    private void CalculateAverageBalanceCreditField(FixedList<ResumenAjusteEntry> entries,
                                                              DateTime date) {
      List<decimal> totalAverageCredit = new List<decimal>();

      for (int i = 1; i <= date.Month; i++) {

        var balances = entries.FindAll(x =>
                                       x.ConsultingDate == new DateTime(date.Year, i,
                                                                        DateTime.DaysInMonth(date.Year, i)));

        totalAverageCredit.Add(CalculateCreditField(balances));
      }

      this.AverageBalanceCredit = Math.Round(totalAverageCredit.Average(), 2);
    }


    private void CalculateAverageBalanceDebitField(FixedList<ResumenAjusteEntry> entries,
                                                             DateTime date) {
      List<decimal> totalAverageDebit = new List<decimal>();

      for (int i = 1; i <= date.Month; i++) {

        var balances = entries.FindAll(x =>
                                       x.ConsultingDate == new DateTime(date.Year, i,
                                                                        DateTime.DaysInMonth(date.Year, i)));

        totalAverageDebit.Add(CalculateDebitField(balances));
      }

      this.AverageBalanceDebit = Math.Round(totalAverageDebit.Average(), 2);
    }


    static private decimal CalculateCreditField(FixedList<ResumenAjusteEntry> balancesByDate) {

      return balancesByDate.FindAll(x => x.Classification == AccountClassification.Credito &&
                                             x.KeyAdjustment != KeyAdjustmentTypes.NO)
                                    .Sum(x => x.TotalValorized);
    }


    private void CalculateCumulativeAdjustment() {

      this.CumulativeAdjustment = Math.Round(this.AverageBalanceDebit > this.AverageBalanceCredit ?
                                  this.AverageBalanceCredit - this.AverageBalanceDebit : 0, 2);
    }


    static private decimal CalculateDebitField(FixedList<ResumenAjusteEntry> balancesByDate) {

      return balancesByDate.FindAll(x => x.Classification == AccountClassification.Deuda &&
                                             x.KeyAdjustment != KeyAdjustmentTypes.NO)
                                    .Sum(x => x.TotalValorized);
    }


    private void CalculateDeductibleAdjustment() {

      this.DeductibleAdjustment = Math.Round(this.AverageBalanceCredit > this.AverageBalanceDebit ?
                                  this.AverageBalanceDebit - this.AverageBalanceCredit : 0, 2);
    }


    private void CalculateFactorFields() {
      this.FactorMonthsElapsed = Math.Round((this.INPC / this.INPCLastMonthPreviousYear) - 1, 4);
      this.Factor12Months = Math.Round((this.INPC / this.INPCPreviousYear) - 1, 4);
    }


    private void CalculateInflationAdjustment12Months() {

      this.InflationAdjustment12Months = Math.Round((this.CumulativeAdjustment + this.DeductibleAdjustment) *
                                          this.Factor12Months, 2);
    }


    private void CalculateInflationAdjustmentMonths() {

      this.InflationAdjustmentMonths = Math.Round((this.CumulativeAdjustment + this.DeductibleAdjustment) *
                                        this.FactorMonthsElapsed, 2);
    }


    private void CalculateINPC() {
      //TODO CONSEGUIR CATALOGO DE INPC
      this.INPCLastMonthPreviousYear = Math.Round(133.555000000000M, 12);
      this.INPCPreviousYear = Math.Round(133.555000000000M, 12);
      this.INPC = Math.Round(138.343000000000M, 12);
    }

    #endregion Private methods

  } // class ResumenAjusteAnualEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
