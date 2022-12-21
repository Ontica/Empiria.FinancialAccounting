/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Fields DTO                              *
*  Type     : AccountFieldsDto                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Fields DTO used for account fields edition.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.AccountsChartEdition.Adapters {

  /// <summary>Fields DTO used for account fields edition.</summary>
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
    } = AccountRole.Undefined;


    public string AccountTypeUID {
      get; set;
    } = string.Empty;


    public DebtorCreditorType DebtorCreditor {
      get; set;
    } = DebtorCreditorType.Undefined;

  }  // class AccountFieldsDto

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.Adapters
