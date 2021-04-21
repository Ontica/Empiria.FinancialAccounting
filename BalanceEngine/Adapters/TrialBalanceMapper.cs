/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : TrialBalanceMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map trial balances.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Domain;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {
  static internal class TrialBalanceMapper {

    static internal FixedList<TrialBalanceDto> Map(FixedList<TrialBalance> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<TrialBalanceDto>(mappedItems);
    }

    static internal TrialBalanceDto Map(TrialBalance trialBalance) {

      var dto = new TrialBalanceDto();

      dto.StandardAccountNumber = trialBalance.StandardAccountNumber;
      dto.StandardAccountName = trialBalance.StandardAccountName;
      dto.InitialBalance = trialBalance.InitialBalance;
      dto.Debit = trialBalance.Debit;
      dto.Credit = trialBalance.Credit;
      dto.CurrentBalance = trialBalance.CurrentBalance;

      return dto;
    }

  } // class TrialBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
