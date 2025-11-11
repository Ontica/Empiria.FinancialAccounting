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
using System.Linq;
using DocumentFormat.OpenXml.Drawing.Charts;
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

    internal FixedList<ResumenAjusteEntry> GetBalancesByMonths() {

      FixedList<DateTime> monthsInYear = GetMonthsInYear();

      FixedList<ResumenAjusteEntry> balancesByMonth = GetResumeBalancesByMonth(monthsInYear);

      return new FixedList<ResumenAjusteEntry>(balancesByMonth);
    }

    #endregion Public methods

    #region Private methods

    private FixedList<ResumenAjusteEntry> ConvertBalanceEntryIntoResumeEntry(
                                                FixedList<TrialBalanceEntry> entriesFromBalanza) {
      List<ResumenAjusteEntry> resumeByMonth = new List<ResumenAjusteEntry>();

      foreach (var entry in entriesFromBalanza) {
        ResumenAjusteEntry resumeEntry = new ResumenAjusteEntry();

        resumeEntry.MapFromTrialBalanceEntry(entry);
        resumeEntry.ConsultingDate = Query.InitialPeriod.ToDate;

        resumeEntry.KeyAdjustment = KeyAdjustmentTypes.SI; // TODO CARGAR CATALOGO
        resumeEntry.CalculateFields();

        resumeByMonth.Add(resumeEntry);
      }

      return new FixedList<ResumenAjusteEntry>(resumeByMonth);
    }


    private FixedList<TrialBalanceEntry> GetEntriesFromBalanza(DateTime monthFilter) {

      Query.InitialPeriod.FromDate = new DateTime(monthFilter.Year, monthFilter.Month, 1);
      Query.InitialPeriod.ToDate = monthFilter;

      var helper = new BalanzaTradicionalHelper(Query);

      FixedList<TrialBalanceEntry> accountEntries = helper.GetPostingEntries();

      return accountEntries.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                             !x.IsParentPostingEntry);
    }


    private void GetExchangeRate(FixedList<TrialBalanceEntry> entriesFromBalanza) {

      Query.InitialPeriod.ExchangeRateDate = Query.InitialPeriod.FromDate.AddDays(-1);
      Query.InitialPeriod.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
      Query.InitialPeriod.ValuateToCurrrencyUID = Currency.MXN.UID;

      var trialBalanceHelper = new TrialBalanceHelper(Query);

      trialBalanceHelper.ValuateAccountEntriesToExchangeRateV2(entriesFromBalanza);
      
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


    private FixedList<ResumenAjusteEntry> GetResumeBalancesByMonth(FixedList<DateTime> monthsInYear) {

      List<ResumenAjusteEntry> resumenAjusteAnual = new List<ResumenAjusteEntry>();

      foreach (var monthFilter in monthsInYear) {

        FixedList<TrialBalanceEntry> entriesFromBalanza = GetEntriesFromBalanza(monthFilter);

        GetExchangeRate(entriesFromBalanza);

        resumenAjusteAnual.AddRange(ConvertBalanceEntryIntoResumeEntry(entriesFromBalanza));
      }

      this.Query.InitialPeriod.FromDate = fromDateFlag;
      this.Query.InitialPeriod.ToDate = toDateFlag;

      return new FixedList<ResumenAjusteEntry>(resumenAjusteAnual);
    }

    #endregion Private methods

  } // class ResumenAjusteAnualHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
