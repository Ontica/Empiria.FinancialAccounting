/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Information holder                    *
*  Type     : AccountingRuleContext                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds a financial transaction and one of its associated budget entries.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Holds a financial transaction and one of its associated budget entries.</summary>
  public class AccountingRuleProcessingResult {

    #region Fields

    private readonly List<VoucherMovementFields> _movements = new List<VoucherMovementFields>(16);
    private readonly List<string> _pendingItems = new List<string>(16);

    #endregion Fields

    #region Properties

    internal FixedList<VoucherMovementFields> Movements {
      get {
        return _movements.ToFixedList();
      }
    }


    internal FixedList<string> PendingItems {
      get {
        return _pendingItems.ToFixedList();
      }
    }


    internal bool HasPendingItems {
      get {
        return _pendingItems.Count != 0;
      }
    }

    #endregion Properties

    #region Methods

    internal void AddMovement(VoucherMovementFields movement) {
      Assertion.Require(movement, nameof(movement));

      _movements.Add(movement);
    }


    internal void AddPendingItem(string message) {
      Assertion.Require(message, nameof(message));

      _pendingItems.Add(message);
    }

    #endregion Methods

  }  // class AccountingRuleProcessingResult

}  // namespace Empiria.FinancialAccounting.AccountingRules
