/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : CashFlow Management                        Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Command DTO                             *
*  Type     : CashEntriesCommand                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input command used to update cash transaction entries.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Linq;

using Empiria.Financial.Integration.CashLedger;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Input command used to update cash transaction entries.</summary>
  public class CashEntriesCommand : SharedCashEntriesCommand {

    internal FixedList<CashEntry> GetEntries(CashTransaction transaction) {
      var allEntries = transaction.GetEntries();

      return allEntries.FindAll(x => this.Entries.Contains(x.Id));
    }

  }  // CashEntriesCommand

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
