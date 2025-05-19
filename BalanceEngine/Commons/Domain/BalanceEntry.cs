/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : TrialBalanceEntry                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a balance entry.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a balance entry.</summary>
  public class BalanceEntry {

    #region Constructors and parsers

    internal BalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get; private set;
    }

    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    public Currency Currency {
      get; private set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public StandardAccount Account {
      get; private set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get; private set;
    }


    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(decimal))]
    public int SubledgerAccountId {
      get; private set;
    }


    [DataField("SALDO_ANTERIOR")]
    public decimal InitialBalance {
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


    [DataField("SALDO_ACTUAL")]
    public decimal CurrentBalance {
      get; private set;
    }


    [DataField("SALDO_PROMEDIO")]
    public decimal AverageBalance {
      get; private set;
    }


    [DataField("FECHA_ULTIMO_MOVIMIENTO")]
    public DateTime LastChangeDate {
      get; private set;
    }

  } //class BalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
