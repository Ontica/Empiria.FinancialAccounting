/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                          Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : AccountingCalendarController                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to manage information about accounting calendars and their opened periods.        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Web API used to manage information about accounting
  /// calendars and their opened periods.</summary>
  public class AccountingCalendarController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/catalogues/accounting-calendars")]
    public CollectionModel GetAcountingCalendarsList() {

      using (var usecases = AccountingCalendarUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> calendars = usecases.GetAccountingCalendars();

        return new CollectionModel(base.Request, calendars);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/catalogues/accounting-calendars/{calendarUID:guid}")]
    public SingleObjectModel GetAcountingCalendar([FromUri] string calendarUID) {

      using (var usecases = AccountingCalendarUseCases.UseCaseInteractor()) {
        AccountingCalendarDto calendar = usecases.GetAccountingCalendar(calendarUID);

        return new SingleObjectModel(base.Request, calendar);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/catalogues/accounting-calendars/{calendarUID:guid}/add-date/{date}")]
    public SingleObjectModel AddDateToAccountingCalendar([FromUri] string calendarUID, [FromUri] DateTime date) {

      using (var usecases = AccountingCalendarUseCases.UseCaseInteractor()) {
        AccountingCalendarDto calendar = usecases.AddDateToAccountingCalendar(calendarUID, date);

        return new SingleObjectModel(base.Request, calendar);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/catalogues/accounting-calendars/{calendarUID:guid}/remove-date/{date}")]
    public SingleObjectModel RemoteDateFromAccountingCalendar([FromUri] string calendarUID, [FromUri] DateTime date) {

      using (var usecases = AccountingCalendarUseCases.UseCaseInteractor()) {
        AccountingCalendarDto calendar = usecases.RemoveDateFromAccountingCalendar(calendarUID, date);

        return new SingleObjectModel(base.Request, calendar);
      }
    }

    #endregion Web Apis

  }  // class AccountingCalendarController

}  // namespace Empiria.FinancialAccounting.WebApi
