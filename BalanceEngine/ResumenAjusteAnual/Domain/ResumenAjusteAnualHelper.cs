/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanzaColumnasMonedaHelper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balanza resumen ajuste anual.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build balanza resumen ajuste anual</summary>
  internal class ResumenAjusteAnualHelper {

    private readonly TrialBalanceQuery Query;
    private DateTime fromDateFlag;
    private DateTime toDateFlag;

    internal ResumenAjusteAnualHelper(TrialBalanceQuery query) {
      Query = query;
      fromDateFlag = this.Query.InitialPeriod.FromDate;
      toDateFlag = this.Query.InitialPeriod.ToDate;
    }

    #region Public methods

    internal FixedList<ResumenAjusteAnualEntry> GetBalancesByMonths() {

      FixedList<DateTime> monthsInYear = GetMonthsInYear();

      FixedList<ResumenAjusteAnualEntry> balancesByMonth = GetResumeBalancesByMonth(monthsInYear);

      return new FixedList<ResumenAjusteAnualEntry>(balancesByMonth);
    }

    #endregion Public methods

    #region Private methods

    private FixedList<ResumenAjusteAnualEntry> ConvertBalanceEntryIntoResumeEntry(
                                                FixedList<TrialBalanceEntry> entriesFromBalanza) {
      List<ResumenAjusteAnualEntry> resumeByMonth = new List<ResumenAjusteAnualEntry>();

      foreach (var entry in entriesFromBalanza) {
        ResumenAjusteAnualEntry resumeEntry = new ResumenAjusteAnualEntry();

        resumeEntry.MapFromTrialBalanceEntry(entry);
        resumeEntry.ConsultingDate = Query.InitialPeriod.ToDate;

        resumeEntry.KeyAdjustment = KeyAdjustmentTypes.SI; // TODO CARGAR CATALOGO
        resumeEntry.CalculateFields();

        resumeByMonth.Add(resumeEntry);
      }

      return new FixedList<ResumenAjusteAnualEntry>(resumeByMonth);
    }


    private FixedList<TrialBalanceEntry> GetEntriesFromBalanza(DateTime monthFilter) {

      Query.InitialPeriod.FromDate = new DateTime(monthFilter.Year, monthFilter.Month, 1);
      Query.InitialPeriod.ToDate = monthFilter;

      var helper = new BalanzaTradicionalHelper(Query);

      FixedList<TrialBalanceEntry> accountEntries = helper.GetPostingEntries();

      return accountEntries.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                             !x.IsParentPostingEntry);
    }


    private FixedList<DateTime> GetMonthsInYear() {

      List<DateTime> monthsInYear = new List<DateTime>();

      for (int i = 1; i <= Query.InitialPeriod.ToDate.Month; i++) {

        DateTime monthInYear = DateTime.MinValue;

        if (i < Query.InitialPeriod.ToDate.Month) {

          monthInYear = new DateTime(Query.InitialPeriod.ToDate.Year, i,
                                     DateTime.DaysInMonth(Query.InitialPeriod.ToDate.Year, i));
        } else {

          monthInYear = Query.InitialPeriod.ToDate;
        }

        monthsInYear.Add(monthInYear);
      }

      return monthsInYear.ToFixedList();
    }


    private FixedList<ResumenAjusteAnualEntry> GetResumeBalancesByMonth(FixedList<DateTime> monthsInYear) {

      List<ResumenAjusteAnualEntry> resumenAjusteAnual = new List<ResumenAjusteAnualEntry>();

      foreach (var monthFilter in monthsInYear) {

        FixedList<TrialBalanceEntry> entriesFromBalanza = GetEntriesFromBalanza(monthFilter);

        resumenAjusteAnual.AddRange(ConvertBalanceEntryIntoResumeEntry(entriesFromBalanza));
      }

      this.Query.InitialPeriod.FromDate = fromDateFlag;
      this.Query.InitialPeriod.ToDate = toDateFlag;

      return new FixedList<ResumenAjusteAnualEntry>(resumenAjusteAnual);
    }





    #endregion Private methods

  } // class ResumenAjusteAnualHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
