/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : AccountsChartDto                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data related to an account chart and its contents.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO with data related to an account chart and its contents.</summary>
  public class AccountsChartDto {

    internal AccountsChartDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public FixedList<AccountDto> Accounts {
      get; internal set;
    }

  }  // public class AccountsChartDto


  /// <summary>Output DTO with an account data.</summary>
  public class AccountDto {

    internal AccountDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Number {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string Type {
      get; internal set;
    }


    public AccountRole Role {
      get; internal set;
    }


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    }


    public int Level {
      get; internal set;
    }


    public FixedList<Currency> Currencies {
      get; internal set;
    }

  }  // class AccountDto

}  // namespace Empiria.FinancialAccounting.Adapters
