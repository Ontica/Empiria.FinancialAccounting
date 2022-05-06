/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : AccountFieldsDto                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for account fields edition.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Command payload used for account fields edition.</summary>
  public class AccountFieldsDto {

    public string AccountNumber {
      get; set;
    } = string.Empty;


    public string Name {
      get; set;
    } = string.Empty;


    public string Description {
      get; set;
    } = string.Empty;


    public AccountRole Role {
      get; set;
    }


    public string AccountTypeUID {
      get; set;
    } = string.Empty;


    public DebtorCreditorType DebtorCreditor {
      get; set;
    }

  }  // class AccountFieldsDto

}  // namespace Empiria.FinancialAccounting.Adapters
