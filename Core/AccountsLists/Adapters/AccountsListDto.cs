/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Interface adapters                      *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Data Transfer Object                    *
*  Type     : AccountsListDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for accounts lists.                                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO for accounts lists.</summary>
  public class AccountsListDto {

    internal AccountsListDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }

    public FixedList<AccountsListItemDto> Items {
      get; internal set;
    }

  }  // class AccountsListDto


  /// <summary>Output DTO for accounts list items.</summary>
  public class AccountsListItemDto {

    internal AccountsListItemDto() {
      // no-op
    }

  }  // class AccountsListItemDto

}  // namespace Empiria.FinancialAccounting.Adapters
