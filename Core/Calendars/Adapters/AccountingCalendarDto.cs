﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : AccountingCalendarDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data related to an accounting calendar.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO with data related to an accounting calendar.</summary>
  public class AccountingCalendarDto {

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public FixedList<DateTime> OpenedAccountingDates {
      get; internal set;
    }


  }  // class AccountingCalendarDto

}  // namespace Empiria.FinancialAccounting.Adapters
