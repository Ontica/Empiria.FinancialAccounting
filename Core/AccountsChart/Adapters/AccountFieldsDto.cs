/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : AccountFieldsDto                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for accounts edition.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

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
    }


    public DebtorCreditorType DebtorCreditor {
      get; set;
    }

  }  // class AccountFieldsDto

}  // namespace Empiria.FinancialAccounting.Adapters
