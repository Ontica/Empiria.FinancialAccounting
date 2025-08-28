/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Domain Layer                            *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Information holder                      *
*  Type     : CashLedgerTotal                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information for a cash ledger total entry.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.CashLedger {

  /// <summary>Holds information for a cash ledger total entry.</summary>
  internal class CashLedgerTotal {

    #region Properties

    [DataField("ID_CUENTA_FLUJO")]
    public int CashAccountId {
      get; private set;
    }

    [DataField("NUM_CONCEPTO_FLUJO")]
    public string CashAccountNo {
      get; private set;
    }

    [DataField("ID_MONEDA", ConvertFrom = typeof(long))]
    public Currency Currency {
      get; private set;
    }

    [DataField("DEBE")]
    public decimal Debit {
      get; private set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get; private set;
    }

    #endregion Properties

  }  // class CashLedgerTotal

}  // namespace Empiria.FinancialAccounting.CashLedger
