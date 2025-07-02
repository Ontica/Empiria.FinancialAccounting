/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : ExchangeRateForCurrencies                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to valuate currencies depending rules for balances.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.Time;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to valuate currencies depending rules for balances.</summary>
  internal class ExchangeRateForCurrencies {

    private readonly DateTime fromDate;
    private readonly DateTime toDate;

    #region Constructors and parsers

    internal ExchangeRateForCurrencies() {

    }


    internal ExchangeRateForCurrencies(DateTime _fromDate, DateTime _toDate) {
      Assertion.Require(_fromDate, nameof(_fromDate));
      Assertion.Require(_toDate, nameof(_toDate));

      this.fromDate = _fromDate;
      this.toDate = _toDate;
    }

    #endregion Constructors and parsers

    #region Properties

    internal string ExchangeRateTypeUID {
      get; private set;
    }


    internal string ValuateToCurrrencyUID {
      get; private set;
    }


    internal DateTime ExchangeRateDate {
      get; private set;
    }


    internal FixedList<ExchangeRate> ExchangeRateList {
      get; private set;
    }

    #endregion Properties

    #region Public methods

    internal ExchangeRateForCurrencies GetDefaultExchangeRate() {

      DetermineExchangeRateTypeForDate();

      this.ValuateToCurrrencyUID = Currency.MXN.Code;
      this.ExchangeRateList = ExchangeRate.GetList(ExchangeRateType.Parse(this.ExchangeRateTypeUID),
                                                                          this.ExchangeRateDate);
      return this;
    }


    internal ExchangeRateForCurrencies GetExchangeRateByQuery(BalancesPeriod period) {

      DetermineExchangeRateTypeForDate();

      this.ExchangeRateTypeUID = period.ExchangeRateTypeUID;
      this.ValuateToCurrrencyUID = period.ValuateToCurrrencyUID;
      this.ExchangeRateList = ExchangeRate.GetList(ExchangeRateType.Parse(period.ExchangeRateTypeUID),
                                                   ExchangeRateDate);
      return this;
    }


    internal string InvalidExchangeRateTypeMsg() {

      return $"No se ha registrado el tipo de cambio " +
             $"{ExchangeRateType.Parse(this.ExchangeRateTypeUID).Name} " +
             $"en la fecha {this.ExchangeRateDate} ";
    }

    #endregion Public methods

    #region Private methods

    private void DetermineExchangeRateTypeForDate() {
      var lastDate = toDate;

      while (true) {

        if (lastDate.Day == GetDaysInMonth(lastDate)) {

          GetClosingExchangeRateType(lastDate);
          break;
        } else if (lastDate == GetLastWorkingDate(lastDate)) {

          GetDailyExchangeRateType(lastDate);
          break;
        }

        lastDate = lastDate.AddDays(-1);
      }
    }


    private void GetClosingExchangeRateType(DateTime exchangeRateDate) {
      this.ExchangeRateDate = exchangeRateDate;
      this.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
    }


    private void GetDailyExchangeRateType(DateTime lastWorkingDate) {

      this.ExchangeRateDate = lastWorkingDate;
      this.ExchangeRateTypeUID = ExchangeRateType.Diario.UID;
    }


    private int GetDaysInMonth(DateTime date) {

      return DateTime.DaysInMonth(date.Year, date.Month);
    }


    private DateTime GetLastWorkingDate(DateTime date, bool includeDate = true) {

      var calendar = EmpiriaCalendar.Default;
      return calendar.LastWorkingDate(date, includeDate);
    }

    #endregion Private methods

  } // class ExchangeRateForCurrencies


} // namespace Empiria.FinancialAccounting.BalanceEngine
