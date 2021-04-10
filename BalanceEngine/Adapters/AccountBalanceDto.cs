/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : AccountBalanceDto                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return account balances.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return account balances.</summary>
  public class AccountBalanceDto {

    public string AccountNumber {
      get; internal set;
    } = string.Empty;


    public string AccountName {
      get; internal set;
    } = string.Empty;


    public decimal Total {
      get; internal set;
    }

  }  // class AccountBalanceDto

}  // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
