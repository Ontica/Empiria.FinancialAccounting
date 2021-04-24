/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : AccountBalanceMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map account balances.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map account balances.</summary>
  static internal class AccountBalanceMapper {

    static internal AccountBalanceDto Map(AccountBalance balance) {

      return new AccountBalanceDto {
        AccountNumber = balance.Account.Number,
        AccountName = balance.Account.Name,
        Total = balance.Total
      };
    }

  }  // class AccountBalanceMapper

}  // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
