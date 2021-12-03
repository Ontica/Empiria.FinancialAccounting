/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : VouchersByAccountConstructor               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate vouchers by account.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services to generate vouchers by account.</summary>
  internal class VouchersByAccountConstructor {

    private readonly BalanceCommand Command;

    internal VouchersByAccountConstructor(BalanceCommand command) {
      Assertion.AssertObject(command, "command");

      Command = command;
    }



    #region Public methods

    internal VouchersByAccount Build() {

      FixedList<VouchersByAccountEntry> voucherEntries = GetVoucherEntries();

      var returnedVoucherEntries = new FixedList<IVouchersByAccountEntry>(
                                        voucherEntries.Select(x => (IVouchersByAccountEntry) x));

      return new VouchersByAccount(Command, returnedVoucherEntries);
    }



    #endregion Public methods


    private FixedList<VouchersByAccountEntry> GetVoucherEntries() {

      VouchersByAccountCommandData commandData = VouchersByAccountCommandDataMapped();

      return StoredVoucherDataService.GetVouchersByAccountEntries(commandData);
    }

    private VouchersByAccountCommandData VouchersByAccountCommandDataMapped() {

      var commandExtensions = new VouchersByAccountCommandExtensions(Command);
      VouchersByAccountCommandData commandData = commandExtensions.MapToVouchersByAccountCommandData();
      
      return commandData;
    }

    #region Private methods

    #endregion

  } // class VouchersByAccountConstructor

} // namespace Empiria.FinancialAccounting.BalanceEngine
