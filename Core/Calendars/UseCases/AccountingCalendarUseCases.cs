/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : AccountingCalendarUseCases                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for accounting calendars management.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for accounting calendars management.</summary>
  public class AccountingCalendarUseCases : UseCase {

    #region Constructors and parsers

    protected AccountingCalendarUseCases() {
      // no-op
    }

    static public AccountingCalendarUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountingCalendarUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> GetExchangeRatesTypes() {
      var list = ExchangeRateType.GetList();

      return list.MapToNamedEntityList();
    }

    public FixedList<NamedEntityDto> GetAccountingCalendars() {
      throw new NotImplementedException();
    }

    public FixedList<ExchangeRateDto> GetExchangeRatesOnADate(DateTime date) {
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(date);

      return ExchangeRatesMapper.Map(exchangeRates);
    }

    public AccountingCalendarDto GetAccountingCalendar(string calendarUID) {
      throw new NotImplementedException();
    }

    public AccountingCalendarDto AddDateToAccountingCalendar(string calendarUID, DateTime date) {
      throw new NotImplementedException();
    }

    public AccountingCalendarDto RemoveDateFromAccountingCalendar(string calendarUID, DateTime date) {
      throw new NotImplementedException();
    }


    #endregion Use cases

  }  // class AccountingCalendarUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
