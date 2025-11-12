/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaDolarizadaMapper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map resumen ajuste anual.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map resumen ajuste anual</summary>
  static internal class ResumenAjusteAnualMapper {

    #region Public methods

    static internal ResumenAjusteAnualDto Map(TrialBalanceQuery query,
                                             FixedList<ResumenAjusteAnualEntry> entries) {

      return new ResumenAjusteAnualDto {
        Query = query,
        Columns = DataColumns(query.InitialPeriod.ToDate),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }


    static public FixedList<DataTableColumn> DataColumns(DateTime toDate) {

      var previousYear = toDate.AddYears(-1).Year;

      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("fiscalYearDate", "Fechas", "date"));
      columns.Add(new DataTableColumn("credit", "Créditos", "decimal"));
      columns.Add(new DataTableColumn("debit", "Deudas", "decimal"));
      columns.Add(new DataTableColumn("averageBalanceCredit", "Saldo promedio anual créditos", "decimal"));
      columns.Add(new DataTableColumn("averageBalanceDebit", "Saldo promedio anual deudas", "decimal"));
      columns.Add(new DataTableColumn("cumulativeAdjustment", "Ajuste Anual Acumulable", "decimal"));
      columns.Add(new DataTableColumn("deductibleAdjustment", "Ajuste Anual Deducible", "decimal")); // COL SUGERENCIA
      columns.Add(new DataTableColumn(
        "inflationAdjustmentMonths",
        "Ajuste anual por inflación por los meses trasncurridos acumulable (deducible)", "decimal"));

      columns.Add(new DataTableColumn(
        "inflationAdjustment12Months",
        "Ajuste anual por inflación por 12 meses acumulable (deducible)", "decimal"));

      columns.Add(new DataTableColumn(
        "iNPCLastMonthPreviousYear",
        $"INPC 31 de diciembre {previousYear}", "decimal", 12));

      columns.Add(new DataTableColumn("iNPCPreviousYear", $"INPC {previousYear}", "decimal", 12));
      columns.Add(new DataTableColumn("iNPC", $"INPC {toDate.Year}", "decimal", 12));
      columns.Add(new DataTableColumn("factorMonthsElapsed", "Factor meses transcurridos", "decimal", 4));
      columns.Add(new DataTableColumn("factor12Months", "Factor 12 meses", "decimal", 4));
      
      return columns.ToFixedList();
    }


    static public ResumenAjusteAnualEntryDto MapEntry(ResumenAjusteAnualEntry x) {

      return new ResumenAjusteAnualEntryDto {
        FiscalYearDate = x.FiscalYearDate,
        Credit = x.Credit,
        Debit = x.Debit,
        AverageBalanceCredit = x.AverageBalanceCredit,
        AverageBalanceDebit = x.AverageBalanceDebit,
        CumulativeAdjustment = x.CumulativeAdjustment,
        DeductibleAdjustment = x.DeductibleAdjustment, // COL SUGERENCIA
        InflationAdjustmentMonths = x.InflationAdjustmentMonths,
        InflationAdjustment12Months = x.InflationAdjustment12Months,
        INPCLastMonthPreviousYear = x.INPCLastMonthPreviousYear,
        INPCPreviousYear = x.INPCPreviousYear,
        INPC = x.INPC,
        FactorMonthsElapsed = x.FactorMonthsElapsed,
        Factor12Months = x.Factor12Months
      };
    }


    #endregion Public methods


    #region Private methods



    #endregion Private methods

  } // class ResumenAjusteAnualMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
