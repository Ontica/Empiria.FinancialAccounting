/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : AccountingCalendarDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTOs with data related to an accounting calendar.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO with data related to an accounting calendar.</summary>
  public class AccountingCalendarDto {

    internal AccountingCalendarDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public FixedList<AccountingCalendarPeriodDto> Periods {
      get; internal set;
    }


    public FixedList<DateTime> OpenedAccountingDates {
      get; internal set;
    }


  }  // class AccountingCalendarDto



  /// <summary>Input and Output DTO for accounting calendar periods.</summary>
  public class AccountingCalendarPeriodDto {

    public AccountingCalendarPeriodDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string Period {
      get; set;
    }


    public DateTime FromDate {
      get; set;
    }


    public DateTime ToDate {
      get; set;
    }

  }  // class AccountingCalendarPeriodDto

}  // namespace Empiria.FinancialAccounting.Adapters
